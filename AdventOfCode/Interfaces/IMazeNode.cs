namespace AdventOfCode.Interfaces;

internal interface IMazeNode
{
	/// <summary>
	/// Represents the coords in a graph as X & Y components
	/// </summary>
	IGraphCoordinate Location { get; set; }

	/// <summary>
	/// Represents the distance between one node and another
	/// </summary>
	int Distance { get; set; }
}