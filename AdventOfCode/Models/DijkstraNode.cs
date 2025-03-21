using AdventOfCode.Enums;
using AdventOfCode.Interfaces;

namespace AdventOfCode.Models;

/// <summary>
/// Represents a node in a Dijkstra algorithm search
/// </summary>
internal class DijkstraNode
	: IMazeNode, IDirectional
{
	#region Properties

	/// <summary>
	/// The distance from the start node
	/// </summary>
	public int Distance { get; set; } = int.MaxValue;

	/// <summary>
	/// The location in the grid
	/// </summary>
	public Coordinate Location { get; } = null!;

	/// <summary>
	/// The direction of travel for the node
	/// </summary>
	public DirectionOfTravel Direction { get; set; } = DirectionOfTravel.Unknown;

	/// <summary>
	/// Explicit implementation for IMazeNode
	/// </summary>
	IGraphCoordinate IMazeNode.Location => Location;

	#endregion

	#region Ctor

	/// <summary>
	/// ctor - requires the location to be set; other properties can be set later
	/// </summary>
	/// <param name="location"></param>
	public DijkstraNode(Coordinate location)
	{
		ArgumentNullException.ThrowIfNull(location, nameof(location));
		Location = location;
	}

	/// <summary>
	/// Alternate ctor - duplicates the existing node
	/// </summary>
	/// <param name="other">The node to be duplicated</param>
	public DijkstraNode(DijkstraNode other)
	{
		ArgumentNullException.ThrowIfNull(other, nameof(other));
		Distance = other.Distance;
		Location = new Coordinate(other.Location);
		Direction = other.Direction;
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