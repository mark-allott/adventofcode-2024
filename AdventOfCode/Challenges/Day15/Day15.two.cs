using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day15;

public partial class Day15
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

		var (map, directions) = ParseInputToMapAndDirections(InputFileLines);

		var warehouse = new WarehouseWoeGrid();
		warehouse.LoadWarehouseDetail(map, 2);
		warehouse.LoadRobotMovements(directions);

		warehouse.Move(0, 2);
		long total = warehouse.SumOfGPS;
		PartTwoResult = $"Sum of GPS coordinates = {total}";
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
		var (map, directions) = ParseInputToMapAndDirections(_partTwoTestInput);

		Debug.Assert(map.Count == 7);
		Debug.Assert(directions.Count == 1);

		var warehouse = new WarehouseWoeGrid();
		warehouse.LoadWarehouseDetail(map, 2);
		warehouse.LoadRobotMovements(directions);
		warehouse.Move(0, 2);
		Console.WriteLine(warehouse.ToString());

		//	Repeat large test from part one
		(map, directions) = ParseInputToMapAndDirections(_partOneLargeTest);
		warehouse = new WarehouseWoeGrid();
		warehouse.LoadWarehouseDetail(map, 2);
		warehouse.LoadRobotMovements(directions);
		Console.WriteLine(warehouse.ToString());
		warehouse.Move(0, 2);
		Console.WriteLine(warehouse.ToString());
		Debug.Assert(9021 == warehouse.SumOfGPS);
	}

	/// <summary>
	/// test data, as per challenge specifications
	/// </summary>
	private List<string> _partTwoTestInput = new List<string>()
	{
		"#######",
		"#...#.#",
		"#.....#",
		"#..OO@#",
		"#..O..#",
		"#.....#",
		"#######",
		"",
		"<vv<<^^<<^^"
	};

	#endregion
}
