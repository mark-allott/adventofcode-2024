using AdventOfCode.Comparers;
using AdventOfCode.Enums;
using AdventOfCode.Extensions;

namespace AdventOfCode.Models;

internal class RaceCondition
{
	#region Fields

	/// <summary>
	/// Holds the maze cells
	/// </summary>
	private MazeGrid _maze = null!;

	/// <summary>
	/// Holds the upper bounds of the maze coordinates
	/// </summary>
	private Coordinate _bounds = null!;

	/// <summary>
	/// Holds the location of the start point within the maze
	/// </summary>
	private Coordinate _startLocation = null!;

	/// <summary>
	/// Holds the location of the end point in the maze
	/// </summary>
	private Coordinate _endLocation = null!;

	#endregion

	#region Properties
	/// <summary>
	/// Indexer accessor, using separate X and Y coords
	/// </summary>
	/// <param name="x">The x-Coord of the cell</param>
	/// <param name="y">The y-Coord of the cell</param>
	/// <returns>The type occupying the cell</returns>
	public MazeCellType this[int x, int y]
	{
		get => _maze[x, y];
		private set => _maze[x, y] = value;
	}

	/// <summary>
	/// Alternate Indexer accessor, using a <see cref="Coordinate"/> to get/set the cell
	/// </summary>
	/// <param name="coord">The location to access</param>
	/// <returns>The type occupying the cell</returns>
	public MazeCellType this[Coordinate coord]
	{
		get => _maze[coord.X, coord.Y];
		private set => _maze[coord.X, coord.Y] = value;
	}

	#endregion

	#region Constructors
	#endregion

	#region Overrides

	/// <summary>
	/// Debug friendly override to display the maze grid as shown on the website
	/// </summary>
	/// <returns>A textual representation of the maze</returns>
	public override string ToString()
	{
		return _maze.ToString();
	}

	#endregion

	#region Methods

	/// <summary>
	/// Loads the maze from the specified <paramref name="input"/> values
	/// </summary>
	/// <param name="input">A textual representation of the maze</param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public void Load(IEnumerable<string> input)
	{
		ArgumentNullException.ThrowIfNull(nameof(input));
		var data = input.ToList();
		if (data.Any(l => l.Length != data[0].Length))
			throw new ArgumentOutOfRangeException(nameof(input), "Ragged maze areas are not supported");

		//	Set bounds and initialise maze grid
		_bounds = new Coordinate(data.Count, data[0].Length);
		_maze = new MazeGrid(_bounds);

		//	Load cell details into the grid
		for (var y = 0; y < data.Count; y++)
			for (var x = 0; x < data[y].Length; x++)
			{
				var cell = data[y][x].ToMazeCellType();
				this[x, y] = cell;
				if (cell == MazeCellType.Start)
					_startLocation = new Coordinate(y, x);
				if (cell == MazeCellType.End)
					_endLocation = new Coordinate(y, x);
			}

		//	When leaving there MUST be a start and end location specified
		ArgumentNullException.ThrowIfNull(_startLocation, nameof(_startLocation));
		ArgumentNullException.ThrowIfNull(_endLocation, nameof(_endLocation));
	}

	#region Dijkstra Algorithm

	/// <summary>
	/// Solves the maze using Dijkstra's algorithm
	/// </summary>
	/// <returns>The shortest path solution, or <see cref="int.MaxValue"/> if no solution</returns>
	public int DijkstraSolver()
	{
		var distanceStrategy = new RamRunDistanceStrategy();
		var solver = new DijkstraMazeSolver(_maze);
		return solver.Solve(DirectionOfTravel.East, null!, distanceStrategy);
	}

	/// <summary>
	/// Method to locate shortcuts in the maze and return the number found and how much savings they make
	/// </summary>
	/// <returns>A dictionary of savings found and how many alternate routes have the same saving</returns>
	public Dictionary<int, int> GetShortcuts()
	{
		//	Create the maze, get the nodes and work out the solution
		var distanceStrategy = new RamRunDistanceStrategy();
		var solver = new DijkstraMazeSolver(_maze);
		//	Solve using standard solution first to get all distances populated for nodes
		var bestStandardPath = solver.Solve(DirectionOfTravel.East, null!, distanceStrategy);
		var nodesInSolution = solver.GetSolutionNodes();

		var results = new Dictionary<int, int>();

		foreach (var node in nodesInSolution)
		{
			var neighbours = node.NeighboursAtRange(2, nodesInSolution);
			var shortcuts = neighbours
				.Where(n => n.Distance > 0)
				.Where(n => n.Distance - node.Distance > 2)
				.Select(s => new { StartNode = node, EndNode = s, Saving = s.Distance - node.Distance - 2 })
				.ToList();

			shortcuts.ForEach(s =>
			{
				if (!results.TryGetValue(s.Saving, out var count))
					results[s.Saving] = 0;
				results[s.Saving] += 1;
			});
		}
		return results;
	}

	#endregion

	#endregion
}