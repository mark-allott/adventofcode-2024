using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day16;

public partial class Day16
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

		var maze = new ReindeerMaze();
		maze.Load(InputFileLines);
		long total = maze.DijkstraPathSolver();
		PartTwoResult = $"Reindeer Maze cells on path = {total}";
		return true;
	}

	#endregion

	#region IPartTwoTestable implementation

	public void PartTwoTest()
	{
		for (var i = 0; i < _partOneTestInput.Count; i++)
		{
			var maze = new ReindeerMaze();
			maze.Load(_partOneTestInput[i]);
			var cellsOnPath = maze.DijkstraPathSolver();
			Debug.Assert(cellsOnPath > 0);
			Debug.Assert(_partTwoExpectedScores[i] == cellsOnPath);
		}
	}

	private readonly List<int> _partTwoExpectedScores = new List<int>()
	{
		45, 64
	};

	#endregion
}