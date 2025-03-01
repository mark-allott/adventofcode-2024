using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day12;

public partial class Day12
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

		long total = 0;
		PartOneResult = $"Cost of fencing = {total}";
		return true;
	}

	#endregion

	#region IPartOneTestable implementation

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	public void PartOneTest()
	{
		var inputCount = _partOneTestInput.Count;
		Debug.Assert(inputCount > 0);
		Debug.Assert(inputCount == _partOneExpectedCosts.Count);
	}

	/// <summary>
	/// test data, as per challenge specifications
	/// </summary>
	private List<List<string>> _partOneTestInput = new List<List<string>>()
	{
		new List<string>(){"AAAA","BBCD","BBCC","EEEC"},
		new List<string>(){"OOOOO","OXOXO","OOOOO","OXOXO","OOOOO"},
		new List<string>(){"RRRRIICCFF","RRRRIICCCF","VVRRRCCFFF","VVRCCCJFFF","VVVVCJJCFE","VVIVCCJJEE","VVIIICJJEE","MIIIIIJJEE","MIIISIJEEE","MMMISSJEEE"}
	};

	private List<int> _partOneExpectedCosts = new List<int>()
	{
		140, 772, 1930
	};

	#endregion

	#region Part One code

	#endregion
}