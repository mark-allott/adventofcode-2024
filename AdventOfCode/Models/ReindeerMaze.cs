using System.Text;
using AdventOfCode.Enums;
using AdventOfCode.Extensions;

namespace AdventOfCode.Models;

internal class ReindeerMaze
{
	#region Fields

	/// <summary>
	/// Holds the maze cells
	/// </summary>
	private ReindeerMazeCellType[,] _maze = null!;

	/// <summary>
	/// Holds the upper bounds of the maze coordinates
	/// </summary>
	private MapCoord _bounds = null!;

	/// <summary>
	/// Holds the location of the start point within the maze
	/// </summary>
	private MapCoord _startLocation = null!;

	/// <summary>
	/// Holds the location of the end point in the maze
	/// </summary>
	private MapCoord _endLocation = null!;

	#endregion

	#region Properties

	/// <summary>
	/// Indexer accessor, using separate X and Y coords
	/// </summary>
	/// <param name="x">The x-Coord of the cell</param>
	/// <param name="y">The y-Coord of the cell</param>
	/// <returns>The type occupying the cell</returns>
	public ReindeerMazeCellType this[int x, int y]
	{
		get
		{
			ValidateMazeInputs(x, y);
			return _maze[y, x];
		}
		private set
		{
			ValidateMazeInputs(x, y);
			_maze[y, x] = value;
		}
	}

	/// <summary>
	/// Alternate Indexer accessor, using a <see cref="MapCoord"/> to get/set the cell
	/// </summary>
	/// <param name="coord">The location to access</param>
	/// <returns>The type occupying the cell</returns>
	public ReindeerMazeCellType this[MapCoord coord]
	{
		get => this[coord.X, coord.Y];
		private set => this[coord.X, coord.Y] = value;
	}

	/// <summary>
	/// Public accessor to the start location coords (returns a cloned version so no hacking the location outside of the maze)
	/// </summary>
	public MapCoord StartLocation => _startLocation.DeepCopy();

	/// <summary>
	/// Public accessor to the end location coords (returns a cloned version so no hacking the location outside of the maze)
	/// </summary>
	public MapCoord EndLocation => _endLocation.DeepCopy();

	/// <summary>
	/// Public accessor for the bounds of the maze (returns a cloned version so no hacking the size outside of the maze)
	/// </summary>
	public MapCoord Bounds => _bounds.DeepCopy();

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
		var sb = new StringBuilder();
		for (var y = 0; y < _bounds.Y; y++)
		{
			for (var x = 0; x < _bounds.X; x++)
				sb.Append(this[x, y].ToCharacter());
			sb.AppendLine();
		}
		return sb.ToString();
	}

	#endregion

	#region Methods

	/// <summary>
	/// Common validation of <paramref name="x"/> and <paramref name="y"/>
	/// values as well as making sure the maze if initialised etc.
	/// </summary>
	/// <param name="x">The maze x-Coord</param>
	/// <param name="y">The maze y-Coord</param>
	/// <exception cref="ArgumentNullException"></exception>
	private void ValidateMazeInputs(int x, int y)
	{
		if (_maze is null)
			throw new ArgumentNullException(nameof(_maze), "Maze is uninitialised");
		if (_bounds is null)
			throw new ArgumentNullException(nameof(_bounds), "Maze is not initialised correctly");
		ArgumentOutOfRangeException.ThrowIfNegative(x, nameof(x));
		ArgumentOutOfRangeException.ThrowIfNegative(y, nameof(y));
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(x, _bounds.X, nameof(x));
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(y, _bounds.Y, nameof(y));
	}

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
		_bounds = new MapCoord(data.Count, data[0].Length);
		_maze = new ReindeerMazeCellType[_bounds.Y, _bounds.X];

		//	Load cell details into the gird
		for (var y = 0; y < data.Count; y++)
			for (var x = 0; x < data[y].Length; x++)
			{
				var cell = data[y][x].ToReindeerMazeCellType();
				this[x, y] = cell;
				if (cell == ReindeerMazeCellType.Start)
					_startLocation = new MapCoord(y, x);
				if (cell == ReindeerMazeCellType.End)
					_endLocation = new MapCoord(y, x);
			}

		//	When leaving there MUST be a start and end location specified
		ArgumentNullException.ThrowIfNull(_startLocation, nameof(_startLocation));
		ArgumentNullException.ThrowIfNull(_endLocation, nameof(_endLocation));
	}

	public List<ReindeerMazeRoute> FindRoutes()
	{
		var routes = new List<ReindeerMazeRoute>()
		{
			new ReindeerMazeRoute(this)
		};

		while (routes.Any(r => r.Walking))
		{
			var walkingRoutes = routes.Where(r => r.Walking)
				.OrderBy(o => o.RouteOrder)
				.ToList();

			Console.WriteLine($"Total Routes: {routes.Count}; Walking Count: {walkingRoutes.Count}; Completed Count: {routes.Count(r => r.CompletesMaze)}");
			//	Container for holding newly discovered routes
			var newRoutes = new List<ReindeerMazeRoute>();

			//	Loop around each route still walking
			foreach (var route in walkingRoutes)
			{
				//	Whilst this route can still move, loop until it can't
				while (route.Walking)
				{
					var lastMovement = route.LastMove;

					//	Calculate possible moves from the last location
					var moves = lastMovement.Location.ReindeerMoves(lastMovement.Direction, this);
					List<ReindeerMove> newMoves;

					switch (moves.Count)
					{
						//	Blocked on 3 sides, so walked into a dead-end
						case 0:
							route.Walking = false;
							break;

						//	One move only, so execute the move
						case 1:
							//	If the only move to make is a turn, this will result in 2 moves; the turn and a forward
							newMoves = GetReindeerMoves(lastMovement, moves[0]);
							newMoves.ForEach(m => route.AddMovement(m));
							break;

						//	More than one move available.
						default:
							//	Make the first movement the "default" route to
							//	be followed; additonal moves will become alternate
							//	routes that will be followed once the default route
							//	completes - either by finding the end point or it
							//	becomes blocked, or starts going in circles

							//	Copy the route taken so far into new route(s)
							var newRouteDirections = Enumerable.Range(1, moves.Count - 1)
								.Select(s => new { Index = s, Route = route.Duplicate() })
								.ToList();

							//	Execute the move on the "default" route
							newMoves = GetReindeerMoves(lastMovement, moves[0]);
							newMoves.ForEach(m => route.AddMovement(m));

							//	On the new routes, execute the moves that were located
							newRouteDirections.ForEach(i =>
							{
								newMoves = GetReindeerMoves(lastMovement, moves[i.Index]);
								newMoves.ForEach(m => i.Route.AddMovement(m));
							});

							//	Add the newly discovered alternate routes to the new routes container
							newRoutes.AddRange(newRouteDirections.Select(i => i.Route));
							break;
					}
				}
			}
			//	The route has now completed - either found the end-point of the
			//	maze, got stuck in a dead-end, or started walking in circles.
			//	Add new routes found to the list of all routes
			routes.AddRange(newRoutes);
		}
		return routes;
	}

	/// <summary>
	/// Get the moves the reindeer will perform based on the <paramref name="lastMove"/> and <paramref name="newMove"/>
	/// </summary>
	/// <param name="lastMove">The last movement the reindeer made</param>
	/// <param name="newMove">The move the reindeer is about to make</param>
	/// <returns>One or more moves the reindeer makes to the new location</returns>
	private List<ReindeerMove> GetReindeerMoves(ReindeerMove lastMove, (ReindeerMazeMove movement, MapCoord location) newMove)
	{
		//	A movement forwards is simply one move in the same direction as previously made
		if (newMove.movement == ReindeerMazeMove.Forward)
		{
			return new List<ReindeerMove>()
			{
				new ReindeerMove(newMove.location, newMove.movement.ToDirectionOfTravel(lastMove.Direction), ReindeerMazeMove.Forward)
			};
		}

		//	Movements involving turns have two components:
		//		1 - the reindeer stays in the same location and rotates left/right
		//		2 - the reindeer moves forward into the new location
		var turningMove = new ReindeerMove(lastMove.Location, lastMove.Direction, newMove.movement);
		var forwardMove = new ReindeerMove(newMove.location, turningMove.MazeMove.ToDirectionOfTravel(turningMove.Direction), ReindeerMazeMove.Forward);
		return new List<ReindeerMove>()
		{
			turningMove,
			forwardMove
		};
	}

	#endregion
}