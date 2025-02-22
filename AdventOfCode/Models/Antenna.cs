namespace AdventOfCode.Models;

/// <summary>
/// Represents an antenna, at a specified location, broadcasting on a specific frequency
/// </summary>
internal class Antenna
	: IEquatable<Antenna>
{
	/// <summary>
	/// The frequency of the broadcast
	/// </summary>
	public char Frequency { get; private set; }

	/// <summary>
	/// The coordinate of the antenna
	/// </summary>
	public (int row, int col) Coordinate { get; private set; }

	/// <summary>
	/// ctor - instantiates a new antenna
	/// </summary>
	/// <param name="frequency">The frequency of the broadcast</param>
	/// <param name="row">The y-coordinate of the broadcast location</param>
	/// <param name="col">The x-coordinate of the broadcast location</param>
	public Antenna(char frequency, int row, int col)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(row, 0, nameof(row));
		ArgumentOutOfRangeException.ThrowIfLessThan(col, 0, nameof(col));
		Frequency = frequency;
		Coordinate = (row, col);
	}

	#region IEquatable implemenation

	public bool Equals(Antenna? other)
	{
		return other is not null &&
			other.Frequency != Frequency &&
			other.Coordinate.row != Coordinate.row &&
			other.Coordinate.col != Coordinate.col;
	}

	#endregion
}