using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day21;

public partial class Day21
	: AbstractDailyChallenge, IAutoRegister, IPartTwoTestable
{
	#region Overrides to run part two of challenge

	/// <summary>
	/// Overrides the base implementation to provide the solution for part two
	/// </summary>
	/// <returns></returns>
	protected override bool PartTwo()
	{
		string output = $"N/A";
		PartTwoResult = $"{ChallengeTitle} complexity = {output}";
		return true;
	}

	#endregion

	#region IPartTwoTestable implementation

	public void PartTwoTest()
	{
	}

	#endregion
}