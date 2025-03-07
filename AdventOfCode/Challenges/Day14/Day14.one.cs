using System.Diagnostics;
using System.Text.RegularExpressions;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day14;

public partial class Day14
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

		var grid = new SecurityBotGrid(101, 103);
		foreach (var line in InputFileLines)
		{
			var (x, y, dx, dy) = GetCoords(line);
			grid.AddBot(x, y, dx, dy);
		}
		grid.Move(100);
		long total = grid.SafetyFactor;
		PartOneResult = $"Safety Factor = {total}";
		return true;
	}

	#endregion

	#region IPartOneTestable implementation

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	public void PartOneTest()
	{
		var grid = new SecurityBotGrid(11, 7);
		SecurityBot testBot = null!;
		foreach (var line in _partOneTestInput)
		{
			var (x, y, dx, dy) = GetCoords(line);
			var bot = grid.AddBot(x, y, dx, dy);

			if (bot.StartPosition.X == 2 && bot.StartPosition.Y == 4 &&
				bot.Velocity.X == 2 && bot.Velocity.Y == -3)
				testBot = bot;
		}

		Debug.Assert(12 == grid.BotCount);
		Debug.Assert(testBot is not null);

		testBot.Move();
		Debug.Assert(4 == testBot.CurrentPosition.X);
		Debug.Assert(1 == testBot.CurrentPosition.Y);

		testBot.Move();
		Debug.Assert(6 == testBot.CurrentPosition.X);
		Debug.Assert(5 == testBot.CurrentPosition.Y);

		testBot.Move();
		Debug.Assert(8 == testBot.CurrentPosition.X);
		Debug.Assert(2 == testBot.CurrentPosition.Y);

		testBot.Move();
		Debug.Assert(10 == testBot.CurrentPosition.X);
		Debug.Assert(6 == testBot.CurrentPosition.Y);

		testBot.Move();
		Debug.Assert(1 == testBot.CurrentPosition.X);
		Debug.Assert(3 == testBot.CurrentPosition.Y);

		grid.Reset();
		grid.Move(100);
		Debug.Assert(12 == grid.SafetyFactor);
	}

	private List<string> _partOneTestInput = new List<string>()
	{
		"p=0,4 v=3,-3",
		"p=6,3 v=-1,-3",
		"p=10,3 v=-1,2",
		"p=2,0 v=2,-1",
		"p=0,0 v=1,3",
		"p=3,0 v=-2,-2",
		"p=7,6 v=-1,-3",
		"p=3,0 v=-1,-2",
		"p=9,3 v=2,3",
		"p=7,3 v=-1,2",
		"p=2,4 v=2,-3",
		"p=9,5 v=-3,-3"
	};

	#endregion

	#region Methods

	[GeneratedRegex(@"=(-?\d+),(-?\d+)", RegexOptions.Compiled)]
	private static partial Regex CoordsRegex();

	private (int x, int y, int dx, int dy) GetCoords(string input)
	{
		if (string.IsNullOrWhiteSpace(input))
			return (0, 0, 0, 0);

		var parts = input.Split(' ');
		Debug.Assert(2 == parts.Length);

		var nr = CoordsRegex();
		var locMatch = nr.Match(parts[0]);
		var velMatch = nr.Match(parts[1]);

		if (!int.TryParse(locMatch.Groups[1].Value, out var x))
			throw new ArgumentException($"Cannot parse '{locMatch.Groups[1].Value}'", nameof(input));
		if (!int.TryParse(locMatch.Groups[2].Value, out var y))
			throw new ArgumentException($"Cannot parse '{locMatch.Groups[2].Value}'", nameof(input));
		if (!int.TryParse(velMatch.Groups[1].Value, out var dx))
			throw new ArgumentException($"Cannot parse '{velMatch.Groups[1].Value}'", nameof(input));
		if (!int.TryParse(velMatch.Groups[2].Value, out var dy))
			throw new ArgumentException($"Cannot parse '{velMatch.Groups[2].Value}'", nameof(input));

		return (x, y, dx, dy);
	}

	#endregion
}