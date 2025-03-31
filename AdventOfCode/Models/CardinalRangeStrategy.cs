using AdventOfCode.Interfaces;

namespace AdventOfCode.Models;

/// <summary>
/// Concrete implementation of <see cref="IGraphCoordinateRangeStrategy"/>, where ranges are calculated using only cardinal point directions.
/// </summary>
internal class CardinalRangeStrategy
	: IGraphCoordinateRangeStrategy
{
	/// <inheritdoc/>
	/// <remarks>
	/// This will return valid ranges provided there is no diagonal movement
	/// between the nodes. Each node must be located within the same X or Y
	/// coordinate
	/// </remarks>
	public int Range(IGraphCoordinate from, IGraphCoordinate to)
	{
		//	From / To must be defined
		ArgumentNullException.ThrowIfNull(from, nameof(from));
		ArgumentNullException.ThrowIfNull(to, nameof(to));

		int dX = Math.Abs(to.X - from.X);
		int dY = Math.Abs(to.Y - from.Y);

		//	If both cardinal ranges are greater than zero then there's a diagonal element to the movement
		if (dX > 0 && dY > 0)
			throw new ArgumentOutOfRangeException($"Range has more than one direction");

		return dX == 0
			? dY
			: dX;
	}
}
