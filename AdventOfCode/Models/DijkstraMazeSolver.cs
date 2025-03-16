using AdventOfCode.Enums;
using AdventOfCode.Extensions;

namespace AdventOfCode.Models;

internal class DijkstraMazeSolver
{
	#region Fields
	private readonly MazeGrid _maze;

	private readonly MapCoord _mazeBounds;

	private MapCoord _endLocation = null!;

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
		_mazeBounds = _maze.Bounds;
	}

	#endregion

	#region Methods

	/// <summary>
	/// From the supplied maze in the constructor, locate all unvisited nodes in the maze
	/// </summary>
	/// <returns>All unvisited nodes, initialises the start node with distance zero and sets the end location, if known</returns>
	private List<DijkstraNode> GetUnvisitedCells()
	{
		var nodes = new List<DijkstraNode>();

		for (var y = 0; y < _mazeBounds.Y; y++)
			for (var x = 0; x < _mazeBounds.X; x++)
			{
				var location = new MapCoord(y, x);
				switch (_maze[location])
				{
					case MazeCellType.Empty:
						nodes.Add(new DijkstraNode(location));
						break;
					case MazeCellType.End:
						nodes.Add(new DijkstraNode(location));
						_endLocation = location;
						break;
					case MazeCellType.Start:
						nodes.Add(new DijkstraNode(location) { Distance = 0 });
						break;
					default:
						break;
				}
			}
		return nodes;
	}

	/// <summary>
	/// Maze solving method, requires the initial direction of travel for the start node
	/// </summary>
	/// <param name="initialDirection">A <see cref="DirectionOfTravel"/> value for the start node</param>
	/// <returns>The minimum distance value for the solution</returns>
	public int Solve(DirectionOfTravel initialDirection)
	{
		var unvisited = GetUnvisitedCells();
		var start = unvisited.FirstOrDefault(q => q.Distance == 0);

		//	If no start node located, then we're not solving
		if (start is null)
			return int.MaxValue;

		//	Set the initial direction of travel from the start node
		start.Direction = initialDirection;

		//	Loop whilst we have unvisited nodes
		while (unvisited.Count > 0)
		{
			//	Take the minimum distance entry from the list of unvisited nodes
			var current = unvisited
				//	Distance of int.MaxValue is unreachable
				.Where(q => q.Distance != int.MaxValue)
				//	Don't try to solve for the end location
				.Where(q => !q.Location.Equals(_endLocation))
				.OrderBy(o => o.Distance)
				.FirstOrDefault();

			//	All possible visits have been made
			if (current is null)
				break;

			//	Remove the current cell from the unvisited list
			unvisited.Remove(current);
			//	Apply changes to the neighbours
			current.UpdateNeighbours(_maze, unvisited);
		}

		var endpoint = unvisited.FirstOrDefault(q => q.Location.Equals(_endLocation));
		return endpoint?.Distance ?? int.MaxValue;
	}

	#endregion
}