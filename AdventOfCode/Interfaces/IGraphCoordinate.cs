namespace AdventOfCode.Interfaces;

internal interface IGraphCoordinate
	: IEquatable<IGraphCoordinate>
{
	/// <summary>
	/// X-coord in a graph
	/// </summary>
	int X { get; }

	/// <summary>
	/// Y coord in a graph
	/// </summary>
	int Y { get; }

	/// <summary>
	/// Sets the coords for the implementing class
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	void Set(int x, int y);
}