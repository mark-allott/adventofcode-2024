using AdventOfCode.Interfaces;

namespace AdventOfCode.Challenges.Day17;

public partial class Day17
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

		string output = "";
		PartOneResult = $"{ChallengeTitle} outputs = {output}";
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
			"Register A: 729",
			"Register B: 0",
			"Register C: 0",
			"",
			"Program: 0,1,5,4,3,0"
	};

	private List<string> _partOneExpectedOutput = new List<string>()
	{
		"4,6,3,5,6,3,5,2,1,0"
	};

	#endregion

	#region Methods

	#endregion
}