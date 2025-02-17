using AdventOfCode.Enums;

namespace AdventOfCode.Models;

/// <summary>
/// Model class to hold details of a matched phrase in the word grid
/// </summary>
internal class WordGridResult
{
	/// <summary>
	/// The direction in which the phrase travels in the grid (based on compass points)
	/// </summary>
	public WordGridDirection Direction { get; private set; } = WordGridDirection.Unknown;

	/// <summary>
	/// The 0-based X-coordinate of the start of the phrase
	/// </summary>
	public int XCoord { get; private set; } = -1;

	/// <summary>
	/// The 0-based Y-coordinate of the start of the phrase
	/// </summary>
	public int YCoord { get; private set; } = -1;

	#region ctor

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="x">The 0-based x-coordinate of the start of the phrase</param>
	/// <param name="y">The 0-based y-coordinate of the start of the phrase</param>
	/// <param name="direction">The direction of travel which the solution takes in the grid</param>
	public WordGridResult(int x, int y, WordGridDirection direction)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(x, 0, nameof(x));
		ArgumentOutOfRangeException.ThrowIfLessThan(y, 0, nameof(y));
		ArgumentOutOfRangeException.ThrowIfEqual((int)direction, (int)WordGridDirection.Unknown, nameof(direction));

		XCoord = x;
		YCoord = y;
		Direction = direction;
	}

	#endregion
}