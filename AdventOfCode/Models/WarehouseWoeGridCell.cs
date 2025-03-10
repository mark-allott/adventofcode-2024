using System.Numerics;
using AdventOfCode.Enums;

namespace AdventOfCode.Models;

internal class WarehouseWoeGridCell
	: GridObject<WarehouseWoeCellType>
{
	#region Properties

	/// <summary>
	/// GPS value is calculated as (X + 100Y)
	/// </summary>
	public int GPS => Descriptor == WarehouseWoeCellType.Box || Descriptor == WarehouseWoeCellType.BoxLeft
		? (int)(100 * Coord.Y + Coord.X)
		: 0;

	#endregion

	#region Ctor

	public WarehouseWoeGridCell(Vector2 coord, WarehouseWoeCellType cellType)
		: base(coord, cellType)
	{
		//	Coords cannot be negative
		ArgumentOutOfRangeException.ThrowIfNegative(coord.X, nameof(coord));
		ArgumentOutOfRangeException.ThrowIfNegative(coord.Y, nameof(coord));
		//	X-Coord cannot be 100 or greater for a box
		if (cellType == WarehouseWoeCellType.Box)
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(coord.X, 100, nameof(coord));
	}

	#endregion

	#region Methods

	/// <summary>
	/// Moves the cell to the specified location
	/// </summary>
	/// <param name="coord">The new coordinate for the cell</param>
	public void MoveTo(Vector2 coord)
	{
		//	Coords cannot be negative
		ArgumentOutOfRangeException.ThrowIfNegative(coord.X, nameof(coord));
		ArgumentOutOfRangeException.ThrowIfNegative(coord.Y, nameof(coord));

		//	X-Coord cannot be 100 or greater for a box
		if (Descriptor == WarehouseWoeCellType.Box)
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(coord.X, 100, nameof(coord));

		Coord = coord;
	}

	/// <summary>
	/// Moves the cell to the specified location using specific x and y coords
	/// </summary>
	/// <param name="x">The x-coord of the location</param>
	/// <param name="y">The y-coord of the location</param>
	public void MoveTo(int x, int y)
	{
		MoveTo(new Vector2(x, y));
	}

	/// <summary>
	/// Moves the cell to a new coordinate based on the <paramref name="offset"/> value
	/// </summary>
	/// <param name="offset">The offset location relative to the current location</param>
	public void MoveBy(Vector2 offset)
	{
		var newLocation = Coord + offset;
		MoveTo(newLocation);
	}

	#endregion
}