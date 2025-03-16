using AdventOfCode.Enums;
using AdventOfCode.Models;

namespace AdventOfCode.Extensions;

internal static class DijkstraNodeExtensions
{
	/// <summary>
	/// Extension method to work what neighbouring nodes there are from the
	/// current <paramref name="node"/> and update them accordingly, based on
	/// values held in <paramref name="unvisitedNodes"/>
	/// </summary>
	/// <param name="node">The current node being examined</param>
	/// <param name="maze">The maze being traversed</param>
	/// <param name="unvisitedNodes">The list of unvisited nodes</param>
	public static void UpdateNeighbours(this DijkstraNode node, MazeGrid maze, List<DijkstraNode> unvisitedNodes)
	{
		ArgumentNullException.ThrowIfNull(node, nameof(node));
		ArgumentNullException.ThrowIfNull(maze, nameof(maze));
		ArgumentNullException.ThrowIfNull(unvisitedNodes, nameof(unvisitedNodes));

		var movesToCheck = new List<MazeMovement>() { MazeMovement.GoForward, MazeMovement.TurnLeft, MazeMovement.TurnRight };

		foreach (var moveToCheck in movesToCheck)
		{
			var travelDirection = moveToCheck.ToDirectionOfTravel(node.Direction);
			var offset = travelDirection.ToMapCoordOffset();
			var newCoord = node.Location.OffsetBy(offset.yOffset, offset.xOffset);
			var nextCell = maze[newCoord];
			//	if we encounter a wall, check the next move
			if (nextCell == MazeCellType.Wall)
				continue;

			//	Find our neighbour from the unvisited nodes
			var neighbour = unvisitedNodes.FirstOrDefault(q => q.Location.Equals(newCoord));
			//	As we check only unvisited nodes, if the neighbour we want isn't there, then it has already been visited
			if (neighbour is null)
				continue;

			//	Calculate the new distance value for the neighbour
			var newDistance = node.Distance + moveToCheck switch
			{
				MazeMovement.GoForward => 1,
				_ => 1001
			};

			//	If the new distance is less than the current value, update the neighbour
			if (newDistance < neighbour.Distance)
			{
				neighbour.Distance = newDistance;
				neighbour.Direction = travelDirection;
			}
		}
	}
}