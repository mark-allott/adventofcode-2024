using AdventOfCode.Enums;
using AdventOfCode.Models;

namespace AdventOfCode.Extensions;

internal static class CoordinateExtensions
{
	/// <summary>
	/// Extension method to add an offset to an existing coordinate
	/// </summary>
	/// <param name="coord">The current coordinate</param>
	/// <param name="rowOffset">The number of rows to offset by (Y-coords)</param>
	/// <param name="columnOffset">The number of columns to offset by (X-coords)</param>
	/// <returns>The updated coordinate</returns>
	public static Coordinate OffsetBy(this Coordinate coord, int rowOffset, int columnOffset)
	{
		return new Coordinate(coord.Y + rowOffset, coord.X + columnOffset);
	}

	/// <summary>
	/// Extension method to apply an offset to <paramref name="coord"/>
	/// </summary>
	/// <param name="coord">The current coordinate</param>
	/// <param name="offset">The offset to be applied</param>
	/// <returns>The new coordinate</returns>
	public static Coordinate OffsetBy(this Coordinate coord, Coordinate offset)
	{
		return new Coordinate(coord.Y + offset.Y, coord.X + offset.X);
	}

	/// <summary>
	/// Extension method to return the possible moves from the
	/// <paramref name="current"/> position, moving in <paramref name="direction"/>
	/// of travel within the specified <paramref name="maze"/>
	/// </summary>
	/// <param name="current">The current position within the maze (expressed as a <see cref="Coordinate"/>)</param>
	/// <param name="direction">The direction (see <see cref="DirectionOfTravel"/>) the reindeer is travelling</param>
	/// <param name="maze">The maze in which the reindeer as trying to get from start to end position</param>
	/// <returns>The list of possible moves</returns>
	public static List<(MazeMovement action, Coordinate location)> PossibleMoves(this Coordinate current, DirectionOfTravel direction, MazeGrid maze)
	{
		ArgumentNullException.ThrowIfNull(current, nameof(current));
		ArgumentOutOfRangeException.ThrowIfEqual((int)direction, (int)DirectionOfTravel.Unknown, nameof(direction));
		ArgumentNullException.ThrowIfNull(maze, nameof(maze));

		var moves = new List<(MazeMovement, Coordinate)>();

		var movesToCheck = new List<MazeMovement>() { MazeMovement.GoForward, MazeMovement.TurnLeft, MazeMovement.TurnRight };

		foreach (var moveToCheck in movesToCheck)
		{
			var travelDirection = moveToCheck.ToDirectionOfTravel(direction);
			var offset = travelDirection.ToMapCoordOffset();
			var newCoord = current.OffsetBy(offset.yOffset, offset.xOffset);
			var nextNode = maze[newCoord];

			if (nextNode != MazeCellType.Wall)
				moves.Add((moveToCheck, newCoord));
		}
		return moves;
	}
}