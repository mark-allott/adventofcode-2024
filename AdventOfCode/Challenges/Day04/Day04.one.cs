using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day04;

public partial class Day04
	: AbstractDailyChallenge, IAutoRegister
{
	#region Overrides to run part one of challenge

	/// <summary>
	/// Override the base implementation to provide the actual answer
	/// </summary>
	/// <returns>True if successful</returns>
	protected override bool PartOne()
	{
		LoadAndReadFile();

		var grid = new WordGrid();
		grid.LoadFromEnumerableOfString(InputFileLines);

		var results = grid.SearchGridForWord("XMAS");
		var total = results.Count;
		PartOneResult = $"Total number of XMAS words found = {total}";
		return true;
	}

	#endregion
}