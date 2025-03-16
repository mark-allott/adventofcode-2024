using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day16;

public partial class Day16
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

		var maze = new ReindeerMaze();
		maze.Load(InputFileLines);
		long total = maze.DijkstraSolver();
		// long total = 0;
		PartOneResult = $"Reindeer Maze lowest score = {total}";
		return true;
	}

	#endregion

	#region IPartOneTestable implementation

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	public void PartOneTest()
	{
		for (var i = 0; i < _partOneTestInput.Count; i++)
		{
			var maze = new ReindeerMaze();
			maze.Load(_partOneTestInput[i]);
			var minScore = maze.DijkstraSolver();
			Debug.Assert(minScore > 0);
			Debug.Assert(_partOneExpectedScores[i] == minScore);
		}
	}

	private List<List<string>> _partOneTestInput = new List<List<string>>()
	{
		new List<string>()
		{
			"###############",
			"#.......#....E#",
			"#.#.###.#.###.#",
			"#.....#.#...#.#",
			"#.###.#####.#.#",
			"#.#.#.......#.#",
			"#.#.#####.###.#",
			"#...........#.#",
			"###.#.#####.#.#",
			"#...#.....#.#.#",
			"#.#.#.###.#.#.#",
			"#.....#...#.#.#",
			"#.###.#.#.#.#.#",
			"#S..#.....#...#",
			"###############"
		},
		new List<string>()
		{
			"#################",
			"#...#...#...#..E#",
			"#.#.#.#.#.#.#.#.#",
			"#.#.#.#...#...#.#",
			"#.#.#.#.###.#.#.#",
			"#...#.#.#.....#.#",
			"#.#.#.#.#.#####.#",
			"#.#...#.#.#.....#",
			"#.#.#####.#.###.#",
			"#.#.#.......#...#",
			"#.#.###.#####.###",
			"#.#.#...#.....#.#",
			"#.#.#.#####.###.#",
			"#.#.#.........#.#",
			"#.#.#.#########.#",
			"#S#.............#",
			"#################"
		}
	};

	private List<int> _partOneExpectedScores = new List<int>()
	{
		7036, 11048
	};

	#endregion

	#region Methods

	#endregion
}