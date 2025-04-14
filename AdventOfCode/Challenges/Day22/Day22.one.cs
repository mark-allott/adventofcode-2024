using System.Diagnostics;
using AdventOfCode.Extensions;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day22;

public partial class Day22
	: AbstractDailyChallenge, IAutoRegister, IPartOneTestable
{
	#region Overrides to run part one of challenge

	/// <summary>
	/// Override the base implementation to provide the actual answer
	/// </summary>
	/// <returns>True if successful</returns>
	protected override bool PartOne()
	{
		LoadAndReadFile();
		//	Convert inputs to a list of ints
		var inputs = InputFileLines.ParseEnumerableOfStringToListOfInt();

		//	Grab a new instance of the model to do some work
		var monkeyMarket = new MonkeyMarket();

		//	Iterate over the inputs, find the 2000th value for the secret, then provide the sum of all values
		var result = inputs.Select(i=> monkeyMarket.MutateFor(i,2000)[2000]).Sum();
		PartOneResult = $"{ChallengeTitle} result = {result}";
		return true;
	}

	#endregion

	#region IPartOneTestable implementation

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	public void PartOneTest()
	{
		var sut = new MonkeyMarket();
		var result = sut.MutateFor(123, 10);

		for (var i = 0; i < _partOneExpectedSecrets.Count;)
		{
			Debug.Assert(_partOneExpectedSecrets[i] == result[++i]);
		}

		long sumOfResults = 0L;
		_partOneTestData.ForEach(i =>
		{
			result = sut.MutateFor(i.Item1, i.Item2);
			var actual = result[i.Item2];
			sumOfResults += actual;
			Debug.Assert(actual == i.Item3);
		});
		Debug.Assert(37327623 == sumOfResults);
	}

	/// <summary>
	/// Part one gives a simple check of for a given initial secret of 123, the next 10 secrets are:
	/// </summary>
	private List<long> _partOneExpectedSecrets = new List<long>()
	{
		15887950,
		16495136,
		527345,
		704524,
		1553684,
		12683156,
		11100544,
		12249484,
		7753432,
		5908254,
	};

	private List<(int, int, long)> _partOneTestData = new List<(int, int, long)>()
	{
		(1,2000,8685429),
		(10,2000,4700978),
		(100,2000,15273692),
		(2024,2000,8667524),
	};

	#endregion

	#region Methods

	#endregion
}