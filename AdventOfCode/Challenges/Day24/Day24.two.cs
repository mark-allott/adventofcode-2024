using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day24;

public partial class Day24
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
		var solver = new CrossedWires();
		solver.Initialise(InputFileLines);
		var result = $"{solver.SolvePartTwo()}";
		PartTwoResult = $"{ChallengeTitle} result = {result}";
		return true;
	}

	#endregion

	#region Methods

	#endregion
}