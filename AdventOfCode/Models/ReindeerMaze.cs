using AdventOfCode.Comparers;
using AdventOfCode.Enums;
using AdventOfCode.Extensions;

namespace AdventOfCode.Models;

internal class ReindeerMaze
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

	/// <summary>
	/// Public accessor to the start location coords (returns a cloned version so no hacking the location outside of the maze)
	/// </summary>
	public Coordinate StartLocation => _startLocation.DeepCopy();

	/// <summary>
	/// Public accessor to the end location coords (returns a cloned version so no hacking the location outside of the maze)
	/// </summary>
	public Coordinate EndLocation => _endLocation.DeepCopy();

	/// <summary>
	/// Public accessor for the bounds of the maze (returns a cloned version so no hacking the size outside of the maze)
	/// </summary>
	public Coordinate Bounds => _bounds.DeepCopy();

	#endregion

	#region ctor

	//	Relies on default ctor - no special case needed

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

		//	Load cell details into the gird
		for (var y = 0; y < data.Count; y++)
			for (var x = 0; x < data[y].Length; x++)
			{
				var cell = data[y][x].ToReindeerMazeCellType();
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

	public int DijkstraSolver()
	{
		var distanceStrategy = new ReindeerMazeDistanceStrategy();
		var solver = new DijkstraMazeSolver(_maze);
		return solver.Solve(DirectionOfTravel.East, null!, distanceStrategy);
	}

	public int DijkstraPathSolver()
	{
		var distanceStrategy = new ReindeerMazeDistanceStrategy();
		var solver = new DijkstraMazeSolver(_maze);

		var paths = solver.SolveMultipleBestPaths(DirectionOfTravel.East, distanceStrategy);
		var locations = paths.SelectMany(m => m.PathCoordinates)
			.DistinctBy(d => d, new GraphCoordinateEqualityComparer())
			.ToList();
		return locations.Count;
	}

	#endregion

	#endregion
}