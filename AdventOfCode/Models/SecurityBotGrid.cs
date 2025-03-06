using System.Numerics;

namespace AdventOfCode.Models;

internal class SecurityBotGrid
{
	#region Fields

	private readonly Vector2 bounds;

	private readonly List<SecurityBot> _bots = new List<SecurityBot>();

	#endregion

	#region Properties

	public Vector2 GridBounds => bounds;

	public long SafetyFactor => GetSafetyFactor();

	public int BotCount => _bots.Count;

	#endregion

	#region Constructors

	public SecurityBotGrid(int width, int height)
	{
		bounds = new Vector2(width, height);
	}

	#endregion

	#region Methods

	/// <summary>
	/// Adds a new bot to the grid
	/// </summary>
	/// <param name="x">X-Coord of start location</param>
	/// <param name="y">Y-Coord of start location</param>
	/// <param name="dx">X-offset of velocity</param>
	/// <param name="dy">Y-offset of velocity</param>
	public SecurityBot AddBot(int x, int y, int dx, int dy)
	{
		var bot = new SecurityBot(x, y, dx, dy, this);
		_bots.Add(bot);

		return bot;
	}

	/// <summary>
	/// Moves all bots on the grid by the specified number of moves
	/// </summary>
	/// <param name="count">The number of moves to be made</param>
	public void Move(int count = 1)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count, nameof(count));
		_bots.ForEach(b => b.MoveFor(count));
	}

	/// <summary>
	/// Calculate the safety factor by multiplying the number of bots in each quadrant
	/// </summary>
	/// <returns>The safety factor of the grid</returns>
	private long GetSafetyFactor()
	{
		var quadrants = _bots
			.Where(q => q.Quadrant != 0)
			.GroupBy(g => g.Quadrant)
			.Select(g => g.ToList().Count)
			.ToList();
		var result = 1L;

		quadrants.ForEach(i => result *= i);
		return result;
	}

	/// <summary>
	/// Performs a reset of the grid, putting all bots to their start positions as if they have never moved
	/// </summary>
	public void Reset()
	{
		_bots.ForEach(b => b.Reset());
	}

	#endregion
}