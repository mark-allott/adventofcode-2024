using System.Diagnostics;
using System.Text;

namespace AdventOfCode.Models;

internal class DiskBlock
	: IEquatable<DiskBlock>
{
	/// <summary>
	/// Holds the blocks used and their index number
	/// </summary>
	private List<int> _indexes = new List<int>();

	/// <summary>
	/// Holds the amount of space available in the block
	/// </summary>
	private int _totalSpace = 0;

	/// <summary>
	/// ctor - initialises a new disk block with the appropriate index number and quantity of blocks used
	/// </summary>
	/// <param name="index">The index number of the disk block data</param>
	/// <param name="initialiser">The initialisation string for the block</param>
	public DiskBlock(int index, string initialiser)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index, nameof(index));
		ArgumentException.ThrowIfNullOrWhiteSpace(initialiser, nameof(initialiser));

		//	The first character of the initialiser is the number of entries in the block
		var usedBlocks = int.Parse(initialiser[0..1]);
		//	Assign the value of the index to the entries
		for (var i = 0; i < usedBlocks; i++)
			_indexes.Add(index);

		//	Calculate the amount of additional free space using the 2nd character (if present)
		var freeSpace = initialiser.Length > 1
			? int.Parse(initialiser[1..2])
			: 0;

		//	calculate the total number of elements in this block
		_totalSpace = _indexes.Count + freeSpace;
	}

	/// <summary>
	/// Override base method to give a helping hand visualising the block in debug sessions
	/// </summary>
	/// <returns>The string representation of the block</returns>
	/// <remarks>
	/// The routine only really works on index values for the blocks lower than 62
	/// due to conversion into a single alphanumeric value
	/// </remarks>
	public override string ToString()
	{
		var sb = new StringBuilder();
		foreach (var index in _indexes)
		{
			char c = index switch
			{
				< 10 => (char)((int)'0' + index),
				< 36 => (char)((int)'A' + index - 10),
				< 62 => (char)((int)'a' + index - 10),
				_ => '?'
			};
			sb.Append(c);
		}
		sb.Append(new string('.', _totalSpace - _indexes.Count));
		return sb.ToString();
	}

	/// <summary>
	/// Helper property to determine if all entries have been allocated
	/// </summary>
	public bool IsFull => _indexes.Count == _totalSpace;

	/// <summary>
	/// Helper property to determine if any blocks are currently being used
	/// </summary>
	public bool IsEmpty => _indexes.Count == 0;

	/// <summary>
	/// Helper method to exchange the last non-empty item in the list of this
	/// disk block into the first empty position in the other block
	/// </summary>
	/// <param name="other">The block to accept the transfer</param>
	/// <returns>True if the transfer succeeds, otherwise false</returns>
	/// <exception cref="ArgumentException"></exception>
	public bool MoveTo(DiskBlock other)
	{
		var startCount = _indexes.Count;
		//	Check the other valid is a valid location to move into
		ArgumentNullException.ThrowIfNull(other);
		if (other.IsFull)
			throw new ArgumentException("Cannot move into a full block", nameof(other));
		//	Check this block has something to move
		if (IsEmpty)
			throw new ArgumentException("Cannot move anything from an empty block", nameof(IsEmpty));

		//	Grab the last index
		var swap = _indexes.LastOrDefault(int.MinValue);
		Debug.Assert(swap != int.MinValue);

		if (other.Add(swap))
			_indexes.RemoveAt(_indexes.Count - 1);
		return startCount == 1 + _indexes.Count;
	}

	/// <summary>
	/// Helper method to insert an entry to this disk block
	/// </summary>
	/// <param name="index">The value of the index to be added</param>
	/// <returns>True if the operation succeeds, otherwise false</returns>
	public bool Add(int index)
	{
		ArgumentOutOfRangeException.ThrowIfEqual(index, int.MinValue, nameof(index));

		if (IsFull)
			return false;

		_indexes.Add(index);
		return true;
	}

	/// <summary>
	/// Calculates the checksum for this block based upon the starting position
	/// and the index value of the entries in the block
	/// </summary>
	/// <param name="startPosition">The starting location of this block within the whole disk</param>
	/// <returns>The start position for the next block and the value of the checksum for this block</returns>
	public (long endPosition, long checksum) CalculateChecksum(long startPosition)
	{
		if (IsEmpty)
			return (startPosition + _totalSpace, 0);

		var checksum = 0L;
		var offset = 0;
		foreach (var i in _indexes)
		{
			checksum += i * (startPosition + offset);
			offset++;
		}
		return (startPosition + _totalSpace, checksum);
	}

	#region IEquatable implementation

	public bool Equals(DiskBlock? other)
	{
		return other is not null &&
			_totalSpace == other._totalSpace &&
			_indexes.Count == other._indexes.Count &&
			_indexes.SequenceEqual(other._indexes);
	}

	#endregion
}