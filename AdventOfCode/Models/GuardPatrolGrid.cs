using System.Text;
using AdventOfCode.Enums;
using AdventOfCode.Extensions;

namespace AdventOfCode.Models;

internal class GuardPatrolGrid
{
	#region Fields

	public DirectionOfTravel StartDirection { get; private set; }

	public int StartRow { get; private set; }

	public int StartColumn { get; private set; }

	public (int min, int max) PatrolWidth { get; private set; }

	public (int min, int max) PatrolHeight { get; private set; }

	private readonly List<List<CellState>> _patrolGrid = new List<List<CellState>>();

	private byte[,] _visitCounter = null!;

	#endregion

	#region ctor

	public GuardPatrolGrid()
	{
		//	Initialise current direction of travel and location as "not known"
		StartDirection = DirectionOfTravel.Unknown;
		StartRow = -1;
		StartColumn = -1;
		PatrolWidth = (0, 0);
		PatrolHeight = (0, 0);
	}

	#endregion

	#region Public methods

	/// <summary>
	/// Loads a patrol grid into the class
	/// </summary>
	/// <param name="input"></param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public void LoadGrid(List<string> input)
	{
		//	input must have a value
		ArgumentNullException.ThrowIfNull(input, nameof(input));

		//	Calculate the lengths of the lines of each element in the grid
		var gridLengths = input.Select(x => x.Length).ToList();

		//	Verify that all lengths are equal - the grid must be lines of equal lengths
		if (!gridLengths.All(x => x == gridLengths[0]))
			throw new ArgumentOutOfRangeException(nameof(input), "Grid input has ragged lengths");

		//	set bounds for the grid
		PatrolWidth = (0, gridLengths[0] - 1);
		PatrolHeight = (0, gridLengths.Count - 1);

		//	Clear any current contents of the grid
		_patrolGrid.Clear();
		_visitCounter = new byte[gridLengths.Count, gridLengths[0]];

		var cellRow = PatrolHeight.min;
		//	Loop for each row in the grid
		foreach (var row in input)
		{
			var cellCol = PatrolWidth.min;
			var cellStates = new List<CellState>();
			//	Loop for each cell in the row
			foreach (var cell in row.ToCharArray())
			{
				//	Convert to the state enum
				var cellState = ConvertCharToCellState(cell);
				cellStates.Add(cellState);

				//	If the current cell is the current position, store that location and direction of travel
				if (cellState == CellState.CurrentPosition)
				{
					StartDirection = cell.ToDirectionOfTravel();
					StartRow = cellRow;
					StartColumn = cellCol;
				}

				//	Check if the location is visited
				if (cellState == CellState.Visited)
				{
					_visitCounter[cellRow, cellCol] = 1;
				}
				cellCol++;
			}
			_patrolGrid.Add(cellStates);
			cellRow++;
		}
	}

	/// <summary>
	/// helper method to determine if the location (supplied as <paramref name="row"/>
	/// and <paramref name="column"/>) are within the bounds of the patrol grid
	/// </summary>
	/// <param name="row">The y-coordinate of the location</param>
	/// <param name="column">The x-coordinate of the location</param>
	/// <returns>True if within the patrol grid, otherwise false</returns>
	public bool IsLocationValid(int row, int column)
	{
		return column >= PatrolWidth.min &&
				column <= PatrolWidth.max &&
				row >= PatrolHeight.min &&
				row <= PatrolHeight.max;
	}

	/// <summary>
	/// Helper method to access the grid and find out the state of a cell
	/// </summary>
	/// <param name="row">The y-coordinate of the location</param>
	/// <param name="column">The x-coordinate of the location</param>
	/// <returns>The <see cref="CellState"/> of the location, or <see cref="CellState.OutOfBounds"/></returns>
	public CellState GetCellState(int row, int column)
	{
		//	Validate co-ordinates fall within bounds of grid
		if (!IsLocationValid(row, column))
			return CellState.OutOfBounds;

		var visitCount = _visitCounter[row, column];
		var state = _patrolGrid[row][column];

		return state switch
		{
			CellState.Visited => visitCount switch
			{
				0 => CellState.Unvisited,
				1 => state,
				< 5 => CellState.VisitedAtLeastTwice,
				_ => CellState.StuckInLoop
			},
			CellState.CurrentPosition => visitCount switch
			{
				< 5 => CellState.CurrentPosition,
				_ => CellState.StuckInLoop,
			},
			_ => state
		};
	}

	/// <summary>
	/// Helper method to allow the grid to be updated based on whether a position has been visited etc.
	/// </summary>
	/// <param name="row">The y-coordinate of the location</param>
	/// <param name="column">The x-coordinate of the location</param>
	/// <param name="state">The new <see cref="CellState"/> for the location</param>
	/// <returns>True if the change completed successfully</returns>
	/// <remarks>
	/// If the proposed change can be completed, then the grid shall be updated accordingly
	/// </remarks>
	public (bool, CellState) SetCellState(int row, int column, CellState state)
	{
		var currentState = GetCellState(row, column);
		var (canChange, newState) = CanChangeState(currentState, state);

		if (canChange)
		{
			_patrolGrid[row][column] = newState;
			if (newState == CellState.Visited)
			{
				_visitCounter[row, column] = (byte)(1 + _visitCounter[row, column]);
			}
		}
		return (canChange, GetCellState(row, column));
	}

	/// <summary>
	/// Helper method to return the number of cells that were visited during a patrol
	/// </summary>
	/// <returns>The total number of cells that have the <see cref="CellState.Visited"/> value</returns>
	public int GetVisitedCellCount()
	{
		var count = 0;
		foreach (var row in _patrolGrid)
		{
			count += row.Where(x => x == CellState.Visited).Count();
		}
		return count;
	}

	/// <summary>
	/// provide a method to allow a new obstruction to be placed at a specific location
	/// </summary>
	/// <param name="row">The y-coordinate of the obstruction</param>
	/// <param name="column">The x-coordinate of the obstruction</param>
	/// <returns>True if the placement succeeded, otherwise false</returns>
	public bool SetNewObstruction(int row, int column)
	{
		//	get the current state of the location
		//	if the location is invalid, we will see OutOfBounds
		var currentState = GetCellState(row, column);

		//	We can only set a position as an obstruction if it hasn't been visited
		if (currentState != CellState.Unvisited)
			return false;

		_patrolGrid[row][column] = CellState.Obstructed;
		return GetCellState(row, column) == CellState.Obstructed;
	}

	#endregion

	#region Private methods

	/// <summary>
	/// Converts the specified <paramref name="cell"/> character to equivalent <see cref="CellState"/>
	/// </summary>
	/// <param name="cell">The character to be converted</param>
	/// <returns>The <see cref="CellState"/> for the grid</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	private static CellState ConvertCharToCellState(char cell)
	{
		return cell switch
		{
			'.' => CellState.Unvisited,
			'#' => CellState.Obstructed,
			'^' => CellState.CurrentPosition,
			'<' => CellState.CurrentPosition,
			'>' => CellState.CurrentPosition,
			'v' => CellState.CurrentPosition,
			'X' => CellState.Visited,
			_ => throw new ArgumentOutOfRangeException(nameof(cell), $"Invalid cell found: {cell}")
		};
	}

	/// <summary>
	/// Works as a simple state machine to determine whether the current cell
	/// can be changed from <paramref name="currentState"/> into
	/// <paramref name="newState"/>
	/// </summary>
	/// <param name="currentState">The state of the cell now</param>
	/// <param name="newState">The proposed future state of the cell</param>
	/// <returns>True if it can change state and what that new state shall be, or false and current state</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	private static (bool, CellState) CanChangeState(CellState currentState, CellState newState)
	{
		var allowedStates = currentState switch
		{
			//	Unvisited can stay unvisited, or change to current position or visited
			CellState.Unvisited => new[] { CellState.Unvisited, CellState.CurrentPosition, CellState.Visited },
			//	Obstructed can only stay obstructed
			CellState.Obstructed => new[] { CellState.Obstructed },
			//	Current position can stay current, or be visited
			CellState.CurrentPosition => new[] { CellState.CurrentPosition, CellState.Visited },
			//	OutOfBounds can only ever remain as such
			CellState.OutOfBounds => new[] { CellState.OutOfBounds },
			//	Visited can only stay visited
			CellState.Visited => new[] { CellState.CurrentPosition, CellState.Visited, CellState.VisitedAtLeastTwice },
			CellState.VisitedAtLeastTwice => new[] { CellState.CurrentPosition, CellState.Visited, CellState.VisitedAtLeastTwice, CellState.StuckInLoop },
			CellState.StuckInLoop => new[] { CellState.CurrentPosition, CellState.Visited, CellState.StuckInLoop },
			_ => throw new ArgumentOutOfRangeException(nameof(currentState)),
		};

		//	Determine if the change can be made
		var canChange = allowedStates.Contains(newState);

		//	Set the value of the valid state to be returned
		var changedState = canChange
			? allowedStates.Length == 1
				? allowedStates[0]
				: newState
			: currentState;

		return (canChange, changedState);
	}

	/// <summary>
	/// Converts the <paramref name="state"/> into a corresponding character for display
	/// </summary>
	/// <param name="state">The <see cref="CellState"/> to convert</param>
	/// <returns>The character equivalent for <paramref name="state"/></returns>
	private static char CellStateToChar(CellState state)
	{
		return state switch
		{
			CellState.Unvisited => '.',
			CellState.Obstructed => '#',
			CellState.CurrentPosition => 'C',
			CellState.Visited => 'X',
			CellState.VisitedAtLeastTwice => '2',
			CellState.StuckInLoop => 'S',
			_ => '?'
		};
	}

	#endregion

	/// <summary>
	/// allows printing out of the current state of the grid to see how it is progressing
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.AppendLine();

		for (var row = PatrolHeight.min; row <= PatrolHeight.max; row++)
		{
			for (var col = PatrolWidth.min; col <= PatrolWidth.max; col++)
			{
				sb.Append(CellStateToChar(GetCellState(row, col)));
			}
			sb.AppendLine();
		}

		return sb.ToString();
	}
}