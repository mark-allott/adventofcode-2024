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

		var garden = new Garden(InputFileLines);
		garden.SetRegions();
		garden.SetFences();

		long total = garden.Regions.Sum(r => r.Cost);
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
		Debug.Assert(inputCount == _partOneExpectedRegions.Count);
		Debug.Assert(inputCount == _partOneExpectedAreas.Count);
		Debug.Assert(inputCount == _partOneExpectedPerimeters.Count);
		Debug.Assert(inputCount == _partOneExpectedCosts.Count);

		for (var i = 0; i < inputCount; i++)
		{
			var input = _partOneTestInput[i];

			var garden = new Garden(input);
			garden.SetRegions();
			garden.SetFences();

			var regionCount = garden.Regions.Count;

			Debug.Assert(_partOneExpectedRegions[i] == regionCount);

			var expectedAreas = _partOneExpectedAreas[i];
			var expectedPerimeters = _partOneExpectedPerimeters[i];
			var expectedCost = _partOneExpectedCosts[i];

			Debug.Assert(expectedAreas.Count == regionCount);
			Debug.Assert(expectedPerimeters.Count == regionCount);

			for (var r = 0; r < garden.Regions.Count; r++)
			{
				var region = garden.Regions[r];
				Debug.Assert(expectedAreas[r] == region.Area);
				Debug.Assert(expectedPerimeters[r] == region.Perimeter);
			}
			Debug.Assert(expectedCost == garden.Regions.Sum(r => r.Cost));
		}
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

	private List<int> _partOneExpectedRegions = new List<int>()
	{
		5, 5, 11
	};

	private List<List<int>> _partOneExpectedAreas = new List<List<int>>()
	{
		new(){ 4, 4, 4, 1, 3 },
		new(){ 21, 1, 1, 1, 1 },
		new(){ 12, 4, 14, 10, 13, 11, 1, 13, 14, 5, 3 }
	};

	private List<List<int>> _partOneExpectedPerimeters = new List<List<int>>()
	{
		new(){ 10, 8, 10, 4, 8 },
		new(){ 36, 4, 4, 4, 4 },
		new(){ 18, 8, 28, 18, 20, 20, 4, 18, 22, 12, 8 }
	};

	private List<int> _partOneExpectedCosts = new List<int>()
	{
		140, 772, 1930
	};

	#endregion

	#region Part One code

	#endregion
}