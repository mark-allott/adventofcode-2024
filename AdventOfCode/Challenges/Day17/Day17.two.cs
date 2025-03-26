using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day17;

public partial class Day17
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

		string output = "";
		PartTwoResult = $"{ChallengeTitle} outputs = {output}";
		return true;
	}

	#endregion

	#region IPartTwoTestable implementation

	public void PartTwoTest()
	{
	}

	#endregion
}