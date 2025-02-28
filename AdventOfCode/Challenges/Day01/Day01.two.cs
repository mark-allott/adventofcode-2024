using AdventOfCode.Interfaces;

namespace AdventOfCode.Challenges.Day01;

public partial class Day01
	: AbstractDailyChallenge, IAutoRegister
{
	#region Overrides for part two of the challenge

	protected override bool PartTwo()
	{
		LoadAndReadFile();

		var numberLists = ParseLinesToIntegerLists(InputFileLines);

		var (a, b) = SplitIntoSeparateOrderedLists(numberLists);
		var similarityScore = 0;

		foreach (var number in a)
			similarityScore += number * b.Count(c => c == number);
		PartTwoResult = $"Similarity score for lists is: {similarityScore}";
		return true;
	}

	#endregion
}