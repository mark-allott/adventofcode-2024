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
	/// <param name="maxY">The maximum row (Y-coord) for the map</param>
	/// <param name="maxX">The maximum column (X-coord) for the map</param>
	/// <returns>True if within bounds, otherwise false</returns>
	/// <remarks>
	/// Both <paramref name="maxY"/> and <paramref name="maxX"/> are 1-based
	/// indexes, whereas internal coordinates are always 0-based
	/// </remarks>
	public bool InBounds(int maxY, int maxX)
	{
		return X >= 0 && Y >= 0 &&
			X < maxX && Y < maxY;
	}

	/// <summary>
	/// Check method allows the coordinate to be verified as within bounds, expressed by another <see cref="MapCoord"/> object, <paramref name="bounds"/>
	/// </summary>
	/// <param name="bounds">The upper bounds of the grid</param>
	/// <returns>True if within bounds, otherwise false</returns>
	/// <remarks>
	/// Both indices of <paramref name="bounds"/> are 1-based
	/// indexes, whereas internal coordinates are always 0-based
	/// </remarks>
	public bool InBounds(MapCoord bounds)
	{
		ArgumentNullException.ThrowIfNull(bounds, nameof(bounds));
		return InBounds(bounds.Y, bounds.X);
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