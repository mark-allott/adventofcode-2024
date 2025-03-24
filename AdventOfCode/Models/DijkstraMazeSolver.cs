using System.Diagnostics;
using AdventOfCode.Enums;
using AdventOfCode.Extensions;
using AdventOfCode.Interfaces;

namespace AdventOfCode.Models;

internal class DijkstraMazeSolver
{
	#region Fields

	private readonly MazeGrid _maze;

	private DijkstraNode _startNode = null!;

	private DijkstraNode _endNode = null!;

	private List<DijkstraNode> _mazeNodes = null!;

	#endregion

	#region Ctor

	/// <summary>
	/// ctor
	/// </summary>
	/// <param name="mazeGrid">The maze to be solved, represented by a populated <see cref="MazeGrid"/> object
	public DijkstraMazeSolver(MazeGrid mazeGrid)
	{
		ArgumentNullException.ThrowIfNull(mazeGrid, nameof(mazeGrid));
		_maze = mazeGrid;
	}

	#endregion

	#region Methods

	/// <summary>
	/// From the supplied maze in the constructor, locate all unvisited nodes in the maze
	/// </summary>
	/// <returns>All unvisited nodes, initialises the start node with distance zero and sets the end location, if known</returns>
	private List<DijkstraNode> GetUnvisitedCells()
	{
		(_mazeNodes, _startNode, _endNode) = _maze.GetMazeNodes<DijkstraNode>();
		return new List<DijkstraNode>(_mazeNodes);
	}

	/// <summary>
	/// Maze solving method, requires the initial direction of travel for the start node
	/// </summary>
	/// <param name="initialDirection">A <see cref="DirectionOfTravel"/> value for the start node</param>
	/// <returns>The minimum distance value for the solution</returns>
	public int Solve(DirectionOfTravel initialDirection, List<DijkstraNode> unvisited, IDijkstraDistanceStrategy<MazeMovement> distanceStrategy)
	{
		unvisited ??= GetUnvisitedCells();
		var start = unvisited.FirstOrDefault(q => q.Distance == 0);

		//	If no start node located, then we're not solving
		if (start is null)
			return int.MaxValue;

		//	Is start the same as _startNode?
		Debug.Assert(start == _startNode);

		//	Set the initial direction of travel from the start node
		start.Direction = initialDirection;

		return Solve(unvisited, distanceStrategy);
	}

	/// <summary>
	/// Solves the maze using the shortest path found, given the nodes in
	/// <paramref name="unvisited"/>, applying distance units to the nodes
	/// yielded by <paramref name="distanceStrategy"/>
	/// </summary>
	/// <param name="unvisited">A list of unvisited nodes in the maze</param>
	/// <param name="distanceStrategy">The strategy used to calculate the distance units between nodes</param>
	/// <returns>The shortest path distance between start and end nodes</returns>
	/// <remarks>This can be called to provide alternate solutions by modifying
	/// the underlying nodes as visited/unvisited and blocking certain paths by
	/// setting distances to maximum</remarks>
	private int Solve(List<DijkstraNode> unvisited, IDijkstraDistanceStrategy<MazeMovement> distanceStrategy)
	{
		if (unvisited is null || unvisited.Count == 0)
			return int.MaxValue;

		//	Loop whilst we have unvisited nodes
		while (unvisited.Count > 0)
		{
			//	Take the minimum distance entry from the list of unvisited nodes
			var current = unvisited
				//	Distance of int.MaxValue is unreachable
				.Where(q => q.Distance != int.MaxValue)
				//	Don't try to solve for the end location
				.Where(q => !q.Location.Equals(_endNode))
				.OrderBy(o => o.Distance)
				.FirstOrDefault();

			//	All possible visits have been made
			if (current is null)
				break;

			//	Remove the current cell from the unvisited list
			unvisited.Remove(current);
			//	Apply changes to the neighbours
			current.UpdateNeighbours(_maze, unvisited, distanceStrategy);
		}

		return _endNode?.Distance ?? int.MaxValue;
	}

	#endregion
}