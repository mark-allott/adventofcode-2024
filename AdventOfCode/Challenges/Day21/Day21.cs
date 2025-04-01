using AdventOfCode.Interfaces;

namespace AdventOfCode.Challenges.Day21;

public partial class Day21
	: AbstractDailyChallenge, IAutoRegister
{
	#region Overrides of AbstractDailyChallenge

	public Day21()
		: base(21, "day21-input.txt", "Keypad Conundrum")
	{
	}

	#endregion
}