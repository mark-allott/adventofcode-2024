using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges;

public partial class Day03
	: AbstractDailyChallenge, IAutoRegister
{
	#region Overrides to run part two of challenge

	/// <summary>
	/// Overrides the base implementation to provide the solution for part two
	/// </summary>
	/// <returns></returns>
	protected override bool PartTwo()
	{
		LoadAndReadFile();

		var operations = GetOperations(InputFileLines);
		long total = 0;
		operations.ForEach(o => total += ExecuteOperation(o));

		PartTwoResult = $"the total of mul operations that can be executed is {total}";
		return true;
	}

	#endregion

	//	Default for mul operations is to execute them initially
	private bool doMulOperations = true;

	/// <summary>
	/// Execute the operation. Currently only "do", "don't" and "mul" have any actions
	/// </summary>
	/// <param name="o">An instance of the <see cref="Operation"/> class</param>
	/// <returns>The value of the mul operation (if permitted to execute it), or zero</returns>
	private long ExecuteOperation(Operation o)
	{
		ArgumentNullException.ThrowIfNull(o, nameof(o));

		switch (o.Instruction.ToLowerInvariant())
		{
			case "do":
				doMulOperations = true;
				return 0;
			case "don't":
				doMulOperations = false;
				return 0;
			case "mul":
				return doMulOperations
					? ExecuteMulInstruction(o)
					: 0;
			//	All other operations have no effect
			default: return 0;
		}
	}

}