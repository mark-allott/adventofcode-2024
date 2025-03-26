using System.Diagnostics.CodeAnalysis;
using AdventOfCode.Models;

namespace AdventOfCode.Comparers;

/// <summary>
/// Comparer for the <see cref="Coordinate"/> class
/// </summary>
internal class CoordinateEqualityComparer
	: IEqualityComparer<Coordinate>
{
	#region IEqualityComparer implementation

	public bool Equals(Coordinate? a, Coordinate? b)
	{
		return b is null
			? a is null
			: a?.X == b.X && a.Y == b.Y;
	}

	public int GetHashCode([DisallowNull] Coordinate obj)
	{
		return HashCode.Combine(obj.X, obj.Y);
	}

	#endregion
}