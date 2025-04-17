using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day23;

public partial class Day23
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
		var lanParty = new LanParty();
		lanParty.LoadFromInput(InputFileLines);

		var result = lanParty.GetLanPartyPassword();
		PartTwoResult = $"{ChallengeTitle} password = {result}";
		return true;
	}

	#endregion

	#region IPartTwoTestable implementation

	public void PartTwoTest()
	{
		var sut = new LanParty();
		sut.LoadFromInput(_partOneTestInput);
		var actual = sut.GetLanPartyPassword();
		Debug.Assert("co,de,ka,ta" == actual);
	}

	#endregion

	#region Methods

	#endregion
}