namespace AdventOfCode.Models;

internal class LinenLayout
{
	#region Fields

	/// <summary>
	/// Holds the patterns for the linen
	/// </summary>
	private readonly List<string> _patterns;

	//	Holds the desired designs to be assembled
	private readonly List<string> _designs;

	/// <summary>
	/// Memo store - holds details of how many different combinations are possible
	/// </summary>
	private readonly Dictionary<string, long> _lookups = new Dictionary<string, long>();

	#endregion

	#region Constructors

	/// <summary>
	/// ctor - accepting the patterns available and desired designs
	/// </summary>
	/// <param name="patterns"></param>
	/// <param name="designs"></param>
	public LinenLayout(List<string> patterns, List<string> designs)
	{
		_patterns = patterns;
		_designs = designs;
	}

	#endregion

	#region Methods

	/// <summary>
	/// Determines how many of the requested designs (passed via ctor) are valid based on the patterns known
	/// </summary>
	/// <returns>The number of patterns that are valid</returns>
	public int GetValidDesigns()
	{
		return _designs.Count(d => GetDesignScore(d) > 0);
	}

	/// <summary>
	/// Recursive method to determine the score (combinations) of the <paramref name="design"/> based on known patterns
	/// </summary>
	/// <param name="design">The design to verify</param>
	/// <returns>The number of combinations of patterns that can make up the <paramref name="design"/></returns>
	private long GetDesignScore(string design)
	{
		if (_lookups.TryGetValue(design, out long score))
			return score;

		//	Set the number of patterns that can be used to generate the required design
		//	This implicitly sets unsolvable designs to zero due to use of the Sum method (the sum of nothing is zero)
		_lookups[design] = _patterns
			//	Look for patterns which match the start of the design
			.Where(q => design.StartsWith(q))
			//	If the design being looked-up is exactly the same as the pattern, there is one solution
			.Sum(p => design == p
				? 1
				//	Otherwise, continue down the design starting with the next segment after the current pattern
				: GetDesignScore(design[p.Length..]));

		//	There is now a value stored, so return that
		return _lookups[design];
	}

	#endregion
}