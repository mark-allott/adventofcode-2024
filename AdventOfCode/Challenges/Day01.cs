using AdventOfCode.Interfaces;

namespace AdventOfCode.Challenges;

public partial class Day01
	: AbstractDailyChallenge, IAutoRegister
{
	#region Overrides from AbstractDailyChallenge

	public Day01()
		: base(1, "day01-input.txt")
	{
	}

	#endregion
}