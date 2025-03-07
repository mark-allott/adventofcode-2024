using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day14;

public partial class Day14
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

		var grid = new SecurityBotGrid(101, 103);
		foreach (var line in InputFileLines)
		{
			var (x, y, dx, dy) = GetCoords(line);
			grid.AddBot(x, y, dx, dy);
		}

		long total = grid.ElapsedUntilEasterEgg(4);
		grid.WriteResultToFile();
		PartTwoResult = $"Easter egg found after {total} moves";
		return true;
	}

	#endregion
}
