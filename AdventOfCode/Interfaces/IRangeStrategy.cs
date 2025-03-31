namespace AdventOfCode.Interfaces;

internal interface IRangeStrategy<T>
	where T : class
{
	/// <summary>
	/// Determine the range between two objects
	/// </summary>
	/// <param name="from">The object to measure from</param>
	/// <param name="to">The object to measure to</param>
	/// <returns>The distance between the two objects</returns>
	int Range(T from, T to);
}