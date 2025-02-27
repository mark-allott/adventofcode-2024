using System.Diagnostics;
using System.Text;

namespace AdventOfCode.Models;

internal class BlockEntry
	: IEquatable<BlockEntry>
{
	#region Fields

	private int? _blockIndex;
	private int _length;

	#endregion

	#region Property Accessors

	public bool IsSpace => !_blockIndex.HasValue;

	public int Length => _length;

	public int? BlockIndex => _blockIndex;

	#endregion

	#region ctor

	/// <summary>
	/// Standard constructor - takes the index and a length
	/// </summary>
	/// <param name="blockIndex">The index of the file (may be null)</param>
	/// <param name="length">The length of the file</param>
	public BlockEntry(int? blockIndex, int length)
	{
		if (blockIndex.HasValue)
			ArgumentOutOfRangeException.ThrowIfNegative(blockIndex ?? 0, nameof(blockIndex));
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length, nameof(length));

		_blockIndex = blockIndex;
		_length = length;
	}

	/// <summary>
	/// Static constructor to create a "space" of specific length
	/// </summary>
	/// <param name="length">The length of the file</param>
	/// <returns>The "space" of specified length</returns>
	public static BlockEntry Space(int length)
	{
		return new BlockEntry(null, length);
	}

	/// <summary>
	/// Static constructor to create a "space" based on an existing block
	/// </summary>
	/// <param name="entry">The <see cref="BlockEntry"/>> to be duplicated as a "space"</param>
	/// <returns>The "space" of required length</returns>
	public static BlockEntry SpaceFromBlock(BlockEntry entry)
	{
		return new BlockEntry(null, entry._length);
	}

	#endregion

	#region Overrides from base classes

	/// <summary>
	/// Overrides the base implementation to provide a helper for debugging
	/// </summary>
	/// <returns>The string representation of the file</returns>
	public override string ToString()
	{
		char c = _blockIndex switch
		{
			null => '.',
			< 10 => (char)((int)'0' + _blockIndex),
			< 36 => (char)((int)'A' + _blockIndex - 10),
			< 62 => (char)((int)'a' + _blockIndex - 36),
			_ => '?'
		};
		return new string(c, _length);
	}

	/// <summary>
	/// Debug helper method to return the block elements as a textual representation of the block
	/// </summary>
	/// <returns>The block index number for a data block, or "...." for a space block, repeated for the length of the block</returns>
	public List<string> ToDebugStrings()
	{
		var block = IsSpace
			? "...."
			: $"{BlockIndex:0000}";

		var output = new List<string>();
		for (var i = 0; i < _length; i++)
			output.Add(block);
		return output;
	}

	#endregion

	#region IEquatable implementation

	/// <summary>
	/// Determines if the two <see cref="BlockEntry"/> objects are equivalent
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public bool Equals(BlockEntry? other)
	{
		return other is not null &&
			_blockIndex == other._blockIndex &&
			_length == other._length;
	}

	#endregion

	/// <summary>
	/// Returns the checksum value for this block based on its start position (<paramref name="offset"/>)
	/// </summary>
	/// <param name="offset">The start position of the block on the disk</param>
	/// <returns>The checksum value for the block</returns>
	public long Checksum(int offset)
	{
		//	Space blocks are always zero value
		if (IsSpace)
			return 0L;

		var checksum = 0L;
		var blockIndex = _blockIndex ?? 0;

		for (var i = 0; i < _length; i++)
			checksum += blockIndex * (offset + i);

		//	Alternate way of calculating which sums the offsets and multiplies by the index value
		// long x = blockIndex * Enumerable.Range(offset, _length).Sum(s => (long)s);
		// Debug.Assert(x == checksum);
		return checksum;
	}

	/// <summary>
	/// Debug helper method to return the block as a list of CSV entries in the form "offset, index"
	/// </summary>
	/// <param name="offset">The start position on the disk for the block</param>
	/// <returns>The rows of CSV strings that represent the block</returns>
	public List<string> BlockAsCsv(int offset)
	{
		var rows = new List<string>();
		for (var i = 0; i < _length; i++)
			rows.Add($"{offset + i},{_blockIndex ?? 0}");
		return rows;
	}

	/// <summary>
	/// Debug helper to return the block as a list of integer values
	/// </summary>
	/// <returns></returns>
	public List<int> BlockAsInts()
	{
		var rows = new List<int>();
		for (var i = 0; i < _length; i++)
			rows.Add(_blockIndex ?? 0);
		return rows;
	}
}