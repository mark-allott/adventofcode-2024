using System.Diagnostics;
using System.Text;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges;

public partial class Day09
	: AbstractDailyChallenge, IAutoRegister, IPartOneTestable
{
	#region Overrides to run part one of challenge

	/// <summary>
	/// Override the base implementation to provide the actual answer
	/// </summary>
	/// <returns>True if successful</returns>
	protected override bool PartOne()
	{
		LoadAndReadFile();

		long total = 0;
		foreach (var part in InputFileLines)
		{
			var expanded = ExpandDiskMap(part);
			var compacted = CompactDiskMap(expanded);
			var checksum = CalculateDiskMapChecksum(compacted);
			total += checksum;
		}
		PartOneResult = $"Checksum = {total}";
		return true;
	}

	#endregion

	#region IPartOneTestable implementation

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	public void PartOneTest()
	{
		var t = 0;
		foreach (var part in _partOneTestInput)
		{
			var expanded = ExpandDiskMap(part);
			Debug.Assert(_partOneTestExpansion[t] == DiskBlocksToString(expanded));

			var compacted = CompactDiskMap(expanded);
			Debug.Assert(_partOneTestCompacted[t] == DiskBlocksToString(compacted));

			var checksum = CalculateDiskMapChecksum(compacted);
			Debug.Assert(_partOneTestChecksum[t] == checksum);
			t++;
		}
	}

	/// <summary>
	/// test data, as per challenge specifications
	/// </summary>
	private List<string> _partOneTestInput = new List<string>()
	{
		"12345",
		"2333133121414131402"
	};

	/// <summary>
	/// From test data specifications, allow assertions that the "expanded" version
	/// of the diskblocks matches expectations
	/// </summary>
	private List<string> _partOneTestExpansion = new List<string>()
	{
		"0..111....22222",
		"00...111...2...333.44.5555.6666.777.888899"
	};

	/// <summary>
	/// From the test data specifications, allow assertions to confirm that the
	/// compacting operation works as expected
	/// </summary>
	private List<string> _partOneTestCompacted = new List<string>()
	{
		"022111222......",
		"0099811188827773336446555566.............."
	};

	/// <summary>
	/// From the test specifications, assert that the block checksums work as expected
	/// </summary>
	private List<int> _partOneTestChecksum = new List<int>()
	{
		60, 1928
	};

	#endregion

	#region Part One code

	/// <summary>
	/// Helper method to take the compact representation of the disk and break
	/// it into individual blocks
	/// </summary>
	/// <param name="map">The string representing the compact disk descriptor</param>
	/// <returns>The disk as a list of <see cref="DiskBlock"/> objects</returns>
	private List<DiskBlock> ExpandDiskMap(string map)
	{
		//	Create the container for the blocks - check if there is anything to convert
		var blocks = new List<DiskBlock>();
		if (string.IsNullOrWhiteSpace(map))
			return blocks;

		//	The first file entry has index zero, and increments for each block
		var index = 0;
		do
		{
			//	Get the current block descriptor
			var current = map.Length >= 2
				? map[0..2]
				: map[0..1];

			//	Add it to the container
			blocks.Add(new DiskBlock(index, current));

			//	re-assign the map to remove the block descriptor at the front
			map = map.Length > 2
				? map[2..]
				: string.Empty;
			index++;
		}
		while (map.Length > 0);
		return blocks;
	}

	/// <summary>
	/// Helper method to convert the blocks into a string representation
	/// </summary>
	/// <param name="blocks">The blocks to convert</param>
	/// <returns>The string representation of the blocks</returns>
	private string DiskBlocksToString(List<DiskBlock> blocks)
	{
		var sb = new StringBuilder();
		foreach (var block in blocks)
			sb.Append(block.ToString());
		return sb.ToString();
	}

	/// <summary>
	/// Helper method that performs the compacting of the disk, as per the specification
	/// </summary>
	/// <param name="blocks">The lst of <see cref="DiskBlock"/> objects to be compacted</param>
	/// <returns>The blocks in their compacted state</returns>
	private List<DiskBlock> CompactDiskMap(List<DiskBlock> blocks)
	{
		//	Loop for every block present in the list
		//	The loop starts at the beginning of the list and moves forwards
		foreach (var block in blocks)
		{
			//	If the block is empty, no compacting needs to be performed
			if (block.IsEmpty)
				continue;

			//	Loop whilst the current block still has room available
			while (!block.IsFull)
			{
				//	Find the first non-empty block starting from the end of the list
				var swapBlock = blocks.Last(q => !q.IsEmpty);

				//	If the block to move from and move into are the same, then stop
				if (swapBlock == block)
					break;
				//	Move the block from the end towards the front
				swapBlock.MoveTo(block);
			}
		}
		return blocks;
	}

	/// <summary>
	/// Calculates the checksum for the whole disk
	/// </summary>
	/// <param name="blocks">The list of <see cref="DiskBlock"/> objects</param>
	/// <returns>The checksum for the disk</returns>
	private long CalculateDiskMapChecksum(List<DiskBlock> blocks)
	{
		//	Explicitly specify as long values
		var checksum = 0L;
		var startPosition = 0L;

		//	Calculate the checksum for the block, incrementing to overall checksum
		foreach (var block in blocks)
		{
			var (newStartPosition, blockChecksum) = block.CalculateChecksum(startPosition);
			startPosition = newStartPosition;
			checksum += blockChecksum;
		}
		return checksum;
	}

	#endregion
}