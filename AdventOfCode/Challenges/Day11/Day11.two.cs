using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day11;

public partial class Day11
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

		var pebbleLine = new PlutonianPebbleLineEx(InputFileLines[0]);
		long total = pebbleLine.Blink(75);
		PartTwoResult = $"Number of pebbles = {total}";
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
		//	Using the next line class, perform same checks as part one to ensure we have same result
		//	Check after 6 rounds
		var pebbleLine = new PlutonianPebbleLineEx(_partOneTestInput2);
		var pebbleCount = pebbleLine.Blink(6);
		Debug.Assert(22 == pebbleCount);

		//	Check after 25 rounds
		pebbleCount = pebbleLine.Blink(25);
		Debug.Assert(55312 == pebbleCount);
	}

	#endregion
}