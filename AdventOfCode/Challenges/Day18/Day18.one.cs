using System.Diagnostics;
using AdventOfCode.Extensions;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day18;

public partial class Day18
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

		var corruptedCoords = GetCoordinates(InputFileLines);
		var maze = new RamRun(71, 71);
		maze.LoadCorruption(corruptedCoords, 1024);
		string output = $"{maze.GetShortestPath()}";
		PartOneResult = $"{ChallengeTitle} minimum steps = {output}";
		return true;
	}

	#endregion

	#region IPartOneTestable implementation

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	public void PartOneTest()
	{
		var corruptedCoords = GetCoordinates(_partOneTestInput);
		var maze = new RamRun(7, 7);
		maze.LoadCorruption(corruptedCoords, 12);
		Console.WriteLine(maze);

		var path = maze.GetShortestPath();
		Debug.Assert(22 == path);
	}

	private List<Coordinate> GetCoordinates(IEnumerable<string> input)
	{
		var coordinates = new List<Coordinate>();
		foreach (var line in input)
		{
			var values = line.ParseStringToListOfInt();
			coordinates.Add(new Coordinate(values[1], values[0]));
		}
		return coordinates;
	}

	private List<string> _partOneTestInput = new List<string>()
	{
		"5,4",
		"4,2",
		"4,5",
		"3,0",
		"2,1",
		"6,3",
		"2,4",
		"1,5",
		"0,6",
		"3,3",
		"2,6",
		"5,1",
		"1,2",
		"5,5",
		"2,5",
		"6,5",
		"1,4",
		"0,4",
		"6,4",
		"1,1",
		"6,1",
		"1,0",
		"0,5",
		"1,6",
		"2,0"
	};

	#endregion

	#region Methods

	#endregion
}