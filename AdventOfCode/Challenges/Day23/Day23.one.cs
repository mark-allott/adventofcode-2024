using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day23;

public partial class Day23
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
		var lanParty = new LanParty();
		lanParty.LoadFromInput(InputFileLines);
		var triplets = lanParty.GetTripletLinksWithComputerNamesStartingWith('t');
		var result = $"{triplets.Count}";
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
		var sut = new LanParty();
		sut.LoadFromInput(_partOneTestInput);
		var triplets = sut.GetTripletLinks();

		for (var i = 0; i < _partOneExpectedTriplets.Count; i++)
			Debug.Assert(_partOneExpectedTriplets[i] == triplets[i]);

		triplets = sut.GetTripletLinksWithComputerNamesStartingWith('t');
		for (var i = 0; i < _partOneExpectedResult.Count; i++)
			Debug.Assert(_partOneExpectedResult[i] == triplets[i]);
	}

	private List<string> _partOneTestInput = new List<string>()
	{
		"kh-tc",
		"qp-kh",
		"de-cg",
		"ka-co",
		"yn-aq",
		"qp-ub",
		"cg-tb",
		"vc-aq",
		"tb-ka",
		"wh-tc",
		"yn-cg",
		"kh-ub",
		"ta-co",
		"de-co",
		"tc-td",
		"tb-wq",
		"wh-td",
		"ta-ka",
		"td-qp",
		"aq-cg",
		"wq-ub",
		"ub-vc",
		"de-ta",
		"wq-aq",
		"wq-vc",
		"wh-yn",
		"ka-de",
		"kh-ta",
		"co-tc",
		"wh-qp",
		"tb-vc",
		"td-yn"
	};

	private List<string> _partOneExpectedTriplets = new List<string>()
	{
		"aq,cg,yn",
		"aq,vc,wq",
		"co,de,ka",
		"co,de,ta",
		"co,ka,ta",
		"de,ka,ta",
		"kh,qp,ub",
		"qp,td,wh",
		"tb,vc,wq",
		"tc,td,wh",
		"td,wh,yn",
		"ub,vc,wq"
	};

	private List<string> _partOneExpectedResult = new List<string>()
	{
		"co,de,ta",
		"co,ka,ta",
		"de,ka,ta",
		"qp,td,wh",
		"tb,vc,wq",
		"tc,td,wh",
		"td,wh,yn"
	};

	#endregion

	#region Methods

	#endregion
}