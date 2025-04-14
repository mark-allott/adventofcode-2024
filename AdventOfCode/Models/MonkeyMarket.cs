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
		var initialValue = value;
		//	Perform the mutations for the required number of iterations
		var result = Enumerable.Range(0, iterations)
			.Select(i => new { Key = i + 1, Value = value = Mutate(value) })
			.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		//	Set the 0-index of the dictionary to the first value passed
		result[0] = initialValue;
		return result;
	}

	/// <summary>
	/// Performs mutations on <paramref name="value"/> for the specified number of <paramref name="iterations"/>
	/// </summary>
	/// <param name="value">The initial buyer secret</param>
	/// <param name="iterations">The number of iterations to run for</param>
	/// <returns>A dictionary of buyer secrets, prices and changes</returns>
	public Dictionary<int, (long value, int price, int change)> MutateWithChangesFor(long value, int iterations)
	{
		var result = new Dictionary<int, (long, int, int)>();
		result.Add(0, (value, (int)value % 10, 0));
		Enumerable.Range(0, iterations)
			.Select(i => new { Iteration = i + 1, Previous = value, Value = value = Mutate(value) })
			.Select(s => new { s.Iteration, s.Value, Price = (int)(s.Value % 10), PreviousPrice = (int)(s.Previous % 10) })
			.ToList()
			.ForEach(i => result.TryAdd(i.Iteration, (i.Value, i.Price, i.Price - i.PreviousPrice)));
		return result;
	}

	/// <summary>
	/// Performs mutations on the buyer secret <paramref name="value"/> for the specified number of <paramref name="iterations"/>
	/// </summary>
	/// <param name="value">The initial value for the buyer secret</param>
	/// <param name="iterations">The number of iterations to perform</param>
	/// <returns>A dictionary of change sequences with the price offered for the first occurrence of the sequence</returns>
	public Dictionary<string, int> MutateWithSequencesFor(long value, int iterations)
	{
		//	Determine the changes in prices for this buyer
		var changes = MutateWithChangesFor(value, iterations);

		var sequences = changes
			//	Get the index number and data
			.Select(s => new { Index = s.Key, Data = s.Value })
			//	Extract the index, price and a sequence of changes
			.Select(s => new
			{
				s.Index,
				Price = s.Data.price,
				//	Make a sequence starting 3 before current index and 4 long, but index MUST be greater than zero
				Sequence = Enumerable.Range(s.Index - 3, 4).Where(i => i > 0).Select(m => changes[m]).Select(s => s.change).ToList()
			})
			//	Only where there are exactly 4 changes
			.Where(q => q.Sequence.Count == 4)
			//	Create a new object with the price and string equivalent of the changes
			.Select(s => new { Sequence = string.Join(",", s.Sequence), s.Index, s.Price })
			.OrderBy(o => o.Index)
			//	Grouping by the sequence...
			.GroupBy(g => g.Sequence)
			//	Select the first price available for the sequence
			.Select(s => new { Sequence = s.Key, Price = s.Select(q => new { q.Index, q.Price }).OrderBy(o => o.Index).FirstOrDefault()?.Price ?? 0 })
			//	Convert to dictionary for later lookups
			.ToDictionary(kvp => kvp.Sequence, kvp => kvp.Price);

		return sequences;
	}
}