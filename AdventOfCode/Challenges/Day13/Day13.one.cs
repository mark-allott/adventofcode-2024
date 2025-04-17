using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day13;

public partial class Day13
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

		long total = 0;
		var contraptions = GetContraptionDetail(InputFileLines);

		for (var i = 0; i < contraptions.Count; i++)
		{
			var (a, b, p) = contraptions[i];
			var solver = new ClawContraptionSolver(a, b, p);
			if (solver.Solve())
				total += solver.GetCost(3, 1);
		}
		PartOneResult = $"Number of tokens = {total}";
		return true;
	}

	#endregion

	#region IPartOneTestable implementation

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	public void PartOneTest()
	{
		var contraptions = GetContraptionDetail(_partOneTestInput);
		Debug.Assert(4 == contraptions.Count);

		for (var i = 0; i < contraptions.Count; i++)
		{
			var (a, b, p) = contraptions[i];
			var (isSolveable, aCount, bCount, tokens) = _partOneExpectedResults[i];
			var solver = new ClawContraptionSolver(a, b, p);
			Debug.Assert(isSolveable == solver.Solve());
			if (isSolveable)
			{
				Debug.Assert(aCount == solver.ValueForA);
				Debug.Assert(bCount == solver.ValueForB);
				Debug.Assert(tokens == solver.GetCost(3, 1));
			}
		}
	}

	private List<string> _partOneTestInput = new List<string>()
	{
		"Button A: X+94, Y+34","Button B: X+22, Y+67","Prize: X=8400, Y=5400",
		"",
		"Button A: X+26, Y+66","Button B: X+67, Y+21","Prize: X=12748, Y=12176",
		"",
		"Button A: X+17, Y+86","Button B: X+84, Y+37","Prize: X=7870, Y=6450",
		"",
		"Button A: X+69, Y+23","Button B: X+27, Y+71","Prize: X=18641, Y=10279"
	};

	private List<(bool isSolveable, int aCount, int bCount, int tokens)> _partOneExpectedResults = new List<(bool, int, int, int)>()
	{
		(true, 80, 40, 280),
		(false, 0, 0, 0),
		(true, 38, 86, 200),
		(false, 0, 0, 0)
	};

	#endregion

	/// <summary>
	/// Helper method to extract details of the contraptions from the <paramref name="input"/>
	/// </summary>
	/// <param name="input">The list of lines to be scanned for contraption details</param>
	/// <returns>A list of string tuples that represent the contraption detail</returns>
	private List<(string line1, string line2, string line3)> GetContraptionDetail(IEnumerable<string> input)
	{
		ArgumentNullException.ThrowIfNull(input);

		var contraptions = new List<(string, string, string)>();
		var paramQueue = new Queue<string>();

		//	Loop for each line found
		foreach (var line in input)
		{
			//	If a newline is found, skip and move to next
			if (string.IsNullOrWhiteSpace(line))
				continue;

			//	Add the line found to the param queue
			paramQueue.Enqueue(line);

			//	When 3 params found, add to the list of contraptions found
			if (paramQueue.Count == 3)
				contraptions.Add((paramQueue.Dequeue(), paramQueue.Dequeue(), paramQueue.Dequeue()));
		}
		return contraptions;
	}
}