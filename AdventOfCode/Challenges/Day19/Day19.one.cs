using System.Diagnostics;
using AdventOfCode.Extensions;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day19;

public partial class Day19
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

		string output = $"N/A";
		PartOneResult = $"{ChallengeTitle} designs that are possible = {output}";
		return true;
	}

	#endregion

	#region IPartOneTestable implementation

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	public void PartOneTest()
	{
	}

	private List<string> _partOneTestInput = new List<string>()
	{
	};

	#endregion

	#region Methods

	#endregion
}