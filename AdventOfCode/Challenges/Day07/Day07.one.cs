using System.Diagnostics;
using AdventOfCode.Enums;
using AdventOfCode.Extensions;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day07;

public partial class Day07
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

		CalibrationOperator[] operators = new[] { CalibrationOperator.Add, CalibrationOperator.Multiply };
		var solutions = FindSolutions(InputFileLines, operators);
		long total = 0;
		solutions.ForEach(s => total += s.Value);
		PartOneResult = $"Total calibration result = {total}";
		return true;
	}

	/// <summary>
	/// Using the lines supplied in <paramref name="input"/> and the permitted
	/// types of operations in <paramref name="operators"/>, find the lines
	/// that can result in a valid solution
	/// </summary>
	/// <param name="input">The list of problems</param>
	/// <param name="operators">The permitted operations that apply to the list</param>
	/// <returns>A list of <see cref="CalibrationProblem"/> objects that represent correct solutions</returns>
	private List<CalibrationProblem> FindSolutions(List<string> input, CalibrationOperator[] operators)
	{
		var solutions = new List<CalibrationProblem>();
		foreach (var line in input)
		{
			var parts = line.Split(':');
			if (!long.TryParse(parts[0], out var expectedResult))
				continue;
			var values = parts[1].ParseStringToListOfInt();

			//	must have at least 2 numbers to work with
			ArgumentOutOfRangeException.ThrowIfLessThan(values.Count, 2, nameof(values));

			//	initialise the list of solutions to be checked and a queue that contains the numbers
			var operationsToBeChecked = new List<CalibrationProblem>();
			var valueQueue = new Queue<int>(values);

			//	use the first two numbers in the queue and add solutions to the initial list
			operationsToBeChecked.AddRange(MakeOperations(valueQueue.Dequeue(), valueQueue.Dequeue(), operators));

			//	Now loop for each remaining number in the queue
			while (valueQueue.Count > 0)
			{
				//	grab the next number from the queue
				var bPart = valueQueue.Dequeue();

				//	Create a new container for updated operations
				var newOperationsList = new List<CalibrationProblem>();

				//	Loop around each operation currently in the list
				foreach (var operation in operationsToBeChecked)
				{
					//	Add new operations based on the current list, adding the new combinations
					newOperationsList.AddRange(MakeOperations(operation, bPart, operators));
				}
				//	Set the list to be checked to the new list
				operationsToBeChecked = newOperationsList;
			}

			//	Quick check: the number of problems to check is a power of the number of values in the list
			//	e.g.	for operations [ +, * ] and numbers [ 1, 2, 3 ], there should be 4 problems to check
			//			for operations [ +, * ] and numbers [ 1, 2, 3, 4 ], there should be 8 problems to check
			Debug.Assert(Math.Pow(operators.Length, values.Count - 1) == operationsToBeChecked.Count);

			//	It doesn't matter which problem solves it, just so long as one option does!
			var solution = operationsToBeChecked.FirstOrDefault(q => q.Value == expectedResult);

			//	Add a solved problem to the list of solutions we have found
			if (solution is not null)
				solutions.Add(solution);
		}
		return solutions;
	}

	/// <summary>
	/// For a given problem, create new problems that represent the combinations required
	/// </summary>
	/// <param name="a">The "a" part of the <see cref="CalibrationProblem"/> object</param>
	/// <param name="b">The "b" part of the <see cref="CalibrationProblem"/> object</param>
	/// <param name="operators">An array of <see cref="CalibrationOperator"/> operations to perform</param>
	/// <returns></returns>
	private List<CalibrationProblem> MakeOperations(object a, int b, CalibrationOperator[] operators)
	{
		var operations = new List<CalibrationProblem>();
		foreach (CalibrationOperator op in operators)
			operations.Add(MakeOperation(a, b, op));
		return operations;
	}

	/// <summary>
	/// for the given inputs, create a <see cref="CalibrationProblem"/> problem, ready to be solved (or not)
	/// </summary>
	/// <param name="a">The first part of the problem</param>
	/// <param name="b">The second part of the problem</param>
	/// <param name="calibrationOperator">The operation to be performed</param>
	/// <returns>The <see cref="CalibrationProblem"/> problem waiting to be solved</returns>
	/// <exception cref="ArgumentException"></exception>
	private CalibrationProblem MakeOperation(object a, int b, CalibrationOperator calibrationOperator)
	{
		if (a is int v)
			return new CalibrationProblem(v, b, calibrationOperator);

		if (a is CalibrationProblem op)
			return new CalibrationProblem(op, b, calibrationOperator);

		throw new ArgumentException("Unhandled value", nameof(a));
	}

	#endregion

	#region IPartOneTestable implementation

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	public void PartOneTest()
	{
		var c1 = new CalibrationProblem(10, 19, CalibrationOperator.Multiply);
		Debug.Assert(c1.Value == 190);
		Debug.Assert("10 * 19" == c1.ToString());

		var c2 = new CalibrationProblem(new CalibrationProblem(81, 40, CalibrationOperator.Add), 27, CalibrationOperator.Multiply);
		Debug.Assert(c2.Value == 3267);
		Debug.Assert("81 + 40 * 27" == c2.ToString());

		CalibrationOperator[] operators = new[] { CalibrationOperator.Add, CalibrationOperator.Multiply };
		var solutions = FindSolutions(_partOneTestInput, operators);
		Debug.Assert(3 == solutions.Count);
		solutions.ForEach(s => Console.WriteLine($"{s} = {s.Value}"));
	}

	private List<string> _partOneTestInput = new List<string>()
	{
		"190: 10 19",
		"3267: 81 40 27",
		"83: 17 5",
		"156: 15 6",
		"7290: 6 8 6 15",
		"161011: 16 10 13",
		"192: 17 8 14",
		"21037: 9 7 18 13",
		"292: 11 6 16 20"
	};

	#endregion
}