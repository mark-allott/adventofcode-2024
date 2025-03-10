using AdventOfCode.Enums;
using AdventOfCode.Models;

namespace AdventOfCode.Extensions;

internal static class MapCoordExtensions
{
	/// <summary>
	/// Extension method to add an offset to an existing coordinate
	/// </summary>
	/// <param name="coord">The current coordinate</param>
	/// <param name="rowOffset">The number of rows to offset by (Y-coords)</param>
	/// <param name="columnOffset">The number of columns to offset by (X-coords)</param>
	/// <returns>The updated coordinate</returns>
	public static MapCoord OffsetBy(this MapCoord coord, int rowOffset, int columnOffset)
	{
		return new MapCoord(coord.Y + rowOffset, coord.X + columnOffset);
	}

	/// <summary>
	/// Extension method to return the possible moves for the reindeer from
	/// <paramref name="current"/> position, moving in <paramref name="direction"/>
	/// of travel within the specified <paramref name="maze"/>
	/// </summary>
	/// <param name="current">The current position within the maze (expressed as a <see cref="MapCoord"/>)</param>
	/// <param name="direction">The direction (see <see cref="DirectionOfTravel"/>) the reindeer is travelling</param>
	/// <param name="maze">The maze in which the reindeer as trying to get from start to end position</param>
	/// <returns>The list of possible moves</returns>
	public static List<(ReindeerMazeMove, MapCoord)> ReindeerMoves(this MapCoord current, DirectionOfTravel direction, ReindeerMaze maze)
	{
		ArgumentNullException.ThrowIfNull(current, nameof(current));
		ArgumentOutOfRangeException.ThrowIfEqual((int)direction, (int)DirectionOfTravel.Unknown, nameof(direction));
		ArgumentNullException.ThrowIfNull(maze, nameof(maze));

		var moves = new List<(ReindeerMazeMove, MapCoord)>();

		var movesToCheck = new List<ReindeerMazeMove>() { ReindeerMazeMove.TurnLeft, ReindeerMazeMove.Forward, ReindeerMazeMove.TurnRight };

		foreach (var moveToCheck in movesToCheck)
		{
			var travelDirection = moveToCheck.ToDirectionOfTravel(direction);
			var offset = travelDirection.ToMapCoordOffset();
			var newCoord = current.OffsetBy(offset.yOffset, offset.xOffset);
			var nextCell = maze[newCoord];
			//	if we don't encounter a wall, return the move and the next possible location to move into
			if (nextCell != ReindeerMazeCellType.Wall)
				moves.Add((moveToCheck, newCoord));
		}
		return moves;
	}
}