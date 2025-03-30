using System.Diagnostics;
using AdventOfCode.Extensions;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day19;

public partial class Day19
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

		var (patterns, designs) = ReadData(InputFileLines);
		var ll = new LinenLayout(patterns, designs);
		var goodDesigns = ll.GetValidDesigns();

		PartOneResult = $"{ChallengeTitle} designs that are possible = {goodDesigns}";
		return true;
	}

	#endregion

	#region IPartOneTestable implementation

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	public void PartOneTest()
	{
		var (patterns, designs) = ReadData(_partOneTestInput);
		var ll = new LinenLayout(patterns, designs);
		var goodDesigns = ll.GetValidDesigns();

		Debug.Assert(6 == goodDesigns);
	}

	private List<string> _partOneTestInput = new List<string>()
	{
		"r, wr, b, g, bwu, rb, gb, br",
		"",
		"brwrr",
		"bggr",
		"gbbr",
		"rrbgbr",
		"ubwu",
		"bwurrg",
		"brgr",
		"bbrgwb"
	};

	#endregion

	#region Methods

	private (List<string> patterns, List<string> designs) ReadData(IEnumerable<string> data)
	{
		var patterns = new List<string>();
		var designs = new List<string>();
		var readingPatterns = true;

		//	Read patterns and desired designs
		foreach (var line in data)
		{
			if (string.IsNullOrWhiteSpace(line))
			{
				readingPatterns = false;
				continue;
			}

			if (readingPatterns)
			{
				patterns.AddRange(GetPatterns(line));
			}
			else
			{
				designs.Add(line);
			}
		}
		return (patterns, designs);
	}

	public List<string> GetPatterns(string input)
	{
		return input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.OrderBy(o => o)
			.ToList();
	}

	#endregion
}