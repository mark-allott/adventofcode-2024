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
		var complexityTotal = 0L;
		var solver = new KeypadConundrum();
		solver.SetupKeypads(_keypad, _arrowKeys, ' ');

		foreach (var code in InputFileLines)
			complexityTotal += solver.GetComplexity(code, 25);

		PartTwoResult = $"{ChallengeTitle} complexity = {complexityTotal}";
		return true;
	}

	#endregion

	#region IPartTwoTestable implementation

	public void PartTwoTest()
	{
	}

	#endregion
}