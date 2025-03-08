using System.Numerics;
using AdventOfCode.Enums;

namespace AdventOfCode.Extensions;

internal static class EnumExtensions
{
	/// <summary>
	/// Converts the character representation of the direction of travel to a <see cref="DirectionOfTravel"/>
	/// </summary>
	/// <param name="c">The character to transpose</param>
	/// <returns>The direction of travel</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static DirectionOfTravel ToDirectionOfTravel(this char c)
	{
		return c switch
		{
			'^' => DirectionOfTravel.North,
			'>' => DirectionOfTravel.East,
			'v' => DirectionOfTravel.South,
			'<' => DirectionOfTravel.West,
			_ => throw new ArgumentOutOfRangeException(nameof(c))
		};
	}

	/// <summary>
	/// Convert a direction of travel into a movement vector
	/// </summary>
	/// <param name="direction">The direction of travel</param>
	/// <returns>The vector offset for movements</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static Vector2 ToMovementOffset(this DirectionOfTravel direction)
	{
		return direction switch
		{
			DirectionOfTravel.North => new Vector2(0, -1),
			DirectionOfTravel.East => new Vector2(1, 0),
			DirectionOfTravel.South => new Vector2(0, 1),
			DirectionOfTravel.West => new Vector2(-1, 0),
			_ => throw new ArgumentOutOfRangeException(nameof(direction))
		};
	}

	/// <summary>
	/// Helper top convert the values on the map to cell types
	/// </summary>
	/// <param name="c">The character to transpose</param>
	/// <returns>The type of cell</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static WarehouseWoeCellType ToWarehouseWoeCellType(this char c)
	{
		return c switch
		{
			'.' => WarehouseWoeCellType.Empty,
			'#' => WarehouseWoeCellType.Wall,
			'O' => WarehouseWoeCellType.Box,
			'@' => WarehouseWoeCellType.Robot,
			_ => throw new ArgumentOutOfRangeException(nameof(c))
		};
	}

	/// <summary>
	/// Converts the cell type into a character representation
	/// </summary>
	/// <param name="cellType">The cell type</param>
	/// <returns>The character representing the cell type</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static char ToCharacter(this WarehouseWoeCellType cellType)
	{
		return cellType switch
		{
			WarehouseWoeCellType.Empty => '.',
			WarehouseWoeCellType.Wall => '#',
			WarehouseWoeCellType.Box => 'O',
			WarehouseWoeCellType.Robot => '@',
			_ => throw new ArgumentOutOfRangeException(nameof(cellType))
		};
	}
}