using System.Text.RegularExpressions;
using AdventOfCode.Extensions;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges;

public partial class Day03
	: AbstractDailyChallenge, IAutoRegister
{
	#region Overrides to run part one of challenge

	/// <summary>
	/// Override the base implementation to provide the actual answer
	/// </summary>
	/// <returns>True if successful</returns>
	protected override bool PartOne()
	{
		LoadAndReadFile();

		var operations = GetOperations(InputFileLines);
		var mulInstructions = GetMulInstructions(operations);
		long total = 0;
		mulInstructions.ForEach(i => total += ExecuteMulInstruction(i));
		PartOneResult = $"Total for mul instructions = {total}";
		return true;
	}

	#endregion

	private readonly Regex OpAndParams = new Regex(@"([a-z][a-z']+)\(([\w,+-]*)\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	/// <summary>
	/// From the given input(s), parse the text for instructions and associated parameter(s)
	/// </summary>
	/// <param name="input">The text to be parsed</param>
	/// <returns>A list of <see cref="Operation"/> objects</returns>
	private List<Operation> GetOperations(List<string> input)
	{
		var operations = new List<Operation>();
		foreach (var inputLine in input)
		{
			var matches = OpAndParams.Matches(inputLine);

			if (matches is not null && matches.Count > 0)
			{
				//	Convert matches into Operation classes
				var x = matches.Where(q => q.Success)
					.Select(s => new Operation(s.Groups[1].Value, s.Groups[2].Value))
					.ToList();
				operations.AddRange(x);
			}
		}
		return operations;
	}

	/// <summary>
	/// Extract the mul operations from the list of operations contained in <paramref name="input"/>
	/// </summary>
	/// <param name="input">A list of <see cref="Operation"/> objects</param>
	/// <returns>The list of mul instructions</returns>
	private List<Operation> GetMulInstructions(List<Operation> input)
	{
		return input.Where(q => q.Instruction.Equals("mul", StringComparison.OrdinalIgnoreCase))
			.ToList();
	}

	/// <summary>
	/// Perform the mul instruction that requires 2 numbers to be multiplied together and the result returned
	/// </summary>
	/// <param name="operation">an <see cref="Operation"/> object, containing the instruction and parameters</param>
	/// <returns>The result of the multiplication of the 2 numbers</returns>
	private int ExecuteMulInstruction(Operation operation)
	{
		if (!operation.Instruction.Equals("mul", StringComparison.OrdinalIgnoreCase))
			throw new ArgumentException($"{operation.Instruction} is not valid.", nameof(operation));

		var numbers = operation.Parameters.ParseEnumerableOfStringToListOfInt();

		//	If 2 numbers are not present, multiplication cannot occur; therefore the result is 0
		if (numbers.Count != 2)
			return 0;

		return numbers[0] * numbers[1];
	}
}