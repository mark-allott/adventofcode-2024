using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day21;

public partial class Day21
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

		var complexityTotal = 0L;
		var solver = new KeypadConundrum();
		solver.SetupKeypads(_keypad, _arrowKeys, ' ');

		foreach (var code in InputFileLines)
		{
			var (keypresses, complexity) = solver.GetSolution(code, 2);
			var c2 = solver.GetComplexity(code, 2);
			Debug.Assert(c2 == complexity);
			complexityTotal += complexity;
		}
		PartOneResult = $"{ChallengeTitle} complexity = {complexityTotal}";
		return true;
	}

	#endregion

	#region IPartOneTestable implementation

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	public void PartOneTest()
	{
		var sut = new KeypadConundrum();
		sut.SetupKeypads(_keypad, _arrowKeys, ' ');

		foreach (var code in _partOneTestInput)
		{
			// (keypresses, complexity) = sut.GetSolution(code, 0);
			// Console.WriteLine($"{code}/0 => {keypresses}");
			// (keypresses, complexity) = sut.GetSolution(code, 1);
			// Console.WriteLine($"{code}/1 => {keypresses}");
			var (keypresses, complexity) = sut.GetSolution(code, 2);
			// Console.WriteLine($"{code}/2 => {keypresses}");
			var expectedKeypresses = _partOneExpectedKeypresses[code];
			var expectedComplexity = _partOneExpectedComplexities[code];
			Debug.Assert(keypresses.Length == expectedKeypresses.Length);
			Debug.Assert(complexity == expectedComplexity);
		}
	}

	private List<string> _keypad = new List<string>()
	{
		"789",
		"456",
		"123",
		" 0A"
	};

	private List<string> _arrowKeys = new List<string>()
	{
		" ^A",
		"<v>"
	};

	private List<string> _partOneTestInput = new List<string>()
	{
		"029A",
		"980A",
		"179A",
		"456A",
		"379A"
	};

	private Dictionary<string, string> _partOneExpectedKeypresses = new Dictionary<string, string>()
	{
		{"029A", "<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A"},
		{"980A", "<v<A>>^AAAvA^A<vA<AA>>^AvAA<^A>A<v<A>A>^AAAvA<^A>A<vA>^A<A>A"},
		{"179A", "<v<A>>^A<vA<A>>^AAvAA<^A>A<v<A>>^AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A"},
		{"456A", "<v<A>>^AA<vA<A>>^AAvAA<^A>A<vA>^A<A>A<vA>^A<A>A<v<A>A>^AAvA<^A>A"},
		{"379A", "<v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A"},
	};

	private Dictionary<string, long> _partOneExpectedComplexities = new Dictionary<string, long>()
	{
		{"029A", 68*29},
		{"980A", 60*980},
		{"179A", 68*179},
		{"456A", 64*456},
		{"379A", 64*379},
	};

	#endregion

	#region Methods

	#endregion
}