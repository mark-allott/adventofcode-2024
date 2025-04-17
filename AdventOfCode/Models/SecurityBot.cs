using System.Numerics;

namespace AdventOfCode.Models;

internal class SecurityBot
{
	#region Properties

	/// <summary>
	/// Holds the starting position of the bot
	/// </summary>
	public Vector2 StartPosition { get; }

	/// <summary>
	/// Holds the velocity at which the bot travels
	/// </summary>
	public Vector2 Velocity { get; }

	/// <summary>
	/// Holds the location the bot is currently occupying
	/// </summary>
	public Vector2 CurrentPosition { get; private set; }

	/// <summary>
	/// The grid on which the bot patrols
	/// </summary>
	private SecurityBotGrid _grid = null!;

	/// <summary>
	/// Holds the number of moves the bot has made
	/// </summary>
	public int MovesMade { get; private set; } = 0;

	/// <summary>
	/// Returns the quadrant number the bot is currently in, based on the rules specified
	/// </summary>
	public int Quadrant => GetQuadrant();

	#endregion

	#region Ctor

	public SecurityBot(int x, int y, int dx, int dy, SecurityBotGrid grid)
	{
		StartPosition = new Vector2(x, y);
		CurrentPosition = StartPosition;
		Velocity = new Vector2(dx, dy);
		_grid = grid;
	}

	#endregion

	#region Methods

	/// <summary>
	/// Moves the bot once on the grid
	/// </summary>
	public void Move()
	{
		var newLocation = CurrentPosition + Velocity;

		//	If moved outside the bounds of the grid, "teleport" to the wrap-around location
		if (newLocation.X < 0)
			newLocation.X += _grid.GridBounds.X;
		if (newLocation.X >= _grid.GridBounds.X)
			newLocation.X -= _grid.GridBounds.X;
		if (newLocation.Y < 0)
			newLocation.Y += _grid.GridBounds.Y;
		if (newLocation.Y >= _grid.GridBounds.Y)
			newLocation.Y -= _grid.GridBounds.Y;

		//	Increment move counter and update current position
		MovesMade++;
		CurrentPosition = newLocation;
	}

	public void MoveFor(int count = 1)
	{
		var newLocation = CurrentPosition + (count * Velocity);

		//	If moved outside the bounds of the grid, "teleport" to the wrap-around location
		while (newLocation.X < 0)
			newLocation.X += _grid.GridBounds.X;
		while (newLocation.X >= _grid.GridBounds.X)
			newLocation.X -= _grid.GridBounds.X;
		while (newLocation.Y < 0)
			newLocation.Y += _grid.GridBounds.Y;
		while (newLocation.Y >= _grid.GridBounds.Y)
			newLocation.Y -= _grid.GridBounds.Y;

		//	Increment move counter and update current position
		MovesMade += count;
		CurrentPosition = newLocation;
	}

	/// <summary>
	/// Returns the quadrant number the bot is currently located in
	/// </summary>
	/// <returns>The quadrant number, or zero if in no quadrant</returns>
	public int GetQuadrant()
	{
		//	Assumes that the grid is an odd number of columns / rows
		var halfX = (int)_grid.GridBounds.X / 2;
		var halfY = (int)_grid.GridBounds.Y / 2;

		//	If less than halfX, in the left-hand side of the grid
		if (CurrentPosition.X < halfX)
			//	If less than halfY, then in the upper quadrant
			return CurrentPosition.Y < halfY
				? 1
				//	If more than halfY, then in lower quadrant
				: CurrentPosition.Y > halfY
					? 3
					: 0;    //	In no-mans land

		//	If more than halfX, then in the right-hand side
		if (CurrentPosition.X > halfX)
			//	If less than halfY, then in the upper quadrant
			return CurrentPosition.Y < halfY
				? 2
				//	If more than halfY, then in lower quadrant
				: CurrentPosition.Y > halfY
					? 4
					: 0;    //	In no-mans land

		//	Probably right in the middle
		return 0;
	}

	/// <summary>
	/// Resets the bot to the initial position
	/// </summary>
	public void Reset()
	{
		MovesMade = 0;
		CurrentPosition = StartPosition;
	}

	#endregion
}