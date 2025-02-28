using System.Diagnostics;
using AdventOfCode.Extensions;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day11;

public partial class Day11
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
		PartOneResult = $"Number of pebbles = {total}";
		return true;
	}

	#endregion

	#region IPartOneTestable implementation

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	public void PartOneTest()
	{
	}

	/// <summary>
	/// test data, as per challenge specifications
	/// </summary>
	private string _partOneTestInput1 = "0 1 10 99 999";
	private string _partOneTestInput2 = "125 17";

	/// <summary>
	/// From the test specifications, assert that the result after blinking is correct
	/// </summary>
	private List<string> _partOneTestInput1AfterBlink = new List<string>()
	{
		"1 2024 1 0 9 9 2021976"
	};
	private List<string> _partOneTestInput2AfterBlink = new List<string>()
	{
		"253000 1 7",
		"253 0 2024 14168",
		"512072 1 20 24 28676032",
		"512 72 2024 2 0 2 4 2867 6032",
		"1036288 7 2 20 24 4048 1 4048 8096 28 67 60 32",
		"2097446912 14168 4048 2 0 2 4 40 48 2024 40 48 80 96 2 8 6 7 6 0 3 2"
	};

	#endregion

	#region Part One code

	#endregion
}