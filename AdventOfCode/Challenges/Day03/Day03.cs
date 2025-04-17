using AdventOfCode.Interfaces;

namespace AdventOfCode.Challenges.Day03;

public partial class Day03
	: AbstractDailyChallenge, IAutoRegister
{
	#region Overrides of AbstractDailyChallenge

	public Day03()
		: base(3, "day03-input.txt")
	{
	}

	#endregion
}