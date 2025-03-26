using AdventOfCode.Enums;
using AdventOfCode.Interfaces;
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
	/// <param name="distanceStrategy">The strategy used to calculate the new distance from the current node</param>
	public static void UpdateNeighbours(this DijkstraNode node, MazeGrid maze, List<DijkstraNode> unvisitedNodes, IDijkstraDistanceStrategy<MazeMovement> distanceStrategy)
	{
		ArgumentNullException.ThrowIfNull(node, nameof(node));
		ArgumentNullException.ThrowIfNull(maze, nameof(maze));
		ArgumentNullException.ThrowIfNull(unvisitedNodes, nameof(unvisitedNodes));
		ArgumentNullException.ThrowIfNull(distanceStrategy, nameof(distanceStrategy));

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
			var newDistance = node.Distance + distanceStrategy.GetDistance(moveToCheck);

			//	If the new distance is less than the current value, update the neighbour
			if (newDistance < neighbour.Distance)
			{
				neighbour.Distance = newDistance;
				neighbour.Direction = travelDirection;
			}
		}
	}

	/// <summary>
	/// Returns a list of neighbouring cells for <paramref name="node"/>, in a <paramref name="maze"/> based on the list of nodes in <paramref name="unvisitedNodes"/>
	/// </summary>
	/// <param name="node">The current node</param>
	/// <param name="maze">The overall maze</param>
	/// <param name="unvisitedNodes">A list of nodes that have not been visited yet</param>
	/// <returns>A list of nodes that are neighbours of <paramref name="node"/> and have not been visited previously</returns>
	public static List<DijkstraNode> GetNeighbours(this DijkstraNode node, MazeGrid maze, List<DijkstraNode> unvisitedNodes)
	{
		ArgumentNullException.ThrowIfNull(node, nameof(node));
		ArgumentNullException.ThrowIfNull(maze, nameof(maze));
		ArgumentNullException.ThrowIfNull(unvisitedNodes, nameof(unvisitedNodes));

		var neighbours = new List<DijkstraNode>();

		var movesToCheck = new List<MazeMovement>() { MazeMovement.GoForward, MazeMovement.TurnLeft, MazeMovement.TurnRight };

		foreach (var moveToCheck in movesToCheck)
		{
			var travelDirection = moveToCheck.ToDirectionOfTravel(node.Direction);
			var offset = travelDirection.ToMapCoordOffset();
			var newCoord = node.Location.OffsetBy(offset.yOffset, offset.xOffset);

			//	Find our neighbour from the unvisited nodes
			var neighbour = unvisitedNodes.FirstOrDefault(q => q.Location.Equals(newCoord));

			//	Check only unvisited nodes, if the neighbour we want is there, return as part of neighbours
			if (neighbour is not null)
				neighbours.Add(neighbour);
		}
		return neighbours;
	}
}