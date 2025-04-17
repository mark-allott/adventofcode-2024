using AdventOfCode.Interfaces;

namespace AdventOfCode.Challenges.Day22;

public partial class Day22
	: AbstractDailyChallenge, IAutoRegister
{
	#region Overrides of AbstractDailyChallenge

	public Day22()
		: base(22, "day22-input.txt", "Monkey Market")
	{
	}

	#endregion
}