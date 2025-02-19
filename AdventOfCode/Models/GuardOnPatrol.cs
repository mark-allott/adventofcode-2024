using AdventOfCode.Enums;

namespace AdventOfCode.Models;

internal class GuardOnPatrol
{
	private GuardPatrolGrid _patrolGrid = null!;

	public GuardOnPatrol()
	{
	}

	/// <summary>
	/// method that performs the required actions that represent a guard patrolling a grid
	/// </summary>
	/// <param name="patrolGrid">The grid to be patrolled</param>
	/// <exception cref="NotImplementedException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	public void Patrol(GuardPatrolGrid patrolGrid)
	{
		ArgumentNullException.ThrowIfNull(patrolGrid, nameof(patrolGrid));

		_patrolGrid = patrolGrid;
		var canMove = true;
		var currDirection = patrolGrid.StartDirection;
		var currRow = patrolGrid.StartRow;
		var currCol = patrolGrid.StartColumn;
		var currState = CellState.CurrentPosition;

		while (canMove)
		{
			//	Can we move in the current direction?
			(canMove, currState) = CanMove(currRow, currCol, currDirection);

			//	Yes, so perform the move
			if (canMove)
			{
				var (newRow, newCol) = GetNewLocation(currRow, currCol, currDirection);
				SetNewLocation((currRow, currCol), (newRow, newCol), currDirection);
				currRow = newRow;
				currCol = newCol;
			}
			//	can't move so check if reason is going out of the grid
			else if (currState == CellState.OutOfBounds)
			{
				patrolGrid.SetCellState(currRow, currCol, CellState.Visited);
			}
			//	can't move because something is in the way, so turn to the right
			else if (currState == CellState.Obstructed)
			{
				currDirection = currDirection switch
				{
					GuardDirection.North => GuardDirection.East,
					GuardDirection.East => GuardDirection.South,
					GuardDirection.South => GuardDirection.West,
					GuardDirection.West => GuardDirection.North,
					_ => throw new NotImplementedException()
				};
				canMove = true;
			}
			else
			{
				throw new InvalidOperationException($"Invalid operation encountered at ({currRow}, {currCol}), moving {currDirection} with state {currState}");
			}
		}
	}

	/// <summary>
	/// Based on the <paramref name="direction"/> of travel, calculate the offset
	/// in coordinates needed to travel in the direction indicated
	/// </summary>
	/// <param name="direction">The intended direction of travel</param>
	/// <returns>The offset coordinates for travel</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <remarks>
	/// Guards move only one cell at a time in one direction before having to
	/// check for any obstructions that would require a change of direction to
	/// proceed with the patrol
	/// </remarks>
	private static (int oRow, int oColumn) GetCellOffset(GuardDirection direction)
	{
		var (offsetRow, offsetColumn) = direction switch
		{
			GuardDirection.North => (-1, 0),
			GuardDirection.East => (0, 1),
			GuardDirection.South => (1, 0),
			GuardDirection.West => (0, -1),
			_ => throw new ArgumentOutOfRangeException(nameof(direction)),
		};

		return (offsetRow, offsetColumn);
	}

	/// <summary>
	/// Calculate the new location in the patrol grid for the guard
	/// </summary>
	/// <param name="currRow">The current y-coordinate in the grid</param>
	/// <param name="currCol">The current x-coordinate in the grid</param>
	/// <param name="direction">The direction in which the guard is to travel</param>
	/// <returns>The coordinates for the new location</returns>
	private static (int newRow, int newCol) GetNewLocation(int currRow, int currCol, GuardDirection direction)
	{
		var (rowOffset, colOffset) = GetCellOffset(direction);
		return (currRow + rowOffset, currCol + colOffset);
	}

	/// <summary>
	/// Based on the rules of the challenge a guard can only move one cell at
	/// a time in the specified direction before having to check to see if they
	/// can continue, or are blocked by an obstruction. If blocked, then they
	/// would need to turn to the right in order to proceed.
	/// </summary>
	/// <param name="currRow">The current y-coordinate of the guard in the grid</param>
	/// <param name="currCol">the current x-coordinate of the guard in the grid</param>
	/// <param name="direction">The planned direction of travel</param>
	/// <returns>True if the move can be made, otherwise false and the cell state of the new location</returns>
	private (bool canMove, CellState cellState) CanMove(int currRow, int currCol, GuardDirection direction)
	{
		var (newRow, newColumn) = GetNewLocation(currRow, currCol, direction);
		var newState = _patrolGrid?.GetCellState(newRow, newColumn) ?? CellState.Unknown;

		//	moving is only valid when the new state is not Unknown, Obstructed or OutOfBounds
		return (!new[] { CellState.Unknown, CellState.Obstructed, CellState.OutOfBounds }.Contains(newState), newState);
	}

	/// <summary>
	/// Performs the required actions to move the guard into the new location, set the previous location as "visited" and set the new location as "current"
	/// </summary>
	/// <param name="current">The current location of the guard on patrol</param>
	/// <param name="future">The new location for the guard</param>
	/// <param name="direction">The direction the guard is travelling</param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	private void SetNewLocation((int row, int col) current, (int row, int col) future, GuardDirection direction)
	{
		//	Calculate the offset from current location to next location
		var (offsetRow, offsetColumn) = GetCellOffset(direction);
		var (rowDelta, colDelta) = (future.row - current.row, future.col - current.col);

		//	Verify that we're moving by valid range
		if (offsetRow != rowDelta || offsetColumn != colDelta)
			throw new ArgumentOutOfRangeException(nameof(direction), $"Cannot move '{direction}' from ({current.row}, {current.col}) to ({future.row}, {future.col})");

		//	Set current position as visited
		_patrolGrid?.SetCellState(current.row, current.col, CellState.Visited);

		//	Set the new position as current if it is valid
		if (_patrolGrid?.IsLocationValid(future.row, future.col) ?? false)
			_patrolGrid?.SetCellState(future.row, future.col, CellState.CurrentPosition);
	}
}