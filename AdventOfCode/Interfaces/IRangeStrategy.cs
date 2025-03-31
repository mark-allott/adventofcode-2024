namespace AdventOfCode.Interfaces;

internal interface IRangeStrategy<T>
{
	int Range(T from, T to);
}