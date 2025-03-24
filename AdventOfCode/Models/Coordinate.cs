using System.Diagnostics.CodeAnalysis;
using AdventOfCode.Interfaces;

namespace AdventOfCode.Models;

internal class Coordinate
	: ICloneable, IEquatable<Coordinate>, IGraphCoordinate
{
	#region Properties

	#region IGraphCoordinate implementation

	/// <inheritdoc/>
	public int X { get; private set; } = 0;

	/// <inheritdoc/>
	public int Y { get; private set; } = 0;

	#endregion

	#endregion

	#region Constructor

	/// <summary>
	/// Standard ctor - note the params are reversed from standard notation due
	/// to use in prior solutions which usd row (y-coord) and column (x-coord) values
	/// </summary>
	/// <param name="y">The y-coordinate</param>
	/// <param name="x">The x-coordinate</param>
	public Coordinate(int y, int x)
	{
		X = x;
		Y = y;
	}

	/// <summary>
	/// Alternate ctor - accepts parameters from a class implementing <see cref="IGraphCoordinate"/>
	/// </summary>
	/// <param name="coordinate">The coordinate to copy</param>
	public Coordinate(IGraphCoordinate coordinate)
	{
		X = coordinate.X;
		Y = coordinate.Y;
	}

	#endregion

	#region ICloneable implementation

	/// <summary>
	/// Creates a new coordinate object from the current value
	/// </summary>
	/// <returns>The new coordinate value, duplicating this one</returns>
	public Coordinate DeepCopy()
	{
		return new Coordinate(Y, X);
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
	/// Check method allows the coordinate to be verified as within bounds, expressed by another <see cref="Coordinate"/> object, <paramref name="bounds"/>
	/// </summary>
	/// <param name="bounds">The upper bounds of the grid</param>
	/// <returns>True if within bounds, otherwise false</returns>
	/// <remarks>
	/// Both indices of <paramref name="bounds"/> are 1-based
	/// indexes, whereas internal coordinates are always 0-based
	/// </remarks>
	public bool InBounds(IGraphCoordinate bounds)
	{
		ArgumentNullException.ThrowIfNull(bounds, nameof(bounds));
		return InBounds(bounds.Y, bounds.X);
	}

	#endregion

	#region IEquatable implementation

	/// <summary>
	/// Provide an equality comparison to determine whether two <see cref="Coordinate"/> objects occupy the same location
	/// </summary>
	/// <param name="other">The <see cref="Coordinate"/> to be compared with this object</param>
	/// <returns>True if both objects have the same coords, otherwise false</returns>
	public bool Equals(Coordinate? other)
	{
		return other is not null &&
			other.X == X &&
			other.Y == Y;
	}

	public bool Equals(IGraphCoordinate? other)
	{
		return other is not null &&
			other.X == X &&
			other.Y == Y;
	}

	#endregion

	#region IGraphPosition implementation

	public void Set(int x, int y)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(x, nameof(x));
		ArgumentOutOfRangeException.ThrowIfNegative(y, nameof(y));
		X = x;
		Y = y;
	}

	#endregion

	#region static helpers

	public static Coordinate operator +(Coordinate a, Coordinate b) => new Coordinate(a.Y + b.Y, a.X + b.X);

	public static Coordinate operator -(Coordinate a, Coordinate b) => new Coordinate(a.Y - b.Y, a.X - b.X);

	public static Coordinate OffsetNorth() => new Coordinate(-1, 0);

	public static Coordinate OffsetSouth() => new Coordinate(1, 0);

	public static Coordinate OffsetEast() => new Coordinate(0, 1);

	public static Coordinate OffsetWest() => new Coordinate(0, -1);

	public static Coordinate OffsetNorthEast() => new Coordinate(-1, 1);

	public static Coordinate OffsetSouthEast() => new Coordinate(1, 1);

	public static Coordinate OffsetSouthWest() => new Coordinate(1, -1);

	public static Coordinate OffsetNorthWest() => new Coordinate(-1, -1);

	#endregion
}