using AdventOfCode.Interfaces;

namespace AdventOfCode.Challenges.Day23;

public partial class Day23
	: AbstractDailyChallenge, IAutoRegister
{
	#region Overrides of AbstractDailyChallenge

	public Day23()
		: base(23, "day23-input.txt", "LAN Party")
	{
	}

	#endregion
}