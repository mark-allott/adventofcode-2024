using System.Numerics;

namespace AdventOfCode.Models;

internal class GridObject<T>
{
	public Vector2 Coord { get; }

	public T Descriptor { get; } = default!;

	public GridObject(Vector2 coord, T descriptor)
	{
		Coord = coord;
		Descriptor = descriptor;
	}
}