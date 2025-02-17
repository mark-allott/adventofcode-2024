namespace AdventOfCode.Enums;

/// <summary>
/// used to indicate the direction in which the word progresses in the grid.
/// <para>e.g. XMAS would be East, SAMX would be West, etc.</para>
/// </summary>
public enum WordGridDirection
{
	Unknown = 0,
	North,
	NorthEast,
	East,
	SouthEast,
	South,
	SouthWest,
	West,
	NorthWest
}