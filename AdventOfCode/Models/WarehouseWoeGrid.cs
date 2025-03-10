using System.Numerics;
using System.Text;
using AdventOfCode.Enums;
using AdventOfCode.Extensions;

namespace AdventOfCode.Models;

internal class WarehouseWoeGrid
{
	#region Fields

	/// <summary>
	/// Container for the locations within the warehouse
	/// </summary>
	private WarehouseWoeGridCell[,] _cells = null!;

	/// <summary>
	/// Container to hold the list of movements to be made by the robot
	/// </summary>
	private Queue<DirectionOfTravel> _movements = new Queue<DirectionOfTravel>();

	/// <summary>
	/// Holds the maximum bounds of the warehouse
	/// </summary>
	private Vector2 _bounds;

	/// <summary>
	/// Holds the location of the robot
	/// </summary>
	private Vector2 _robotPosition;

	/// <summary>
	/// Stores the cell used by the robot
	/// </summary>
	private WarehouseWoeGridCell _robot = null!;

	#endregion

	#region Properties

	/// <summary>
	/// Returns the sum of all cell GPS values
	/// </summary>
	public int SumOfGPS => GetSumOfGPS();

	public WarehouseWoeGridCell this[Vector2 coord]
	{
		get
		{
			ArgumentOutOfRangeException.ThrowIfNegative(coord.X, nameof(coord));
			ArgumentOutOfRangeException.ThrowIfNegative(coord.Y, nameof(coord));
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(coord.X, _bounds.X, nameof(coord));
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(coord.Y, _bounds.Y, nameof(coord));
			return _cells[(int)coord.Y, (int)coord.X];
		}
		set
		{
			ArgumentOutOfRangeException.ThrowIfNegative(coord.X, nameof(coord));
			ArgumentOutOfRangeException.ThrowIfNegative(coord.Y, nameof(coord));
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(coord.X, _bounds.X, nameof(coord));
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(coord.Y, _bounds.Y, nameof(coord));
			_cells[(int)coord.Y, (int)coord.X] = value;
			value.MoveTo(coord);
		}
	}

	#endregion

	#region ctor
	#endregion

	#region Overrides

	/// <summary>
	/// Helper to display the warehouse as a textual representation
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		var sb = new StringBuilder();

		for (var y = 0; y < _bounds.Y; y++)
		{
			for (var x = 0; x < _bounds.X; x++)
				sb.Append(_cells[y, x].Descriptor.ToCharacter());
			//	Add newline after each row is completed
			sb.AppendLine();
		}
		return sb.ToString();
	}

	#endregion

	#region Methods

	/// <summary>
	/// Using the data from <paramref name="input"/>, convert the textual
	/// expression into the cell layout used in the warehouse class
	/// </summary>
	/// <param name="input">The textual representation of the warehouse</param>
	/// <param name="part">Indicates whether running part one or two of the challenge</param>
	/// <exception cref="ArgumentException"></exception>
	public void LoadWarehouseDetail(IEnumerable<string> input, int part = 1)
	{
		ArgumentNullException.ThrowIfNull(input, nameof(input));

		//	Only allow part one / two execution
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(part, nameof(part));
		ArgumentOutOfRangeException.ThrowIfGreaterThan(part, 2, nameof(part));
		var data = input.ToList();
		ArgumentOutOfRangeException.ThrowIfZero(data.Count, nameof(input));

		var maxLength = 100 / part;
		//	Extract length of first line of text
		var lineLength = data[0].Length;
		//	To ensure the GPS calculation for a point will work, the X-coord
		//	cannot be more than 100 (coords are transposed to one less as we
		//	start at top-left point [0,0] when loading into the grid itself)
		ArgumentOutOfRangeException.ThrowIfGreaterThan(lineLength, maxLength, nameof(input));

		//	All lines must be of equal length - otherwise there's a weirdly shaped warehouse
		if (data.Any(l => l.Length != lineLength))
			throw new ArgumentException("Ragged line lengths not supported", nameof(input));

		//	Set the bounds of the grid
		_bounds = new Vector2(lineLength * part, data.Count);

		//	Initialise new array
		_cells = new WarehouseWoeGridCell[data.Count, lineLength * part];
		//	Reset robot
		_robot = null!;

		//	Load in the data, transposing from the string to WarehouseWoeGridCell objects
		for (var y = 0; y < data.Count; y++)
		{
			var line = data[y];
			for (var x = 0; x < line.Length; x++)
			{
				//	Part one is simply stuff the transposed element in the cell
				if (part == 1)
				{
					var cell = new WarehouseWoeGridCell(new Vector2(x, y), line[x].ToWarehouseWoeCellType());
					if (cell.Descriptor == WarehouseWoeCellType.Robot)
						_robot = cell;
					_cells[y, x] = cell;
				}
				//	Part two has a subtle difference: all items are "doubled"
				else
				{
					var cellType = line[x].ToWarehouseWoeCellType();
					var (cellType1, cellType2) = cellType switch
					{
						//	Boxes now have left/right elements
						WarehouseWoeCellType.Box => (WarehouseWoeCellType.BoxLeft, WarehouseWoeCellType.BoxRight),
						//	Robots still only occupy 1 cell, but get an extra space padding
						WarehouseWoeCellType.Robot => (WarehouseWoeCellType.Robot, WarehouseWoeCellType.Empty),
						//	Other types such as Wall and Empty just get doubled
						_ => (cellType, cellType)
					};
					var cell1 = new WarehouseWoeGridCell(new Vector2(x * 2, y), cellType1);
					var cell2 = new WarehouseWoeGridCell(new Vector2(x * 2 + 1, y), cellType2);

					_cells[y, x * 2] = cell1;
					_cells[y, x * 2 + 1] = cell2;

					if (cellType1 == WarehouseWoeCellType.Robot || cellType2 == WarehouseWoeCellType.Robot)
					{
						_robot = cellType1 == WarehouseWoeCellType.Robot
							? cell1
							: cell2;
					}
				}
			}
		}
		//	Find the robot!
		SetRobotPosition();
	}

	/// <summary>
	/// Set the coords of the robot
	/// </summary>
	private void SetRobotPosition()
	{
		_robotPosition = _robot.Coord;
	}

	/// <summary>
	/// Loads a queue with the requested movements for the robot
	/// </summary>
	/// <param name="input">The textual representation of the movements</param>
	public void LoadRobotMovements(IEnumerable<string> input)
	{
		ArgumentNullException.ThrowIfNull(input);

		_movements = new Queue<DirectionOfTravel>();

		foreach (var line in input)
			foreach (var c in line.ToCharArray())
				_movements.Enqueue(c.ToDirectionOfTravel());
	}

	/// <summary>
	/// Moves the robot around the warehouse
	/// </summary>
	/// <param name="moves">The number of moves to be made</param>
	/// <param name="part">Indicates whether running part one or two of the challenge</param>
	/// <remarks>If the number of moves is zero, all remaining movement instrcutions are made</remarks>
	public void Move(int moves = 1, int part = 1)
	{
		//	Only allow part one / two execution
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(part, nameof(part));
		ArgumentOutOfRangeException.ThrowIfGreaterThan(part, 2, nameof(part));

		//	Cannot move if nothing left in the queue
		if (_movements.Count == 0)
			return;

		//	Special case: if zero is passed, all moves in the queue are to be made
		if (moves == 0)
			moves = _movements.Count;

		//	Cannot make more moves than exist in the queue
		var maxMoves = Math.Min(moves, _movements.Count);

		for (var i = 0; i < maxMoves; i++)
		{
			var movement = _movements.Dequeue();
			var movementOffset = movement.ToMovementOffset();
			var horizontalMovements = movementOffset.Y == 0;

			if (part == 1 ||
				(part == 2 && horizontalMovements))
				DoSimpleMovements(movementOffset);
			else
				DoComplexMovements(movementOffset);
		}
	}

	/// <summary>
	/// Refactor into separate method: simple movements are made if moving cells
	/// during part 1, or movement is horizontal for part 2
	/// </summary>
	/// <param name="movementOffset">The movement vector for the cells</param>
	private void DoSimpleMovements(Vector2 movementOffset)
	{
		//	Create a stack of cells that may be moving - always adding the robot to begin with (and end at)
		Stack<WarehouseWoeGridCell> cellsMoving = new Stack<WarehouseWoeGridCell>();
		cellsMoving.Push(_robot);

		//	Loop around finding cells needing to move
		var canMove = true;
		var currentLocation = _robot.Coord;

		while (canMove)
		{
			//	Get new location based on current+offset
			var newLocation = currentLocation + movementOffset;
			var newCell = this[newLocation];

			//	If a wall is found, stop movements
			canMove = newCell.Descriptor != WarehouseWoeCellType.Wall;

			//	If a move is possible, update location, add cell to stack of moving cells
			if (canMove)
			{
				currentLocation = newLocation;
				cellsMoving.Push(newCell);
				if (newCell.Descriptor == WarehouseWoeCellType.Empty)
					break;
			}
		}

		//	No movement possible, so return and loop to next movement
		if (!canMove)
			return;

		MoveCells(movementOffset, cellsMoving);
	}

	/// <summary>
	/// In part 2, vertical movements of cells is more complex as boxes occupy
	/// 2 cells and may be offset by 1 from those above/below
	/// </summary>
	/// <param name="movementOffset">The movement vector</param>
	/// <exception cref="NotImplementedException"></exception>
	private void DoComplexMovements(Vector2 movementOffset)
	{
		//	Do some quick checks: if the robot is moving into a space, then the simple movements will still work
		if (this[_robot.Coord + movementOffset].Descriptor == WarehouseWoeCellType.Empty)
		{
			DoSimpleMovements(movementOffset);
			return;
		}

		//	For part 2, vertical movements that include boxes are more complex
		//	and more cells need to be checked, as each box has left and right
		//	components. Therefore, if one side of a box is spotted above or below
		//	the robots current position, the corresponding element of the box
		//	needs to be located, then repeated until all linked boxes can be
		//	moved into a space, or a wall obstructs the movements
		var movingBoxes = new List<(WarehouseWoeGridCell left, WarehouseWoeGridCell right)>();
		//	Add the robot as the first cell so checking can start
		movingBoxes.Add((_robot, null!));

		//	Loop around finding boxes needing to move
		var canMove = true;
		var currentLocation = _robot.Coord;
		while (canMove)
		{
			//	Get the y-Coord of the row that was checked last
			var yCoord = movementOffset.Y > 0
				? movingBoxes.Max(m => m.left.Coord.Y)
				: movingBoxes.Min(m => m.left.Coord.Y);

			//	Get all box elements of that row in ascending x-Coord
			var horizontalChecks = movingBoxes.Where(q => q.left.Coord.Y == yCoord)
				.Where(q => q.left.Descriptor != WarehouseWoeCellType.Empty)
				.OrderBy(q => q.left.Coord.X)
				.ToList();

			//	Obtain a list of all x-Coords that require checks to be performed
			var xCoords = horizontalChecks.Select(s => (int)s.left.Coord.X).ToList();
			xCoords.AddRange(horizontalChecks.Where(q => q.right is not null).Select(s => (int)s.right.Coord.X));

			//	Get a list of cell coords to check using the current list of cells to be checked and applying the offset to each one
			var coordsToCheck = xCoords.OrderBy(o => o).Select(x => new Vector2(x, yCoord) + movementOffset).ToList();
			//	From the new coords, access the cells
			var cellsToCheck = coordsToCheck.Select(s => this[s]).ToList();

			//	For movement to be permitted none of the cells being checked can be a wall
			canMove = cellsToCheck.All(c => c.Descriptor != WarehouseWoeCellType.Wall);
			//	If we spot a wall, stop now
			if (!canMove)
				return;

			//	Fun starts here - need to spot boxes that are offset to each other and add those if needs be
			//	Create a container for boxes to be added to the list of movements to be made
			var newBoxes = new List<(WarehouseWoeGridCell left, WarehouseWoeGridCell right)>();

			for (var c = 0; c < cellsToCheck.Count; c++)
			{
				//	Get the cell being checked
				var cell = cellsToCheck[c];

				//	If the cell has already been processed, move to next cell
				if (newBoxes.Any(q => q.left == cell || q.right == cell))
					continue;

				//	If the cell is a BoxLeft, add the corresponding right for it
				if (cell.Descriptor == WarehouseWoeCellType.BoxLeft)
				{
					var boxRight = _cells[(int)cell.Coord.Y, (int)cell.Coord.X + 1];
					if (boxRight.Descriptor != WarehouseWoeCellType.BoxRight)
						throw new ArgumentException($"Box left without corresponding right found at {cell.Coord}");
					newBoxes.Add((cell, boxRight));
					continue;
				}

				//	If the cell is a BoxRight, add the corresponding left for it
				if (cell.Descriptor == WarehouseWoeCellType.BoxRight)
				{
					var boxLeft = _cells[(int)cell.Coord.Y, (int)cell.Coord.X - 1];
					if (boxLeft.Descriptor != WarehouseWoeCellType.BoxLeft)
						throw new ArgumentException($"Box right without corresponding left found at {cell.Coord}");
					newBoxes.Add((boxLeft, cell));
					continue;
				}

				// //	Add the empty space to the new list of "boxes"
				// newBoxes.Add((cell, null!));
			}
			//	Add the list of new movements to existing cells
			movingBoxes.AddRange(newBoxes);

			// if the check list is all spaces, then stop
			if (cellsToCheck.All(c => c.Descriptor == WarehouseWoeCellType.Empty))
				break;
		}

		var groupedMoves = movingBoxes.GroupBy(g => (int)g.left.Coord.Y)
			.Select(s => new { Row = s.Key, Boxes = s.OrderBy(o => o.left.Coord.X).ToList() });

		var rowsOfMoves = movementOffset.Y > 0
			? groupedMoves.OrderByDescending(o => o.Row).ToList()
			: groupedMoves.OrderBy(o => o.Row).ToList();

		foreach (var row in rowsOfMoves)
		{
			foreach (var box in row.Boxes)
			{
				//	Move the left element as this will always exist
				var oldLeft = box.left.Coord;
				var newLeft = box.left.Coord + movementOffset;
				var emptyLeft = this[newLeft];
				this[newLeft] = box.left;
				this[oldLeft] = emptyLeft;

				//	The right element may be null, if so skip trying to move something that does not exist
				if (box.right is null)
					continue;
				var oldRight = box.right.Coord;
				var newRight = box.right.Coord + movementOffset;
				var emptyRight = this[newRight];
				this[newRight] = box.right;
				this[oldRight] = emptyRight;
			}
		}
	}

	/// <summary>
	/// Refactored: moving of the cells extracted to its own method to support
	/// part 2 vertical movements
	/// </summary>
	/// <param name="movementOffset">The movement vector</param>
	/// <param name="cellsMoving">A stack of cells that are moving</param>
	private void MoveCells(Vector2 movementOffset, Stack<WarehouseWoeGridCell> cellsMoving)
	{
		WarehouseWoeGridCell emptyCell = null!;
		Vector2 oldLocation = new Vector2(0, 0);
		Vector2 newLocation;

		//	Fun starts here as we move cells around
		while (cellsMoving.Count > 0)
		{
			var movingCell = cellsMoving.Pop();
			oldLocation = movingCell.Coord;
			newLocation = movingCell.Coord + movementOffset;

			//	An empty cell "jumps" into the location where the last cell of the stack is currently
			if (movingCell.Descriptor == WarehouseWoeCellType.Empty)
			{
				emptyCell = movingCell;
				continue;
			}

			//	Update the warehouse grid and cell location
			this[newLocation] = movingCell;
		}

		if (emptyCell is null)
			return;

		this[oldLocation] = emptyCell;
	}

	/// <summary>
	/// Calculates the sum of the GPS values within the warehouse
	/// </summary>
	/// <returns>The summation of GPS values</returns>
	private int GetSumOfGPS()
	{
		var gpsSum = 0;
		for (var y = 0; y < _bounds.Y; y++)
			for (var x = 0; x < _bounds.X; x++)
				gpsSum += _cells[y, x].GPS;
		return gpsSum;
	}

	#endregion
}