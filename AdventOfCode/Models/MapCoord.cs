using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Models;

internal class MapCoord
	: ICloneable
{
	#region Properties

	public int X { get; }
	public int Y { get; }

	#endregion

	#region Constructor

	public MapCoord(int row, int column)
	{
		X = column;
		Y = row;
	}

	#endregion

	#region ICloneable implementation

	/// <summary>
	/// Creates a new coordinate object from the current value
	/// </summary>
	/// <returns>The new coordinate value, duplicating this one</returns>
	public MapCoord DeepCopy()
	{
		return new MapCoord(Y, X);
	}

	object ICloneable.Clone()
	{
		return DeepCopy();
	}

	#endregion

	#region Overrides from base

	/// <summary>
	/// Debug friendly version of the coords
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		return $"({X},{Y})";
	}

	#endregion

	#region Public methods

	/// <summary>
	/// Check method that allows the coordinate to be verified as within the bounds of a map
	/// </summary>
	/// <param name="maxRow">The maximum row (Y-coord) for the map</param>
	/// <param name="maxCol">The maximum column (X-coord) for the map</param>
	/// <returns>True if within bounds, otherwise false</returns>
	/// <remarks>
	/// Both <paramref name="maxRow"/> and <paramref name="maxCol"/> are 1-based
	/// indexes, whereas internal coordinates are always 0-based
	/// </remarks>
	public bool InBounds(int maxRow, int maxCol)
	{
		return X >= 0 && Y >= 0 &&
			X < maxCol && Y < maxRow;
	}

	#endregion
}

internal class MapCoordEqualityComparer
	: IEqualityComparer<MapCoord>
{
	#region IEqualityComparer implementation

	public bool Equals(MapCoord? a, MapCoord? b)
	{
		return b is null
			? a is null
			: a.X == b.X && a.Y == b.Y;
	}

	public int GetHashCode([DisallowNull] MapCoord obj)
	{
		return HashCode.Combine(obj.X, obj.Y);
	}

	#endregion
}