using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day15;

public partial class Day15
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

		var (map, directions) = ParseInputToMapAndDirections(InputFileLines);

		var warehouse = new WarehouseWoeGrid();
		warehouse.LoadWarehouseDetail(map);
		warehouse.LoadRobotMovements(directions);

		warehouse.Move(0);
		long total = warehouse.SumOfGPS;
		PartOneResult = $"Sum of GPS coordinates = {total}";
		return true;
	}

	#endregion

	#region IPartOneTestable implementation

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	public void PartOneTest()
	{
		DoSmallTest();
		DoLargeTest();
	}

	private List<string> _partOneSmallTest = new List<string>()
	{
		"########",
		"#..O.O.#",
		"##@.O..#",
		"#...O..#",
		"#.#.O..#",
		"#...O..#",
		"#......#",
		"########",
		"",
		"<^^>>>vv<v>>v<<"
	};

	private void DoSmallTest()
	{
		var (map, directions) = ParseInputToMapAndDirections(_partOneSmallTest);

		Debug.Assert(map.Count == 8);
		Debug.Assert(directions.Count == 1);

		var warehouse = new WarehouseWoeGrid();
		warehouse.LoadWarehouseDetail(map);
		warehouse.LoadRobotMovements(directions);
		// Console.WriteLine(warehouse.ToString());
		// warehouse.Move();
		// Console.WriteLine(warehouse.ToString());
		// warehouse.Move();
		// Console.WriteLine(warehouse.ToString());

		warehouse.Move(0);
		Console.WriteLine(warehouse.ToString());
		Debug.Assert(2028 == warehouse.SumOfGPS);
	}

	private void DoLargeTest()
	{
		var (map, directions) = ParseInputToMapAndDirections(_partOneLargeTest);

		Debug.Assert(map.Count == 10);
		Debug.Assert(directions.Count == 10);

		var warehouse = new WarehouseWoeGrid();
		warehouse.LoadWarehouseDetail(map);
		warehouse.LoadRobotMovements(directions);
		Console.WriteLine(warehouse.ToString());

		warehouse.Move(0);
		Console.WriteLine(warehouse.ToString());
		Debug.Assert(10092 == warehouse.SumOfGPS);
	}

	private List<string> _partOneLargeTest = new List<string>()
	{
		"##########",
		"#..O..O.O#",
		"#......O.#",
		"#.OO..O.O#",
		"#..O@..O.#",
		"#O#..O...#",
		"#O..O..O.#",
		"#.OO.O.OO#",
		"#....O...#",
		"##########",
		"",
		"<vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^",
		"vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v",
		"><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<",
		"<<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^",
		"^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><",
		"^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^",
		">^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^",
		"<><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>",
		"^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>",
		"v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^",
	};

	#endregion

	#region Methods

	/// <summary>
	/// Helper method to take the whole of the input file and split into the different component parts
	/// </summary>
	/// <param name="input">The contents of the file</param>
	/// <returns>Separate list for map and directions, ready for further processing</returns>
	private (List<string> map, List<string> directions) ParseInputToMapAndDirections(IEnumerable<string> input)
	{
		ArgumentNullException.ThrowIfNull(input, nameof(input));

		//	Initialise stores
		var map = new List<string>();
		var directions = new List<string>();
		var readingMap = true;

		foreach (var line in input)
		{
			if (string.IsNullOrWhiteSpace(line))
			{
				readingMap = false;
				continue;
			}
			if (readingMap)
				map.Add(line);
			else
				directions.Add(line);
		}
		return (map, directions);
	}

	#endregion
}