using AdventOfCode.Enums;

namespace AdventOfCode.Models;

/// <summary>
/// Represents a node in a Dijkstra algorithm search
/// </summary>
internal class DijkstraNode
{
	#region Properties

	/// <summary>
	/// The distance from the start node
	/// </summary>
	public int Distance { get; set; } = int.MaxValue;

	/// <summary>
	/// The location in the grid
	/// </summary>
	public MapCoord Location { get; } = null!;

	/// <summary>
	/// The direction of travel for the node
	/// </summary>
	public DirectionOfTravel Direction { get; set; } = DirectionOfTravel.Unknown;

	#endregion

	#region Ctor

	/// <summary>
	/// ctor - requires the location to be set; other properties can be set later
	/// </summary>
	/// <param name="location"></param>
	public DijkstraNode(MapCoord location)
	{
		ArgumentNullException.ThrowIfNull(location, nameof(location));
		Location = location;
	}

	#endregion

	#region Overrides from base

	/// <summary>
	/// Provides a debug-friendly print of the node
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		return $"[{Location}, {Direction}] => {Distance}";
	}

	#endregion
}