using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day12;

public partial class Day12
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

		var garden = new Garden(InputFileLines);
		garden.SetRegions();
		garden.SetFences();

		long total = garden.Regions.Sum(r => r.BulkCost);
		PartTwoResult = $"Cost of fencing = {total}";
		return true;
	}

	#endregion

	#region Test data for part two

	/// <summary>
	/// Using rules set out in the challenge, run some tests to make sure the
	/// code behaves as expected
	/// </summary>
	public void PartTwoTest()
	{
		var inputCount = _partTwoTestInput.Count;
		Debug.Assert(inputCount > 0);
		Debug.Assert(inputCount == _partTwoExpectedCosts.Count);

		for (var i = 0; i < inputCount; i++)
		{
			var input = _partTwoTestInput[i];

			var garden = new Garden(input);
			garden.SetRegions();
			garden.SetFences();

			var regionCount = garden.Regions.Count;

			Debug.Assert(_partTwoExpectedRegions[i] == regionCount);

			var expectedAreas = _partTwoExpectedAreas[i];
			var expectedSides = _partTwoExpectedSides[i];
			var expectedCost = _partTwoExpectedCosts[i];

			for (var r = 0; r < garden.Regions.Count; r++)
			{
				var region = garden.Regions[r];
				Debug.Assert(expectedAreas[r] == region.Area);
				Debug.Assert(expectedSides[r] == region.Sides);
			}
			Debug.Assert(expectedCost == garden.Regions.Sum(r => r.BulkCost));
		}
	}

	/// <summary>
	/// test data, as per challenge specifications
	/// </summary>
	private List<List<string>> _partTwoTestInput = new List<List<string>>()
	{
		new List<string>(){"AAAA","BBCD","BBCC","EEEC"},
		new List<string>(){"OOOOO","OXOXO","OOOOO","OXOXO","OOOOO"},
		new List<string>(){"EEEEE","EXXXX","EEEEE","EXXXX","EEEEE"},
		new List<string>(){"AAAAAA","AAABBA","AAABBA","ABBAAA","ABBAAA","AAAAAA"},
		new List<string>(){"RRRRIICCFF","RRRRIICCCF","VVRRRCCFFF","VVRCCCJFFF","VVVVCJJCFE","VVIVCCJJEE","VVIIICJJEE","MIIIIIJJEE","MIIISIJEEE","MMMISSJEEE"}
	};

	private List<int> _partTwoExpectedRegions = new List<int>()
	{
		5, 5, 3, 3, 11
	};

	private List<List<int>> _partTwoExpectedAreas = new List<List<int>>()
	{
		new(){ 4, 4, 4, 1, 3 },
		new(){ 21, 1, 1, 1, 1 },
		new(){ 17, 4, 4 },
		new(){ 28, 4, 4 },
		new(){ 12, 4, 14, 10, 13, 11, 1, 13, 14, 5, 3 }
	};

	private List<List<int>> _partTwoExpectedSides = new List<List<int>>()
	{
		new(){ 4, 4, 8, 4, 4 },
		new(){ 20, 4, 4, 4, 4 },
		new(){ 12, 4, 4 },
		new(){ 12, 4, 4 },
		new(){ 10, 4, 22, 12, 10, 12, 4, 8, 16, 6, 6 }
	};

	private List<int> _partTwoExpectedCosts = new List<int>()
	{
		80, 436, 236, 368, 1206
	};

	#endregion
}
