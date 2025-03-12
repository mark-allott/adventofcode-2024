using AdventOfCode.Enums;

namespace AdventOfCode.Models;

/// <summary>
/// Internal class to represent a movement by the reindeer
/// </summary>
internal class ReindeerMove
{
	/// <summary>
	/// Holds the location at the end of the move
	/// </summary>
	public MapCoord Location { get; } = null!;

	/// <summary>
	/// Initial value is not known and should be only used at the start position
	/// </summary>
	public DirectionOfTravel Direction { get; } = DirectionOfTravel.Unknown;

	/// <summary>
	/// The movement being executed
	/// </summary>
	public ReindeerMazeMove MazeMove { get; } = ReindeerMazeMove.Unknown;

	/// <summary>
	/// ctor
	/// </summary>
	/// <param name="location">The coords for the movement</param>
	/// <param name="direction">The direction of travel</param>
	/// <param name="mazeMove">The type of move being made</param>
	public ReindeerMove(MapCoord location, DirectionOfTravel direction, ReindeerMazeMove mazeMove)
	{
		ArgumentNullException.ThrowIfNull(location, nameof(location));
		Location = location;
		Direction = direction;
		MazeMove = mazeMove;
	}

	/// <summary>
	/// Debug friendly override to display the movement
	/// </summary>
	/// <returns>A text representation of the move</returns>
	public override string ToString()
	{
		return $"{Location} => {Direction}/{MazeMove}";
	}
}