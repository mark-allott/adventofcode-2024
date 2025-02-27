using AdventOfCode.Interfaces;

namespace AdventOfCode.Challenges.Day02;

public partial class Day02
	: AbstractDailyChallenge, IAutoRegister
{
	#region Overrides of AbstractDailyChallenge

	public Day02()
		: base(2, "day02-input.txt")
	{
	}

	#endregion
}