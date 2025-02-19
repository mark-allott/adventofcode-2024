namespace AdventOfCode.Enums;

internal enum CellState
{
	Unknown = 0,
	Unvisited,
	Obstructed,
	CurrentPosition,
	OutOfBounds,
	Visited,
}