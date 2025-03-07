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
}