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

			//	Only try to check a coordinate that is within bounds of the maze
			if (!newCoord.InBounds(maze.Bounds))
				continue;

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

	/// <summary>
	/// Returns a list of <see cref="DijkstraNode"/> neighbours of <paramref name="node"/>, that are within the permitted list of <paramref name="mazeNodes"/>
	/// </summary>
	/// <param name="node">The node to find neighbours for</param>
	/// <param name="range">The range at which a neighbour must appear</param>
	/// <param name="mazeNodes">The list of permitted nodes in the maze</param>
	/// <returns>The neighouring nodes</returns>
	public static List<DijkstraNode> NeighboursAtRange(this DijkstraNode node, int range, List<DijkstraNode> mazeNodes)
	{
		ArgumentNullException.ThrowIfNull(node, nameof(node));
		ArgumentNullException.ThrowIfNull(mazeNodes, nameof(mazeNodes));

		//	Always convert to +ve range first
		range = Math.Abs(range);

		//	Create a list of coordinates where neighbours are required to be located
		var nodeCoords = new List<Coordinate>()
		{
			node.Location.OffsetBy(0, range),
			node.Location.OffsetBy(0, -range),
			node.Location.OffsetBy(range ,0),
			node.Location.OffsetBy(-range, 0)
		};

		//	return the list of nodes which occupy the desired locations
		return mazeNodes.Where(n => nodeCoords.Contains(n.Location))
			.ToList();
	}

	/// <summary>
	/// Returns a list of nodes from <paramref name="allowedNodes"/> that are
	/// within the <paramref name="range"/> of <paramref name="node"/>, using
	/// the <paramref name="rangeStrategy"/> to calculate the range between nodes
	/// </summary>
	/// <param name="node">The node requesting neighbours</param>
	/// <param name="range">The range within which to return neighbours</param>
	/// <param name="rangeStrategy">The strategy to use to determine range between two nodes</param>
	/// <param name="allowedNodes">The nodes from which neighbours shall be chosen</param>
	/// <returns>Nodes that are neighbours for <paramref name="node"/>, within the specified <paramref name="range"/></returns>
	public static IEnumerable<DijkstraNode> NeighboursInRange(this DijkstraNode node, int range, IGraphCoordinateRangeStrategy rangeStrategy, List<DijkstraNode> allowedNodes)
	{
		ArgumentNullException.ThrowIfNull(node, nameof(node));
		ArgumentNullException.ThrowIfNull(rangeStrategy, nameof(rangeStrategy));
		ArgumentNullException.ThrowIfNull(allowedNodes, nameof(allowedNodes));

		//	Always convert to +ve range first
		range = Math.Abs(range);

		return allowedNodes
			.Where(n => !node.Equals(n))
			.Where(n => rangeStrategy.Range(node.Location, n.Location) <= range)
			.ToList();
	}
}