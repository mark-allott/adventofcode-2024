using System.Diagnostics;
using AdventOfCode.Enums;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges;

public partial class Day07
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

		CalibrationOperator[] operators = new[] { CalibrationOperator.Add, CalibrationOperator.Multiply, CalibrationOperator.Concatenate };
		var solutions = FindSolutions(InputFileLines, operators);
		long total = 0;
		solutions.ForEach(s => total += s.Value);
		PartTwoResult = $"Total calibration result = {total}";
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
		var c1 = new CalibrationProblem(15, 6, CalibrationOperator.Concatenate);
		Debug.Assert(c1.Value == 156);
		Debug.Assert("15 || 6" == c1.ToString());

		//	192: 17 8 14 => 17 || 8 + 14
		var c2 = new CalibrationProblem(new CalibrationProblem(17, 8, CalibrationOperator.Concatenate), 14, CalibrationOperator.Add);
		Debug.Assert(c2.Value == 192);
		Debug.Assert("17 || 8 + 14" == c2.ToString());

		CalibrationOperator[] operators = new[] { CalibrationOperator.Add, CalibrationOperator.Multiply, CalibrationOperator.Concatenate };
		var solutions = FindSolutions(_partOneTestInput, operators);
		Debug.Assert(6 == solutions.Count);
		solutions.ForEach(s => Console.WriteLine($"{s} = {s.Value}"));
	}

	#endregion
}