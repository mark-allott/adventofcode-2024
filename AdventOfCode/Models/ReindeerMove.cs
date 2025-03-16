using AdventOfCode.Enums;

namespace AdventOfCode.Models;

/// <summary>
/// Internal class to represent a movement by the reindeer
/// </summary>
internal class ReindeerMove
{
	#region Fields

	/// <summary>
	/// Container for forward moves that can be made from this one
	/// </summary>
	private readonly ArrayByEnum<ReindeerMove, MazeMovement> _forwardMoves = new ArrayByEnum<ReindeerMove, MazeMovement>(MazeMovement.Unknown);

	/// <summary>
	/// Container for backward moves from this one
	/// </summary>
	private readonly ArrayByEnum<ReindeerMove, DirectionOfTravel> _backwardMoves = new ArrayByEnum<ReindeerMove, DirectionOfTravel>(DirectionOfTravel.Unknown);

	#endregion

	#region Properties

	/// <summary>
	/// Holds the location at the end of the move
	/// </summary>
	public MapCoord Location { get; } = null!;

	/// <summary>
	/// Initial value is not known and should be only used at the start position
	/// </summary>
	public DirectionOfTravel Direction { get; } = DirectionOfTravel.Unknown;

	/// <summary>
	/// The movement being executed
	/// </summary>
	public MazeMovement MazeMove { get; } = MazeMovement.Unknown;

	public IEnumerable<ReindeerMove> ForwardMoves => _forwardMoves.Where(k => k is not null).ToList();

	public IEnumerable<ReindeerMove> BackwardMoves => _backwardMoves.Where(k => k is not null).ToList();

	public string AsKey => ToString();

	public bool CanMoveForward => ForwardMoves.Any();

	public bool CanMoveBackward => BackwardMoves.Any();

	public bool LoopingMove { get; set; } = false;

	public int MovementScore => MazeMove switch
	{
		MazeMovement.Unknown => 0,
		MazeMovement.GoForward => 1,
		_ => 1000,
	};

	#endregion

	/// <summary>
	/// ctor
	/// </summary>
	/// <param name="location">The coords for the movement</param>
	/// <param name="direction">The direction of travel</param>
	/// <param name="mazeMove">The type of move being made</param>
	/// <param name="backwardMove">The move leading to this one</param>
	public ReindeerMove(MapCoord location, DirectionOfTravel direction, MazeMovement mazeMove)
	{
		ArgumentNullException.ThrowIfNull(location, nameof(location));
		Location = location;
		Direction = direction;
		MazeMove = mazeMove;
	}

	#region Overrides from base

	/// <summary>
	/// Debug friendly override to display the movement
	/// </summary>
	/// <returns>A text representation of the move</returns>
	public override string ToString()
	{
		return $"{Location}:{Direction}:{MazeMove}";
	}

	#endregion

	#region Methods

	/// <summary>
	/// Add a forward move to this move
	/// </summary>
	/// <param name="forwardMove">One of the possible forward moves from this one</param>
	/// <exception cref="ArgumentException"></exception>
	public void AddForwardMove(ReindeerMove forwardMove)
	{
		ArgumentNullException.ThrowIfNull(forwardMove, nameof(forwardMove));
		var existingMove = _forwardMoves[forwardMove.MazeMove];
		if (existingMove is not null)
			throw new ArgumentException($"This {nameof(ReindeerMove)} already has a move for '{forwardMove.MazeMove}' defined", nameof(forwardMove));
		_forwardMoves[forwardMove.MazeMove] = forwardMove;
	}

	public void AddBackwardMove(ReindeerMove backwardMove)
	{
		ArgumentNullException.ThrowIfNull(backwardMove, nameof(backwardMove));
		var existingMove = _backwardMoves[backwardMove.Direction];

		//	Check if already assigned this exact same movement
		if (existingMove == backwardMove)
			return;

		//	Not the same move, so something has gone wrong
		if (existingMove is not null)
			throw new ArgumentException($"This {nameof(ReindeerMove)} already has a move for '{backwardMove.Direction}' defined", nameof(backwardMove));

		//	Not seen this move before, so assign it
		_backwardMoves[backwardMove.Direction] = backwardMove;
	}

	#endregion
}