using AdventOfCode.Interfaces;

namespace AdventOfCode.Models;

/// <summary>
/// Concrete implementation that calculates the distance between nodes on different planes
/// </summary>
internal class DiagonalMoveRangeStrategy
	: IGraphCoordinateRangeStrategy
{
	/// <inheritdoc/>
	public int Range(IGraphCoordinate from, IGraphCoordinate to)
	{
		//	From / To must be defined
		ArgumentNullException.ThrowIfNull(from, nameof(from));
		ArgumentNullException.ThrowIfNull(to, nameof(to));

		int dX = Math.Abs(to.X - from.X);
		int dY = Math.Abs(to.Y - from.Y);

		return dX + dY;
	}
}