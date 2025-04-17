using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day18;

public partial class Day18
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
		var maze = new RamRun(71, 71);

		PartTwoResult = $"{ChallengeTitle} : first blocking coordinate = {GetBlockingCoordinate(InputFileLines, 1024, maze)}";
		return true;
	}

	/// <summary>
	/// Determines which coordinate being blocked results in a maze that cannot be solved
	/// </summary>
	/// <param name="input">The list of coords for corruption</param>
	/// <param name="startingByte">The initial number of coordinates to block</param>
	/// <param name="maze">The maze being solved</param>
	/// <returns>The coordinate that results in the blocking of the maze</returns>
	/// <exception cref="IndexOutOfRangeException"></exception>
	private Coordinate GetBlockingCoordinate(IEnumerable<string> input, int startingByte, RamRun maze)
	{
		var corruptedCoords = GetCoordinates(input);
		var stepCount = 0;
		var byteCount = startingByte;
		maze.LoadCorruption(corruptedCoords, byteCount);

		do
		{
			var coord = corruptedCoords[byteCount];
			byteCount++;
			if (byteCount > corruptedCoords.Count)
				break;
			maze.SetCorruptedCoordinate(coord);
			stepCount = maze.GetShortestPath();

		} while (stepCount != int.MaxValue);

		if (byteCount > corruptedCoords.Count)
			throw new IndexOutOfRangeException($"{byteCount} exceeds length of coordinate list");

		return corruptedCoords[byteCount - 1];
	}

	#endregion

	#region IPartTwoTestable implementation

	public void PartTwoTest()
	{
		var maze = new RamRun(7, 7);
		var expectedCoordinate = new Coordinate(1, 6);
		var coordinate = GetBlockingCoordinate(_partOneTestInput, 12, maze);
		Debug.Assert(coordinate.Equals(expectedCoordinate));
	}

	#endregion
}