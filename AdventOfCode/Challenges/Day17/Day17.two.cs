using AdventOfCode.Extensions;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day17;

public partial class Day17
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

		var (a, b, c, program) = ExtractSetup();

		//	Get the list of integers within the program
		var integers = program.ParseStringToListOfInt();
		//	Reverse so the search starts with the end digit
		integers.Reverse();

		//	Storage for solutions
		var solutions = new List<long>();

		//	Setup the computer
		var computer = new ChronospatialComputer();
		computer.LoadProgram(program);

		//	Initialise the queue of numbers to evaluate
		var checkQueue = new Queue<(int offset, long value)>();
		checkQueue.Enqueue((0, 0));

		//	Whilst there are still possible outcomes to investigate, keeping searching
		while (checkQueue.Count > 0)
		{
			//	Pull the next value for investigation from the queue
			(var offset, a) = checkQueue.Dequeue();

			//	if the offset is the length of the list of integers to find, a solution has been found!
			if (offset == integers.Count)
			{
				//	add to the solution list and move onto the next
				solutions.Add(a);
				continue;
			}

			//	Assemble the last integers being looked for this time around the loop and reverse them
			var lastInts = integers.GetRange(0, offset + 1);
			lastInts.Reverse();

			//	look for new solutions, shifting the previous value left by 3 bits
			var newSolutions = GetSolutionValues(lastInts, a << 3, computer);

			//	if no solutions found, then the previous value for "a" wasn't good for this iteration
			if (newSolutions.Count == 0)
				continue;

			//	enqueue new solutions to be investigated
			newSolutions.ForEach(v => checkQueue.Enqueue((offset + 1, v)));
		}

		//	all solutions found, get the lowest value
		a = solutions.Min();
		PartTwoResult = $"{ChallengeTitle} : lowest value found is {a}";
		return true;
	}

	/// <summary>
	/// Get all possible solutions to the current set of integers
	/// </summary>
	/// <param name="integers">The list of integers to find at the "end" of the program output</param>
	/// <param name="initialA">The initial value for the "A" register</param>
	/// <param name="computer">The computer, loaded with the program and ready to run</param>
	/// <returns>All solutions to the problem (if any)</returns>
	private List<long> GetSolutionValues(List<int> integers, long initialA, ChronospatialComputer computer)
	{
		var solutions = new List<long>();
		var endDigits = string.Join(',', integers);

		//	loop around the 3-bit range looking for potential solutions for the digits
		for (var i = 0; i < 8; i++)
		{
			computer.SetRegisters(initialA + i);
			computer.Run();

			if (computer.Output.EndsWith(endDigits))
				solutions.Add(initialA + i);
		}

		return solutions;
	}

	#endregion
}