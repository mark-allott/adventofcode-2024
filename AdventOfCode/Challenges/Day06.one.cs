using System.Diagnostics;
using AdventOfCode.Enums;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges;

public partial class Day06
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

		var grid = new GuardPatrolGrid();
		grid.LoadGrid(InputFileLines);

		var guard = new GuardOnPatrol();
		guard.Patrol(grid);

		var total = grid.GetVisitedCellCount();
		PartOneResult = $"Total number of moves = {total}";
		return true;
	}

	#endregion

	#region IPartOneTestable implementation

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	public void PartOneTest()
	{
		ValidateConversion(testInput, expectedStartState);
		ValidateConversion(expected, expectedEndStates);

		var grid = new GuardPatrolGrid();
		grid.LoadGrid(testInput);

		var guard = new GuardOnPatrol();
		guard.Patrol(grid);

		var total = grid.GetVisitedCellCount();
		Debug.Assert(41 == total);
	}

	/// <summary>
	/// Test data, as provided on the challenge website
	/// </summary>
	private readonly List<string> testInput = new List<string>()
	{
		"....#.....",
		".........#",
		"..........",
		"..#.......",
		".......#..",
		"..........",
		".#..^.....",
		"........#.",
		"#.........",
		"......#..."
	};

	/// <summary>
	/// Expected states after conversion from characters to enum states for the initial test solution
	/// </summary>
	private readonly List<List<CellState>> expectedStartState = new List<List<CellState>>()
	{
		new List<CellState>(){ CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Obstructed,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited},
		new List<CellState>(){ CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Obstructed},
		new List<CellState>(){ CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited},
		new List<CellState>(){ CellState.Unvisited,CellState.Unvisited,CellState.Obstructed,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited},
		new List<CellState>(){ CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Obstructed,CellState.Unvisited,CellState.Unvisited},
		new List<CellState>(){ CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited},
		new List<CellState>(){ CellState.Unvisited,CellState.Obstructed,CellState.Unvisited,CellState.Unvisited,CellState.CurrentPosition,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited},
		new List<CellState>(){ CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Obstructed,CellState.Unvisited},
		new List<CellState>(){ CellState.Obstructed,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited},
		new List<CellState>(){ CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Obstructed,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited},
	};

	/// <summary>
	/// Expected states for the solution (as specified on challenge website)
	/// </summary>
	private readonly List<string> expected = new List<string>()
	{
		"....#.....",
		"....XXXXX#",
		"....X...X.",
		"..#.X...X.",
		"..XXXXX#X.",
		"..X.X.X.X.",
		".#XXXXXXX.",
		".XXXXXXX#.",
		"#XXXXXXX..",
		"......#X..",
	};

	/// <summary>
	/// Expected states after conversion from characters to enum states for the test solution
	/// </summary>
	private readonly List<List<CellState>> expectedEndStates = new List<List<CellState>>()
	{
		new List<CellState>(){ CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Obstructed,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited},
		new List<CellState>(){ CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Obstructed},
		new List<CellState>(){ CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Visited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Visited,CellState.Unvisited},
		new List<CellState>(){ CellState.Unvisited,CellState.Unvisited,CellState.Obstructed,CellState.Unvisited,CellState.Visited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Visited,CellState.Unvisited},
		new List<CellState>(){ CellState.Unvisited,CellState.Unvisited,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Obstructed,CellState.Visited,CellState.Unvisited},
		new List<CellState>(){ CellState.Unvisited,CellState.Unvisited,CellState.Visited,CellState.Unvisited,CellState.Visited,CellState.Unvisited,CellState.Visited,CellState.Unvisited,CellState.Visited,CellState.Unvisited},
		new List<CellState>(){ CellState.Unvisited,CellState.Obstructed,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Unvisited},
		new List<CellState>(){ CellState.Unvisited,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Obstructed,CellState.Unvisited},
		new List<CellState>(){ CellState.Obstructed,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Visited,CellState.Unvisited,CellState.Unvisited},
		new List<CellState>(){ CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Unvisited,CellState.Obstructed,CellState.Visited,CellState.Unvisited,CellState.Unvisited},
	};

	private void ValidateConversion(List<string> input, List<List<CellState>> expectedStates)
	{
		var grid = new GuardPatrolGrid();
		grid.LoadGrid(input);

		for (var row = 0; row < expectedStates.Count; row++)
		{
			for (var col = 0; col < expectedStates[row].Count; col++)
			{
				var state = grid.GetCellState(row, col);
				var expectedState = expectedStates[row][col];
				Debug.Assert(expectedState == state);
			}
		}
	}

	#endregion
}