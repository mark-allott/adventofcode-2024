using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day13;

public partial class Day13
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

		long total = 0;
		var contraptions = GetContraptionDetail(InputFileLines);

		for (var i = 0; i < contraptions.Count; i++)
		{
			var (a, b, p) = contraptions[i];
			var solver = new ClawContraptionSolver(a, b, p);
			solver.CorrectPrizeLocation();
			if (solver.Solve())
				total += solver.GetCost(3, 1);
		}
		PartTwoResult = $"Number of tokens = {total}";
		return true;
	}

	#endregion
}