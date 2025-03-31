using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day20;

public partial class Day20
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

		var maze = new RaceCondition();
		maze.Load(InputFileLines);

		var results = maze.GetShortcuts(2, new CardinalRangeStrategy());

		string output = $"{results.Where(r => r.Key >= 100).Sum(r => r.Value)}";
		PartOneResult = $"{ChallengeTitle} cheats saving at least 100ps = {output}";
		return true;
	}

	#endregion

	#region IPartOneTestable implementation

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	public void PartOneTest()
	{
		//	Verify the maze solution is 84 steps without any shortcuts/cheats
		var maze = new RaceCondition();
		maze.Load(_partOneTestInput);
		var minScore = maze.DijkstraSolver();
		Debug.Assert(minScore > 0);
		Debug.Assert(minScore != int.MaxValue);
		Debug.Assert(84 == minScore);

		var results = maze.GetShortcuts(2, new CardinalRangeStrategy());

		Debug.Assert(results.Count == _partOneExpectedShortcuts.Count);
		foreach (var shortcut in _partOneExpectedShortcuts)
		{
			Debug.Assert(results.TryGetValue(shortcut.Key, out var shortcutResult));
			Debug.Assert(shortcutResult == shortcut.Value);
		}
	}

	private List<string> _partOneTestInput = new List<string>()
	{
		"###############",
		"#...#...#.....#",
		"#.#.#.#.#.###.#",
		"#S#...#.#.#...#",
		"#######.#.#.###",
		"#######.#.#...#",
		"#######.#.###.#",
		"###..E#...#...#",
		"###.#######.###",
		"#...###...#...#",
		"#.#####.#.###.#",
		"#.#...#.#.#...#",
		"#.#.#.#.#.#.###",
		"#...#...#...###",
		"###############"
	};

	/// <summary>
	/// Holds the details for the expected results from calculating the shortcuts
	/// </summary>
	private Dictionary<int, int> _partOneExpectedShortcuts = new Dictionary<int, int>()
	{
		{2, 14},
		{4, 14},
		{6, 2},
		{8, 4},
		{10, 2},
		{12, 3},
		{20, 1},
		{36, 1},
		{38, 1},
		{40, 1},
		{64, 1},
	};

	#endregion

	#region Methods

	#endregion
}