using AdventOfCode.Enums;

namespace AdventOfCode.Models;

internal class GuardPatrolGrid
{
	#region Fields

	public GuardDirection StartDirection { get; private set; }

	public int StartRow { get; private set; }

	public int StartColumn { get; private set; }

	public (int min, int max) PatrolWidth { get; private set; }

	public (int min, int max) PatrolHeight { get; private set; }

	private readonly List<List<CellState>> _patrolGrid = new List<List<CellState>>();

	#endregion

	#region ctor

	public GuardPatrolGrid()
	{
		//	Initialise current direction of travel and location as "not known"
		StartDirection = GuardDirection.Unknown;
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

		//	Loop for each row in the grid
		foreach (var row in input)
		{
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
					StartDirection = ConvertToGuardDirection(cell);
					StartRow = input.IndexOf(row);
					StartColumn = row.IndexOf(cell);
				}
			}
			_patrolGrid.Add(cellStates);
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
		return IsLocationValid(row, column)
			? _patrolGrid[row][column]
			: CellState.OutOfBounds;
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
	public bool SetCellState(int row, int column, CellState state)
	{
		var currentState = GetCellState(row, column);
		var (canChange, newState) = CanChangeState(currentState, state);

		if (canChange)
			_patrolGrid[row][column] = newState;
		return canChange;
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
	/// Converts the character representation of the guards current location to a <see cref="GuardDirection"/>
	/// </summary>
	/// <param name="cell">The character to transpose</param>
	/// <returns>The direction of travel for the guard</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	private GuardDirection ConvertToGuardDirection(char cell)
	{
		return cell switch
		{
			'^' => GuardDirection.North,
			'>' => GuardDirection.East,
			'v' => GuardDirection.South,
			'<' => GuardDirection.West,
			_ => throw new ArgumentOutOfRangeException(nameof(cell))
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
			CellState.Visited => new[] { CellState.Visited },
			_ => throw new ArgumentOutOfRangeException(nameof(currentState)),
		};

		//	Determine if the change can be made
		var canChange = allowedStates.Contains(newState);

		//	Set the value of the valid state to be returned
		newState = canChange
			? allowedStates.Length == 1
				? allowedStates[0]
				: newState
			: currentState;

		return (canChange, newState);
	}

	#endregion
}