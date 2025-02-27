using System.Diagnostics;
using System.Text;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges;

public partial class Day09
	: AbstractDailyChallenge, IAutoRegister, IPartTwoTestable
{
	#region Overrides to run part two of challenge

	/// <summary>
	/// Overrides the base implementation to provide the solution for part two
	/// </summary>
	/// <returns></returns>
	protected override bool PartTwo()
	{
		LoadAndReadFile();

		long total = 0;
		foreach (var part in InputFileLines)
		{
			var expanded = ExpandDiskMap2(part);
			var reverseCheck = string.Join("", expanded.Select(s => s.Initialiser));
			Debug.Assert(reverseCheck == part);

			var compacted = CompactFiles(expanded);
			var checksum = CalculateDiskMapChecksum(compacted);
			total += checksum;
		}

		PartTwoResult = $"Disk checksum = {total}";
		return true;
	}

	/// <summary>
	/// Helper method to take the compact representation of the disk and break
	/// it into individual blocks
	/// </summary>
	/// <param name="map">The string representing the compact disk descriptor</param>
	/// <returns>The disk as a list of <see cref="DiskBlock"/> objects</returns>
	private List<DiskBlockEx> ExpandDiskMap2(string map)
	{
		//	Create the container for the blocks - check if there is anything to convert
		var blocks = new List<DiskBlockEx>();
		if (string.IsNullOrWhiteSpace(map))
			return blocks;

		//	The first file entry has index zero, and increments for each block
		var index = 0;
		var offset = 0;
		do
		{
			//	Get the current block descriptor
			var current = map.Length >= 2
				? map[0..2]
				: map[0..1];

			//	Add it to the container
			var newBlock = new DiskBlockEx(index, current, offset);
			blocks.Add(newBlock);

			//	re-assign the map to remove the block descriptor at the front
			map = map.Length > 2
				? map[2..]
				: string.Empty;

			offset += newBlock.Length;
			index++;
		}
		while (map.Length > 0);
		return blocks;
	}

	/// <summary>
	/// Helper method that performs the compacting of the disk by file, as per the specification
	/// </summary>
	/// <param name="blocks">The list of <see cref="DiskBlock"/> objects to be compacted</param>
	/// <returns>The blocks in their compacted state</returns>
	private List<DiskBlockEx> CompactFiles(List<DiskBlockEx> blocks)
	{
		var cwd = Directory.GetCurrentDirectory();
		var outputFilePath = Path.Combine(cwd, "data", "debug", $"day-{DayNumber:00}-compact.txt");

		if (File.Exists(outputFilePath))
			File.Delete(outputFilePath);

		File.WriteAllText(outputFilePath, $"{DateTime.UtcNow:O}: Running {nameof(CompactFiles)}" + Environment.NewLine);

		var rightBlockIndex = blocks.Count - 1;

		while (rightBlockIndex > 0)
		{
			var rightBlock = blocks[rightBlockIndex];
			rightBlockIndex--;

			if (rightBlock.IsEmpty)
				continue;

			var candidate = blocks.Where(q => q.SpaceRemaining >= rightBlock.BlockLength)
				.FirstOrDefault(q => q.BlockIndex < rightBlock.BlockIndex);
			if (candidate is null)
				continue;

			var spaceBefore = candidate.SpaceRemaining;
			if (rightBlock.MoveTo(candidate))
			{
				var spaceAfter = candidate.SpaceRemaining;
				File.AppendAllText(outputFilePath, $"Block moved: {rightBlock.BlockIndex:0000} [Len:{rightBlock.BlockLength:00}] => Block {candidate.BlockIndex:0000} [Len:{spaceBefore} to Len:{spaceAfter}]" + Environment.NewLine);
			}

			// WriteBlocks(blocks, rightBlock.BlockIndex);
		}
		return blocks;
	}

	private void WriteBlocks(List<DiskBlockEx> blocks, int iterations)
	{
		var output = new List<string>();
		foreach (var block in blocks)
			output.Add(block.ToDebugString());
		try
		{
			var cwd = Directory.GetCurrentDirectory();
			var inputFilePath = Path.Combine(cwd, "data", "debug", $"day-{DayNumber:00}-debug-{iterations:0000}.txt");
			File.WriteAllLines(inputFilePath, output, Encoding.ASCII);
		}
		catch (IOException iox)
		{
			Console.WriteLine(iox.Message);
		}
	}

	/// <summary>
	/// Convert the blocks into a string equivalent to the "expanded" string from the website examples
	/// </summary>
	/// <param name="blocks"></param>
	/// <returns></returns>
	private string DiskBlocksExToString(List<DiskBlockEx> blocks)
	{
		var sb = new StringBuilder();
		foreach (var block in blocks)
			sb.Append(block.ToString());
		return sb.ToString();
	}

	/// <summary>
	/// Helper method to calculate the checksum for the entire disk
	/// </summary>
	/// <param name="blocks">The blocks that represent the disk</param>
	/// <returns>The checksum for the disk</returns>
	private long CalculateDiskMapChecksum(List<DiskBlockEx> blocks)
	{
		//	Explicitly specify as long values
		var checksum = 0L;

		//	Calculate the checksum for the block, incrementing to overall checksum
		foreach (var block in blocks)
		{
			var blockChecksum = block.CalculateChecksum();
			checksum += blockChecksum;
		}

		// ExportMapAsCsv(blocks);
		return checksum;
	}

	/// <summary>
	/// Debug helper method to output the disk as a CSV file
	/// </summary>
	/// <param name="blocks">The blocks representing the disk</param>
	private void ExportMapAsCsv(List<DiskBlockEx> blocks)
	{
		var csv = new List<string>();
		foreach (var block in blocks)
			csv.AddRange(block.ExportBlocksToCsv());

		try
		{
			var cwd = Directory.GetCurrentDirectory();
			var inputFilePath = Path.Combine(cwd, "data", "debug", $"day-{DayNumber:00}-debug-blocks.csv");
			File.WriteAllLines(inputFilePath, csv, Encoding.ASCII);
		}
		catch (IOException iox)
		{
			Console.WriteLine(iox.Message);
		}
	}

	#endregion

	#region Test data for part two

	/// <summary>
	/// Using rules set out in the challenge, run some tests to make sure the
	/// code behaves as expected
	/// </summary>
	public void PartTwoTest()
	{
		var expanded = ExpandDiskMap2(_partTwoTestInput);

		var reverseCheck = string.Join("", expanded.Select(s => s.Initialiser));
		Debug.Assert(reverseCheck == _partTwoTestInput);
		ArgumentOutOfRangeException.ThrowIfNotEqual(DiskBlocksExToString(expanded), _partTwoExpanded, nameof(expanded));
		Debug.Assert(_partTwoExpanded == DiskBlocksExToString(expanded));

		var compacted = CompactFiles(expanded);
		ArgumentOutOfRangeException.ThrowIfNotEqual(DiskBlocksExToString(compacted), _partTwoCompacted, nameof(compacted));
		Debug.Assert(_partTwoCompacted == DiskBlocksExToString(compacted));

		var checksum = CalculateDiskMapChecksum(compacted);
		ArgumentOutOfRangeException.ThrowIfNotEqual(checksum, 2858, nameof(checksum));
		Debug.Assert(2858 == checksum);
	}

	private string _partTwoTestInput = "2333133121414131402";
	private string _partTwoExpanded = "00...111...2...333.44.5555.6666.777.888899";
	private string _partTwoCompacted = "00992111777.44.333....5555.6666.....8888..";

	#endregion
}