using AdventOfCode.Enums;

namespace AdventOfCode.Models;

/// <summary>
/// Represents a plt of land within a garden
/// </summary>
internal class GardenPlot
{
	/// <summary>
	/// The location of the plot within the overall garden
	/// </summary>
	public MapCoord Location { get; }

	/// <summary>
	/// A representation of the planted item in the plot
	/// </summary>
	public char Plant { get; }

	/// <summary>
	/// A flags enum representing the fences around the plot
	/// </summary>
	public FencePosition FencePositions { get; private set; } = FencePosition.None;

	/// <summary>
	/// Holds a reference to the region this plot is in
	/// </summary>
	public GardenRegion Region { get; private set; } = null!;

	#region ctor

	public GardenPlot(MapCoord location, char plant)
	{
		ArgumentNullException.ThrowIfNull(location, nameof(location));
		Location = location;
		Plant = plant;
	}

	#endregion

	/// <summary>
	/// Sets the position of all fences at the same time
	/// </summary>
	/// <param name="positions">The fence positions for the plot</param>
	public void SetFences(FencePosition positions)
	{
		FencePositions = positions;
	}

	/// <summary>
	/// Adds a fence, or fences in the specific position(s)
	/// </summary>
	/// <param name="positions">Which position(s) to be added</param>
	public void AddFences(FencePosition positions)
	{
		FencePositions |= positions;
	}

	/// <summary>
	/// Removes a fence, or fences from a plot
	/// </summary>
	/// <param name="positions">The position(s) to remove</param>
	public void RemoveFences(FencePosition positions)
	{
		FencePositions &= positions;
	}

	/// <summary>
	/// Debug helper to represent the plot
	/// </summary>
	/// <returns>The location and planted item in the plot</returns>
	public override string ToString()
	{
		return $"[{Location} {Plant}]";
	}

	/// <summary>
	/// Sets the region for the plot. Once set, cannot be changed
	/// </summary>
	/// <param name="region">The region to which the plot belongs</param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public void SetRegion(GardenRegion region)
	{
		ArgumentNullException.ThrowIfNull(region, nameof(region));
		if (Region is not null && Region != region)
			throw new ArgumentOutOfRangeException(nameof(region), $"Cannot change regions once set");

		Region = region;
	}
}