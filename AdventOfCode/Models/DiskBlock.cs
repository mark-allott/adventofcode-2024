using System.Diagnostics;
using System.Text;

namespace AdventOfCode.Models;

internal class DiskBlock
	: IEquatable<DiskBlock>
{
	/// <summary>
	/// Holds the blocks used and their index number
	/// </summary>
	private int?[] _fileBlocks = null!;

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

		//	Calculate the amount of additional free space using the 2nd character (if present)
		var freeSpace = initialiser.Length > 1
			? int.Parse(initialiser[1..2])
			: 0;

		//	calculate the total number of elements in this block
		_totalSpace = usedBlocks + freeSpace;

		//	Allocate block memory
		_fileBlocks = new int?[_totalSpace];
		for (var i = 0; i < usedBlocks; i++)
			_fileBlocks[i] = index;
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
		for (var i = 0; i < _totalSpace; i++)
		{
			var block = _fileBlocks[i];
			char c = block switch
			{
				null => '.',
				< 10 => (char)((int)'0' + block),
				< 36 => (char)((int)'A' + block - 10),
				< 62 => (char)((int)'a' + block - 36),
				_ => '?'
			};
			sb.Append(c);
		}
		return sb.ToString();
	}

	/// <summary>
	/// Helper property to determine if all entries have been allocated
	/// </summary>
	public bool IsFull => _fileBlocks.All(q => q.HasValue);

	/// <summary>
	/// Helper property to determine if any blocks are currently being used
	/// </summary>
	public bool IsEmpty => _fileBlocks.All(q => !q.HasValue);

	/// <summary>
	/// Helper method to exchange the last non-empty item in the list of this
	/// disk block into the first empty position in the other block
	/// </summary>
	/// <param name="other">The block to accept the transfer</param>
	/// <returns>True if the transfer succeeds, otherwise false</returns>
	/// <exception cref="ArgumentException"></exception>
	public bool MoveTo(DiskBlock other)
	{
		//	Check the other valid is a valid location to move into
		ArgumentNullException.ThrowIfNull(other);
		if (other.IsFull)
			throw new ArgumentException("Cannot move into a full block", nameof(other));

		//	Check this block has something to move
		if (IsEmpty)
			throw new ArgumentException("Cannot move anything from an empty block", nameof(IsEmpty));

		//	locate the block to move
		for (var i = _totalSpace; i > 0; i--)
		{
			var blockIndex = _fileBlocks[i - 1];

			if (!blockIndex.HasValue)
				continue;

			if (other.Add(blockIndex.Value))
			{
				_fileBlocks[i - 1] = null;
				return true;
			}
		}
		return false;
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

		for (var i = 0; i < _totalSpace; i++)
		{
			if (_fileBlocks[i].HasValue)
				continue;
			_fileBlocks[i] = index;
			return true;
		}
		return false;
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
		foreach (var i in _fileBlocks)
		{
			checksum += i.HasValue
				? i.Value * (startPosition + offset)
				: 0;
			offset++;
		}
		return (startPosition + _totalSpace, checksum);
	}

	#region IEquatable implementation

	public bool Equals(DiskBlock? other)
	{
		return other is not null &&
			_totalSpace == other._totalSpace &&
			_fileBlocks.Length == other._fileBlocks.Length &&
			_fileBlocks.SequenceEqual(other._fileBlocks);
	}

	#endregion
}