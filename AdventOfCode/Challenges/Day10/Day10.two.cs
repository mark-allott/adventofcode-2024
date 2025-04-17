using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day10;

public partial class Day10
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

		long total = 0;
		TopographicMap map = new TopographicMap(InputFileLines);
		var uniqueCounts = new Queue<int>();
		foreach (var position in map.StartPositions)
		{
			var routes = map.FindRoutes(position);
			var routesToTop = routes.Where(r => map.IsValidRouteToTop(r)).ToList();
			total += routesToTop.Count;
		}
		PartTwoResult = $"Trailhead rating summation = {total}";
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
		long total = 0;
		TopographicMap map = new TopographicMap(_partOneTestInput);
		var expectedCounts = new List<int>() { 20, 24, 10, 4, 1, 4, 5, 8, 5 };
		var expectedCountQueue = new Queue<int>();
		expectedCounts.ForEach(i => expectedCountQueue.Enqueue(i));

		foreach (var position in map.StartPositions)
		{
			var routes = map.FindRoutes(position);
			var routesToTop = routes.Where(r => map.IsValidRouteToTop(r)).ToList();
			var actual = routesToTop.Count;
			var expected = expectedCountQueue.Dequeue();

			Debug.Assert(expected == actual);
			total += actual;
		}

		Debug.Assert(expectedCounts.Sum() == total);
	}

	#endregion
}