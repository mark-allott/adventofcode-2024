using System.Diagnostics;
using AdventOfCode.Extensions;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day22;

public partial class Day22
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
		//	Convert inputs to a list of ints
		var inputs = InputFileLines.ParseEnumerableOfStringToListOfInt();

		//	Grab a new instance of the model to do some work
		var monkeyMarket = new MonkeyMarket();
		var sequences = inputs.Select(i => monkeyMarket.MutateWithSequencesFor(i, 2000))
			.ToList();
		var result = CalculateBananasWon(sequences);
		PartTwoResult = $"{ChallengeTitle} bananas won = {result}";
		return true;
	}

	#endregion

	#region IPartTwoTestable implementation

	public void PartTwoTest()
	{
		var sut = new MonkeyMarket();
		var actual = sut.MutateWithChangesFor(123, 10);

		foreach (var expected in _partTwoTestData)
		{
			var actualSecret = actual[expected.Key].value;
			var actualPrice = actual[expected.Key].price;
			var actualChange = actual[expected.Key].change;
			Debug.Assert(expected.Value.secret == actualSecret);
			Debug.Assert(expected.Value.price == actualPrice);
			Debug.Assert(expected.Value.change == actualChange);
		}
		var sequences = _partTwoTestInput.Select(i => sut.MutateWithSequencesFor(i, 2000))
			.ToList();
		var bananas = CalculateBananasWon(sequences);
		Debug.Assert(23 == bananas);
	}

	private Dictionary<int, (long secret, int price, int change)> _partTwoTestData = new Dictionary<int, (long, int, int)>()
	{
		{ 0, (123, 3, 0) },
		{ 1, (15887950, 0 ,-3) },
		{ 2, (16495136, 6, 6) },
		{ 3, (527345, 5, -1) },
		{ 4, (704524, 4, -1) },
		{ 5, (1553684, 4, 0) },
		{ 6, (12683156, 6, 2) },
		{ 7, (11100544, 4, -2) },
		{ 8, (12249484, 4, 0) },
		{ 9, (7753432, 2, -2)}
	};

	private List<int> _partTwoTestInput = new List<int>()
	{
		1,
		2,
		3,
		2024
	};

	#endregion

	#region Methods

	/// <summary>
	/// Calculates the greatest number of bananas that can be gained from the given <paramref name="buyerSequencePrices"/>
	/// </summary>
	/// <param name="buyerSequencePrices">A list of dictionary objects containing sequences of changes and bananas being offered for that sequence</param>
	/// <returns>The maxmimum number of bananas that can be obtained</returns>
	private int CalculateBananasWon(List<Dictionary<string, int>> buyerSequencePrices)
	{
		return buyerSequencePrices.SelectMany(m => m.Keys)
			.Distinct()
			.Select(s => buyerSequencePrices.Select(bsp => bsp.TryGetValue(s, out var price) ? price : 0).Sum())
			.Max();
	}

	#endregion
}