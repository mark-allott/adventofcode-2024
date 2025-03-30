using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day19;

public partial class Day19
	: AbstractDailyChallenge, IAutoRegister
{
	#region Overrides to run part two of challenge

	/// <summary>
	/// Overrides the base implementation to provide the solution for part two
	/// </summary>
	/// <returns></returns>
	protected override bool PartTwo()
	{
		LoadAndReadFile();

		var (patterns, designs) = ReadData(InputFileLines);
		var ll = new LinenLayout(patterns, designs);
		var goodDesignScores = ll.GetValidDesignScores();

		PartTwoResult = $"{ChallengeTitle} : {goodDesignScores}";
		return true;
	}

	#endregion
}