using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day19;

public partial class Day19
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

		PartTwoResult = $"{ChallengeTitle} : N/A";
		return true;
	}

	#endregion

	#region IPartTwoTestable implementation

	public void PartTwoTest()
	{
	}

	#endregion
}