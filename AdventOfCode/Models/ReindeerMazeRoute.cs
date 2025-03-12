using System.Text;
using AdventOfCode.Enums;

namespace AdventOfCode.Models;

/// <summary>
/// internal class representing a route through the maze
/// </summary>
internal class ReindeerMazeRoute
{
	#region Fields

	/// <summary>
	/// Debug counter - increments each time a route is generated
	/// </summary>
	private static int _counter = 1;

	/// <summary>
	/// Holds the moves made by the reindeer when attempting to solve the maze
	/// </summary>
	private readonly List<ReindeerMove> _moves = new List<ReindeerMove>();

	/// <summary>
	/// Holds the start position of the reindeer in the maze
	/// </summary>
	private ReindeerMove _startingMove = null!;

	/// <summary>
	/// Holds the location of the end-point of the maze
	/// </summary>
	private MapCoord _endLocation = null!;

	/// <summary>
	/// Holds the boundary of the maze
	/// </summary>
	private MapCoord _bounds = null!;

	#endregion

	#region Properties

	/// <summary>
	/// Property accessor to represent whether this route solves the maze
	/// </summary>
	public bool CompletesMaze { get; private set; } = false;

	/// <summary>
	/// Property accessor yielding the last move made by the reindeer
	/// </summary>
	public ReindeerMove LastMove => _moves.Count > 0
		? _moves.LastOrDefault()!
		: _startingMove;

	/// <summary>
	/// Property accessor giving read-only access to the list of moves made
	/// </summary>
	public List<ReindeerMove> Movements => _moves.AsReadOnly().ToList();

	/// <summary>
	/// Holds the counter for the route - allows sorting in order of routes discovered
	/// </summary>
	public int RouteOrder { get; }

	/// <summary>
	/// Returns the score for the route, based on one point per movement and 1000 points per turn
	/// </summary>
	/// <remarks>The score is always zero if the maze was not completed</remarks>
	public int Score
	{
		get
		{
			if (!CompletesMaze)
				return 0;
			return _moves.Count(q => q.MazeMove == ReindeerMazeMove.Forward) + 1000 * _moves.Count(q => q.MazeMove == ReindeerMazeMove.TurnLeft || q.MazeMove == ReindeerMazeMove.TurnRight);
		}
	}

	/// <summary>
	/// Property accessor to indicate whether the reindeer is still walking in the maze, attempting to find a solution
	/// </summary>
	public bool Walking { get; set; } = true;

	#endregion

	#region ctor

	/// <summary>
	/// Private constructor to allow for duplication of previous routes
	/// </summary>
	private ReindeerMazeRoute(ReindeerMazeRoute route)
	{
		_startingMove = route._startingMove;
		_endLocation = route._endLocation;
		_moves.AddRange(route._moves);
		_bounds = route._bounds;
		RouteOrder = _counter++;
	}

	/// <summary>
	/// Standard constructor, initialises the route being taken from the supplied <paramref name="maze"/>
	/// </summary>
	/// <param name="maze">The maze to be travelled</param>
	public ReindeerMazeRoute(ReindeerMaze maze)
	{
		//	Initialise the route with the reindeer in the start position, facing East (as per brief on website)
		_startingMove = new ReindeerMove(maze.StartLocation, DirectionOfTravel.East, ReindeerMazeMove.Forward);
		_endLocation = maze.EndLocation;
		_bounds = maze.Bounds;
		RouteOrder = _counter++;
	}

	#endregion

	/// <summary>
	/// Duplicates the route taken so far to allow a reindeer to try an alternate direction
	/// </summary>
	/// <returns>The route, but duplicated</returns>
	public ReindeerMazeRoute Duplicate()
	{
		return new ReindeerMazeRoute(this);
	}

	/// <summary>
	/// Updates the <see cref="Walking"/> property, then if still able to
	/// walk, adds the move to the route. Finally, checks to see if the
	/// location the reindeer is now at is the end-point of the maze
	/// </summary>
	/// <param name="move">The movement being made</param>
	public void AddMovement(ReindeerMove move)
	{
		ArgumentNullException.ThrowIfNull(move, nameof(move));
		ArgumentOutOfRangeException.ThrowIfEqual((int)move.MazeMove, (int)ReindeerMazeMove.Unknown, nameof(move));

		//	If we're already halted, quit now
		if (!Walking)
			return;

		//	Only able to continue walking if the reindeer has not been in that location before.
		//	If a duplicate location is seen, it would suggest the reindeer is walking in circles
		var previousVisits = _moves.Where(q => q.Location.Equals(move.Location)).ToList();
		var canMove = true;
		if (previousVisits.Count > 0)
		{
			//	If any previous visits and walking forwards, stop walking the route
			canMove = !(move.MazeMove == ReindeerMazeMove.Forward);
			//	We can move if there's only one previous visit and we're now turning left/right
			canMove = canMove &&
					previousVisits.Count == 1 &&
					previousVisits[0].MazeMove == ReindeerMazeMove.Forward &&
					(move.MazeMove == ReindeerMazeMove.TurnLeft || move.MazeMove == ReindeerMazeMove.TurnRight);
		}
		Walking = Walking && canMove;

		//	If we are continuing to walk the maze, add the move to the route
		if (Walking)
		{
			_moves.Add(move);
		}

		//	If the reindeer reaches the endpoint, the maze completes
		CompletesMaze = move.Location.Equals(_endLocation);
		Walking = Walking && !CompletesMaze;
	}

	/// <summary>
	/// Overrides base implementation to yield debug-friendly representation of the route
	/// </summary>
	/// <returns>The route counter, walking/completed indicators and last move made</returns>
	public override string ToString()
	{
		return $"{RouteOrder}:{(Walking ? "W" : "H")}{(CompletesMaze ? "C" : "N")}:{LastMove}";
	}

	/// <summary>
	/// Allows printing of the route taken in the maze
	/// </summary>
	/// <returns>Returns the route number and steps taken during the route</returns>
	public string Print()
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Route: {RouteOrder}");
		foreach (var move in _moves)
			sb.AppendLine($"{move}");
		if (CompletesMaze)
			sb.AppendLine("Completed");
		return sb.ToString();
	}
}