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
	public int ColumnNumber { get; private set; } = -1;

	/// <summary>
	/// The 0-based Y-coordinate of the start of the phrase
	/// </summary>
	public int RowNumber { get; private set; } = -1;

	#region ctor

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="column">The 0-based column-coordinate of the start of the phrase</param>
	/// <param name="row">The 0-based row-coordinate of the start of the phrase</param>
	/// <param name="direction">The direction of travel which the solution takes in the grid</param>
	public WordGridResult(int column, int row, WordGridDirection direction)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(column, 0, nameof(column));
		ArgumentOutOfRangeException.ThrowIfLessThan(row, 0, nameof(row));
		ArgumentOutOfRangeException.ThrowIfEqual((int)direction, (int)WordGridDirection.Unknown, nameof(direction));

		ColumnNumber = column;
		RowNumber = row;
		Direction = direction;
	}

	#endregion
}