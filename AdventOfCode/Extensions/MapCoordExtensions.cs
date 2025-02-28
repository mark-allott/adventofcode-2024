using AdventOfCode.Models;

namespace AdventOfCode.Extensions;

internal static class MapCoordExtensions
{
	/// <summary>
	/// Extension method to add an offset to an existing coordinate
	/// </summary>
	/// <param name="coord">The current coordinate</param>
	/// <param name="rowOffset">The number of rows to offset by (Y-coords)</param>
	/// <param name="columnOffset">The number of columns to offset by (X-coords)</param>
	/// <returns>The update coordinate</returns>
	public static MapCoord OffsetBy(this MapCoord coord, int rowOffset, int columnOffset)
	{
		return new MapCoord(coord.Y + rowOffset, coord.X + columnOffset);
	}
}