namespace AdventOfCode.Models;

internal class MonkeyMarket
{
	/// <summary>
	/// Mutates the <paramref name="value"/> according to the rules of the challenge
	/// </summary>
	/// <param name="value">The value to be mutated</param>
	/// <returns>The mutated value</returns>
	public long Mutate(long value)
	{
		//	Step 1.1: multiply by 64 and mix
		value ^= value * 64;
		//	Step 1.2: prune
		value %= 16777216;
		//	Step 2.1: integer division by 32 and mix
		value ^= value / 32;
		//	Step 2.2: prune
		value %= 16777216;
		//	Step 3.1: multiply by 2048 and mix
		value ^= value * 2048;
		//	Step 3.2: prune
		value %= 16777216;
		return value;
	}

	/// <summary>
	/// Mutates the specified <paramref name="value"/> for the specified number of <paramref name="iterations"/>
	/// </summary>
	/// <param name="value">The initial secret value</param>
	/// <param name="iterations">The number of mutations to execute</param>
	/// <returns>A dictionary containing the mutated value of each iteration</returns>
	public Dictionary<int, long> MutateFor(long value, int iterations)
	{
		return Enumerable.Range(0, iterations)
			.Select(i => new { Key = i + 1, Value = value = Mutate(value) })
			.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
	}
}