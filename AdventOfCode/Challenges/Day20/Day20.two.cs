using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day20;

public partial class Day20
	: AbstractDailyChallenge, IAutoRegister, IPartTwoTestable
{
	#region Overrides to run part two of challenge

	/// <summary>
	/// Overrides the base implementation to provide the solution for part two
	/// </summary>
	/// <returns></returns>
	protected override bool PartTwo()
	{
		var maze = new RaceCondition();
		maze.Load(InputFileLines);

		var results = maze.GetShortcuts(20, new DiagonalMoveRangeStrategy());

		string output = $"{results.Where(r => r.Key >= 100).Sum(r => r.Value)}";
		PartTwoResult = $"{ChallengeTitle} cheats saving at least 100ps = {output}";
		return true;
	}

	#endregion

	#region IPartTwoTestable implementation

	public void PartTwoTest()
	{
		var maze = new RaceCondition();
		maze.Load(_partOneTestInput);

		var results = maze.GetShortcuts(20, new DiagonalMoveRangeStrategy())
			.Where(r => r.Key >= 50)
			.ToDictionary();

		Debug.Assert(results.Count == _partTwoExpectedShortcuts.Count);
		foreach (var shortcut in _partTwoExpectedShortcuts)
		{
			Debug.Assert(results.TryGetValue(shortcut.Key, out var shortcutResult));
			Debug.Assert(shortcutResult == shortcut.Value);
		}
	}

	/// <summary>
	/// Holds the details for the expected results from calculating the shortcuts
	/// </summary>
	private Dictionary<int, int> _partTwoExpectedShortcuts = new Dictionary<int, int>()
	{
		{50, 32},
		{52, 31},
		{54, 29},
		{56, 39},
		{58, 25},
		{60, 23},
		{62, 20},
		{64, 19},
		{66, 12},
		{68, 14},
		{70, 12},
		{72, 22},
		{74, 4},
		{76, 3}
	};

	#endregion
}