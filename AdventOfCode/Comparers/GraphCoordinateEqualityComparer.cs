using System.Diagnostics.CodeAnalysis;
using AdventOfCode.Interfaces;

namespace AdventOfCode.Comparers;

/// <summary>
/// Implementation of a comparer for the <see cref="IGraphCoordinate"/> interface
/// </summary>
internal class GraphCoordinateEqualityComparer
	: IEqualityComparer<IGraphCoordinate>
{
	public bool Equals(IGraphCoordinate? a, IGraphCoordinate? b)
	{
		return b is null
			? a is null
			: a?.X == b.X && a.Y == b.Y;
	}

	public int GetHashCode([DisallowNull] IGraphCoordinate obj)
	{
		return HashCode.Combine(obj.X, obj.Y);
	}
}