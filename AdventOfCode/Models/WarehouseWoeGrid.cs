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
	/// <exception cref="ArgumentException"></exception>
	public void LoadWarehouseDetail(IEnumerable<string> input)
	{
		ArgumentNullException.ThrowIfNull(input, nameof(input));
		var data = input.ToList();
		ArgumentOutOfRangeException.ThrowIfZero(data.Count, nameof(input));

		//	Extract length of first line of text
		var lineLength = data[0].Length;
		//	To ensure the GPS calculation for a point will work, the X-coord
		//	cannot be more than 100 (coords are transposed to one less as we
		//	start at top-left point [0,0] when loading into the grid itself)
		ArgumentOutOfRangeException.ThrowIfGreaterThan(lineLength, 100, nameof(input));

		//	All lines must be of equal length - otherwise there's a weirdly shaped warehouse
		if (data.Any(l => l.Length != lineLength))
			throw new ArgumentException("Ragged line lengths not supported", nameof(input));

		//	Set the bounds of the grid
		_bounds = new Vector2(lineLength, data.Count);

		//	Initialise new array
		_cells = new WarehouseWoeGridCell[data.Count, lineLength];

		//	Load in the data, transposing from the string to WarehouseWoeGridCell objects
		for (var y = 0; y < data.Count; y++)
		{
			var line = data[y];
			for (var x = 0; x < line.Length; x++)
			{
				var cell = new WarehouseWoeGridCell(new Vector2(x, y), line[x].ToWarehouseWoeCellType());
				if (cell.Descriptor == WarehouseWoeCellType.Robot)
					_robot = cell;
				_cells[y, x] = cell;
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
	/// <remarks>If the number of moves is zero, all remaining movement instrcutions are made</remarks>
	public void Move(int moves = 1)
	{
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
				var newCell = _cells[(int)newLocation.Y, (int)newLocation.X];

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

			//	No movement possible, so loop to next movement
			if (!canMove)
				continue;

			//	Fun starts here as we move cells around
			while (cellsMoving.Count > 0)
			{
				var x = 0;
				var y = 0;
				var movingCell = cellsMoving.Pop();
				//	An empty cell "jumps" into the location where the robot is currently
				if (movingCell.Descriptor == WarehouseWoeCellType.Empty)
				{
					x = (int)_robot.Coord.X;
					y = (int)_robot.Coord.Y;
				}
				else
				{
					var newLocation = movingCell.Coord + movementOffset;
					x = (int)newLocation.X;
					y = (int)newLocation.Y;
				}
				//	Update the warehouse grid
				_cells[y, x] = movingCell;
				//	Update the location within the cell itself
				movingCell.MoveTo(x, y);
			}
		}
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