using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Models;

internal class Antinode
	: IEquatable<Antinode>
{
	/// <summary>
	/// Stores the y-coordinate of the location for the antinode
	/// </summary>
	public int Row { get; }

	/// <summary>
	/// Stores the x-coordinate of the location for the antinode
	/// </summary>
	public int Column { get; }

	/// <summary>
	/// Holds details of the bounds for the map in X-Y coordinates (assumes top-left is [0,0] and bottom is [column-1, row-1])
	/// </summary>
	private (int row, int column) _bounds;

	/// <summary>
	/// The antenna sending the broadcast
	/// </summary>
	public Antenna BroadcastAntenna { get; } = null!;

	/// <summary>
	/// The antenna paired to the broadcast which allows the antinode(s) to be calculated
	/// </summary>
	public Antenna PairedAntenna { get; } = null!;

	/// <summary>
	/// Determines whether the antinode is within the bounds of the map
	/// </summary>
	public bool InBounds => IsInbounds();

	/// <summary>
	/// ctor - takes two antennae and works out the antinode for the broadcast location
	/// </summary>
	/// <param name="broadcast">The broadcast antenna</param>
	/// <param name="paired">The antenna paired with the <paramref name="broadcast"/></param>
	public Antinode(Antenna broadcast, Antenna paired, (int row, int column) bounds)
	{
		ArgumentNullException.ThrowIfNull(broadcast, nameof(broadcast));
		ArgumentNullException.ThrowIfNull(paired, nameof(paired));
		ArgumentOutOfRangeException.ThrowIfEqual(broadcast, paired, nameof(paired));

		BroadcastAntenna = broadcast;
		PairedAntenna = paired;

		var offsetRow = paired.Coordinate.row - broadcast.Coordinate.row;
		var offsetColumn = paired.Coordinate.col - broadcast.Coordinate.col;

		Row = broadcast.Coordinate.row + (2 * offsetRow);
		Column = broadcast.Coordinate.col + (2 * offsetColumn);
		_bounds = bounds;
	}

	/// <summary>
	/// Method to determine whether the antinode lies within the bounds of the map
	/// </summary>
	/// <returns>True if within the bounds of the map, otherwise false</returns>
	private bool IsInbounds()
	{
		return Row >= 0 && Column >= 0 &&
			Row < _bounds.row && Column < _bounds.column;
	}

	#region IEquatable implementation

	public bool Equals(Antinode? other)
	{
		return other is not null &&
			Row == other.Row &&
			Column == other.Column;
	}

	#endregion

	/// <summary>
	/// Overrides the base method to show the coordinates of the antinode, frequency of broadcast and <see cref="GetHashCode"/>
	/// </summary>
	/// <returns>The string representation for the antinode</returns>
	public override string ToString()
	{
		return $"{BroadcastAntenna.Frequency}({Row}, {Column} [{GetHashCode()}])";
	}

	/// <summary>
	/// Overrides the base implementation to provide a new hashcode value
	/// </summary>
	/// <param name="obj">The antinode</param>
	/// <returns>The value for the hashcode</returns>
	public int GetHashCode([DisallowNull] Antinode obj)
	{
		return HashCode.Combine(obj.Row, obj.Column);
	}
}

/// <summary>
/// Helper class to determine whether two antinodes are equivalent
/// </summary>
/// <remarks>
/// Antinodes are considered equivalent if they both occupy the same map location,
/// irrespective of the broadcast frequency
/// </remarks>
internal class AntinodeEqualityComparer
	: IEqualityComparer<Antinode>
{
	public bool Equals(Antinode? x, Antinode? y)
	{
		return x is null
			? y is null || y.Equals(x)
			: x.Equals(y);
	}

	public int GetHashCode([DisallowNull] Antinode obj)
	{
		return HashCode.Combine(obj.Row, obj.Column);
	}
}