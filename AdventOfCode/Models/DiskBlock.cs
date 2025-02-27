using System.Text;

namespace AdventOfCode.Models;

internal class DiskBlock
	: IEquatable<DiskBlock>
{
	/// <summary>
	/// Holds the blocks used and their index number
	/// </summary>
	protected int?[] _fileBlocks = null!;

	/// <summary>
	/// Holds the amount of space available in the block
	/// </summary>
	protected int _totalSpace = 0;

	public int BlockLength => _totalSpace;

	protected readonly int _blockIndex = -1;

	protected readonly int _blockOffset;

	protected readonly string _initial;

	/// <summary>
	/// ctor - initialises a new disk block with the appropriate index number and quantity of blocks used
	/// </summary>
	/// <param name="index">The index number of the disk block data</param>
	/// <param name="initialiser">The initialisation string for the block</param>
	public DiskBlock(int index, string initialiser, int offset)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index, nameof(index));
		ArgumentException.ThrowIfNullOrWhiteSpace(initialiser, nameof(initialiser));
		ArgumentOutOfRangeException.ThrowIfNegative(offset, nameof(offset));

		//	Set the original index value for the file in this block
		_blockIndex = index;

		//	Set the offset value for start of block
		_blockOffset = offset;

		//	The first character of the initialiser is the number of entries in the block
		var usedBlocks = int.Parse(initialiser[0..1]);

		//	Calculate the amount of additional free space using the 2nd character (if present)
		var freeSpace = initialiser.Length > 1
			? int.Parse(initialiser[1..2])
			: 0;
		_initial = initialiser.PadRight(2);
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
	public long CalculateChecksum()
	{
		if (IsEmpty)
			return 0L;

		var checksum = 0L;
		for (var i = 0; i < _totalSpace; i++)
		{
			var blockValue = _fileBlocks[i];
			checksum += blockValue.HasValue
				? blockValue.Value * (_blockOffset + i)
				: 0;
		}
		return checksum;
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

internal class DiskBlockEx
	: IEquatable<DiskBlockEx>
{
	#region Fields

	/// <summary>
	/// The original index value for the block
	/// </summary>
	public int BlockIndex { get; private set; }

	/// <summary>
	/// The offset value from the start of the disk for this block
	/// </summary>
	public int BlockOffset { get; private set; }

	/// <summary>
	/// The length of the original data block
	/// </summary>
	public int BlockLength { get; private set; }

	/// <summary>
	/// The total length of this block
	/// </summary>
	public int Length { get; private set; }

	/// <summary>
	/// A block that is full has no space at the end
	/// </summary>
	public bool IsFull => !_blocks[^1].IsSpace;

	/// <summary>
	/// A block that is empty only contains space
	/// </summary>
	public bool IsEmpty => _blocks[0].IsSpace;

	/// <summary>
	/// The amount of space left is determined by the last BlockEntry
	/// </summary>
	public int SpaceRemaining => _blocks[^1].IsSpace ? _blocks[^1].Length : 0;

	/// <summary>
	/// A container for the individual <see cref="BlockEntry"/> objects
	/// </summary>
	private List<BlockEntry> _blocks = new List<BlockEntry>();

	/// <summary>
	/// The value of the initialiser
	/// </summary>
	private readonly string _initial;

	/// <summary>
	/// Publically accessible value for the initialiser with no spaces present
	/// </summary>
	public string Initialiser => _initial.Trim();

	#endregion

	#region ctor

	/// <summary>
	/// Constructor for the block and the initial data block
	/// </summary>
	/// <param name="index">The value of the data block index</param>
	/// <param name="initialiser">The string to initialise the block</param>
	/// <param name="offset">The offset value from the start of the disk</param>
	public DiskBlockEx(int index, string initialiser, int offset)
	{
		//	Validate inputs:
		//		*	index CANNOT be negative
		//		*	initialiser CANNOT be blank/empty/null/whitespace
		//		*	offset CANNOT be negative
		ArgumentOutOfRangeException.ThrowIfNegative(index, nameof(index));
		ArgumentNullException.ThrowIfNullOrWhiteSpace(initialiser, nameof(initialiser));
		ArgumentOutOfRangeException.ThrowIfNegative(offset, nameof(offset));

		//	Set the original index value for the file in this block
		BlockIndex = index;

		//	Set the offset value for start of block
		BlockOffset = offset;

		//	Store initialiser for debug printing
		_initial = initialiser.PadRight(2);

		//	The first character of the initialiser is the number of entries in the block
		var usedBlocks = int.Parse(initialiser[0..1]);

		//	Calculate the amount of additional free space using the 2nd character (if present)
		var freeSpace = initialiser.Length > 1
			? int.Parse(initialiser[1..2])
			: 0;

		//	Add the data block
		_blocks.Add(new BlockEntry(BlockIndex, usedBlocks));
		//	Any spare space is represented by a "space" block
		if (freeSpace > 0)
			_blocks.Add(BlockEntry.Space(freeSpace));

		//	Set the values for the publically accessible properties
		BlockLength = usedBlocks;
		Length = usedBlocks + freeSpace;
	}

	#endregion

	#region IEquatable implementation

	public bool Equals(DiskBlockEx? other)
	{
		return other is not null &&
			BlockIndex == other.BlockIndex &&
			BlockOffset == other.BlockOffset &&
			_blocks.SequenceEqual(other._blocks);
	}

	#endregion

	#region Overrides from base
	/// <summary>
	/// Override base method to give a helping hand visualising the block in debug sessions
	/// </summary>
	/// <returns>The string representation of the block</returns>
	/// <remarks>
	/// The routine only really works on index values for the blocks lower than 62
	/// due to conversion into a single alphanumeric value for each block used
	/// </remarks>
	public override string ToString()
	{
		var sb = new StringBuilder();
		foreach (var block in _blocks)
			sb.Append(block.ToString());
		return sb.ToString();
	}

	#endregion

	/// <summary>
	/// Adds a <see cref="BlockEntry"/> object to this container
	/// </summary>
	/// <param name="newEntry">The data block to add to this block</param>
	/// <returns>True if the operation succeeds, otherwise false</returns>
	public bool Add(BlockEntry newEntry)
	{
		//	validate input:
		//		*	cannot be null
		//		*	cannot move right (BlockIndex of newEntry MUST be greater than current BlockIndex)
		ArgumentNullException.ThrowIfNull(newEntry, nameof(newEntry));
		ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(newEntry.BlockIndex ?? -1, BlockIndex, nameof(newEntry.BlockIndex));

		//	incoming files are added into the end block, which must be a space and of sufficient size
		var lastEntry = _blocks[^1];

		//	last block MUST be a space block with sufficient space to be replaced
		if (!lastEntry.IsSpace || newEntry.Length > lastEntry.Length)
			return false;

		//	Replace last block with incoming entry
		_blocks[^1] = newEntry;

		//	If the space available was greater than the incoming block, create a new space entry of correct size
		if (newEntry.Length < lastEntry.Length)
			_blocks.Add(BlockEntry.Space(lastEntry.Length - newEntry.Length));
		return true;
	}

	/// <summary>
	/// Moves the data block from this container to the <paramref name="other"/> container
	/// </summary>
	/// <param name="other">The container to receive the data block</param>
	/// <returns>True if the operation succeeds, otherwise false</returns>
	/// <exception cref="ArgumentException"></exception>
	public bool MoveTo(DiskBlockEx other)
	{
		//	Check the other valid is a valid location to move into
		ArgumentNullException.ThrowIfNull(other);
		if (other.IsFull)
			throw new ArgumentException("Cannot move into a full block", nameof(other));

		//	Check this block has something to move
		if (IsEmpty)
			throw new ArgumentException("Cannot move anything from an empty block", nameof(IsEmpty));

		var blockToMove = _blocks[0];
		if (!other.Add(blockToMove))
			return false;

		//	Set the entry for the original data block to be a space block
		_blocks[0] = BlockEntry.SpaceFromBlock(blockToMove);

		//	Tidy things up a bit...
		//		*	The data block doesn't exist now, so BlockLength is now zero
		//		*	If all blocks in the container are now spaces, replace with a single space block
		BlockLength = 0;
		if (_blocks.All(b => b.IsSpace))
		{
			_blocks.Clear();
			_blocks.Add(BlockEntry.Space(Length));
		}
		return true;
	}

	/// <summary>
	/// Debug helper method - yields a prettified version of the block
	/// </summary>
	/// <returns>The prettified version of the block</returns>
	public string ToDebugString()
	{
		//	Make an array of the block contents
		var items = new List<string>();
		foreach (var block in _blocks)
			items.AddRange(block.ToDebugStrings());

		var sb = new StringBuilder();

		sb.Append($"[{BlockIndex:00000} | {_initial} | {BlockOffset:000000}] ")
			.Append('[')
			.Append(string.Join(',', items))
			.Append(']')
			.Append($" | {CalculateChecksum()} |");
		return sb.ToString();
	}

	/// <summary>
	/// Debug helper method to convert the block into a CSV value
	/// </summary>
	/// <returns>Returns the block as a CSV</returns>
	public List<string> ExportBlocksToCsv()
	{
		var lines = new List<string>();
		var offset = BlockOffset;
		foreach (var block in _blocks)
		{
			lines.AddRange(block.BlockAsCsv(offset));
			offset += block.Length;
		}
		return lines;
	}

	/// <summary>
	/// Calculate the checksum for this block from the <see cref="BlockEntry"/> entities
	/// </summary>
	/// <returns>The checksum</returns>
	public long CalculateChecksum()
	{
		var checksum = 0L;
		var offset = BlockOffset;

		foreach (var block in _blocks)
		{
			checksum += block.Checksum(offset);
			offset += block.Length;
		}

		return checksum;
	}

	/// <summary>
	/// debug helper method to return the block as a list of integer values
	/// </summary>
	/// <returns>The integer values of the block</returns>
	public List<int> ExportBlockAsInts()
	{
		var rows = new List<int>();
		foreach (var block in _blocks)
			rows.AddRange(block.BlockAsInts());
		return rows;
	}
}