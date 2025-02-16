using AdventOfCode.Interfaces;

namespace AdventOfCode.Challenges;

public partial class Day01
	: AbstractDailyChallenge, IAutoRegister
{
	#region Overrides for part one of the challenge

	protected override bool PartOne()
	{
		LoadAndReadFile();

		var numberLists = ParseLinesToIntegerLists(InputFileLines);

		var (a, b) = SplitIntoSeparateOrderedLists(numberLists);

		var answer = 0;

		for (var i = 0; i < a.Count; i++)
			answer += Math.Abs(b[i] - a[i]);

		PartOneResult = $"Total distance between lists is {answer}";
		return true;
	}

	#endregion

	private (List<int> a, List<int> b) SplitIntoSeparateOrderedLists(List<List<int>> input)
	{
		//	If any list does not contain exactly 2 numbers, fail now as input does not match expectations
		if (input.Any(a => a.Count != 2))
			throw new ArgumentException("Data mismatch: lines exist without 2 numbers");

		//	Get the first set of numbers as a list in ascending order
		var orderedA = input.Select(s => s.First())
			.OrderBy(o => o)
			.ToList();

		//	Get the second set of numbers as a new list, again in ascending order
		var orderedB = input.Select(s => s.Last())
			.OrderBy(o => o)
			.ToList();

		//Should not happen, but check the lists have same number of entries
		if (orderedA.Count != orderedB.Count)
		{
			Console.WriteLine($"Mismatch in list lengths! A={orderedA.Count}, B={orderedB.Count}");
			throw new ArgumentOutOfRangeException("Mismatch in list lengths");
		}
		return (orderedA, orderedB);
	}
}