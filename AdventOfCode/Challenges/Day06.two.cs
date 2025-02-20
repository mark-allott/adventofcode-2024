using System.Diagnostics;
using AdventOfCode.Enums;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges;

public partial class Day06
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

		var total = GetLoopingSolutionCount(InputFileLines);
		PartTwoResult = $"The number of new obstruction locations is {total}";
		return true;
	}

	/// <summary>
	/// Helper method to work out how many possible locations need to be tested for placing obstructions
	/// </summary>
	/// <param name="grid">The grid to be tested</param>
	/// <returns></returns>
	private List<(int row, int column)> GetPossibleObstructionLocations(GuardPatrolGrid grid)
	{
		var locations = new List<(int row, int column)>();

		for (var row = grid.PatrolHeight.min; row <= grid.PatrolHeight.max; row++)
		{
			for (var column = grid.PatrolWidth.min; column <= grid.PatrolWidth.max; column++)
			{
				if (grid.GetCellState(row, column) == CellState.Unvisited)
					locations.Add((row, column));
			}
		}
		return locations;
	}

	private int GetLoopingSolutionCount(List<string> input)
	{
		//	Initialise grid with given grid
		var grid = new GuardPatrolGrid();
		grid.LoadGrid(input);

		//	Work out how many locations are needing to be tested
		var possibleLocations = GetPossibleObstructionLocations(grid);
		//	Stores the locations that cause a guard to be stuck in a loop
		var loopingLocations = new List<(int row, int column)>();

		//	Attempt to solve each possible location
		foreach (var possibleLocation in possibleLocations)
		{
			//	force a reload of the original grid layout
			grid.LoadGrid(input);

			//	try to set the location of the new obstuction
			if (!grid.SetNewObstruction(possibleLocation.row, possibleLocation.column))
				continue;

			var guard = new GuardOnPatrol();
			if (guard.PatrolStuckInLoop(grid))
				loopingLocations.Add(possibleLocation);
		}

		return loopingLocations.Count;
	}

	#endregion

	#region Test data for part two

	/// <summary>
	/// Using rules set out in the challenge, run some tests to make sure the
	/// code behaves as expected
	/// </summary>
	public void PartTwoTest()
	{
		var loopingSolutionCount = GetLoopingSolutionCount(testInput);

		Debug.Assert(6 == loopingSolutionCount);
	}

	#endregion
}