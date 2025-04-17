using AdventOfCode.Interfaces;

namespace AdventOfCode.Challenges.Day20;

public partial class Day20
	: AbstractDailyChallenge, IAutoRegister
{
	#region Overrides of AbstractDailyChallenge

	public Day20()
		: base(20, "day20-input.txt", "Race Condition")
	{
	}

	#endregion
}