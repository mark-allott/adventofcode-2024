using System.Diagnostics;
using AdventOfCode.Comparers;
using AdventOfCode.Extensions;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day10;

public partial class Day10
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
		TopographicMap map = new TopographicMap(InputFileLines);
		var uniqueCounts = new Queue<int>();
		foreach (var position in map.StartPositions)
		{
			var routes = map.FindRoutes(position);
			var routesToTop = routes.Select(r => map.IsValidRouteToTop(r)).ToList();
			var distinctEndpoints = routes.Where(q => map.IsValidRouteToTop(q))
				.DistinctBy(r => r.LastPosition, new CoordinateEqualityComparer())
				.ToList();
			total += distinctEndpoints.Count;
		}

		PartOneResult = $"Trailhead score summation = {total}";
		return true;
	}

	#endregion

	#region IPartOneTestable implementation

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	public void PartOneTest()
	{
		var v = new List<List<int>>();
		var rowNumber = 0;
		foreach (var line in _partOneTestInput)
		{
			var lineValues = line.ParseStringToListOfInt(rowNumber);
			v.Add(lineValues);
			rowNumber++;
		}

		//	no ragged lists please!
		Debug.Assert(v.All(q => q.Count == v[0].Count));
		TopographicMap map = new TopographicMap(_partOneTestInput);

		Debug.Assert(9 == map.StartPositions.Count);

		var uniqueCounts = new Queue<int>();
		foreach (var position in map.StartPositions)
		{
			var routes = map.FindRoutes(position);
			var routesToTop = routes.Select(r => map.IsValidRouteToTop(r)).ToList();
			var distinctEndpoints = routes.Where(q => map.IsValidRouteToTop(q))
				.DistinctBy(r => r.LastPosition, new CoordinateEqualityComparer())
				.ToList();
			uniqueCounts.Enqueue(distinctEndpoints.Count);
		}

		Debug.Assert(_partOneTestScores.Count == uniqueCounts.Count);
		var summationScore = 0;
		foreach (var expectedCount in _partOneTestScores)
		{
			var actual = uniqueCounts.Dequeue();
			summationScore += actual;
			Debug.Assert(expectedCount == actual);
		}

		Debug.Assert(_partOneSummationScore == summationScore);
	}

	/// <summary>
	/// test data, as per challenge specifications
	/// </summary>
	private List<string> _partOneTestInput = new List<string>()
	{
		"89010123",
		"78121874",
		"87430965",
		"96549874",
		"45678903",
		"32019012",
		"01329801",
		"10456732"
	};

	/// <summary>
	/// From the test specifications, assert that the trailhead checksums work as expected
	/// </summary>
	private List<int> _partOneTestScores = new List<int>()
	{
		5, 6, 5, 3, 1, 3, 5, 3, 5
	};

	/// <summary>
	/// From the test specifications, assert that the summation value is correct
	/// </summary>
	private int _partOneSummationScore = 36;

	#endregion

	#region Part One code

	#endregion
}