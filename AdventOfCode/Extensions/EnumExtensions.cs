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
			_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, $"{nameof(ToMovementOffset)}")
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
			'[' => WarehouseWoeCellType.BoxLeft,
			']' => WarehouseWoeCellType.BoxRight,
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
			WarehouseWoeCellType.BoxLeft => '[',
			WarehouseWoeCellType.BoxRight => ']',
			_ => throw new ArgumentOutOfRangeException(nameof(cellType))
		};
	}

	/// <summary>
	/// Converts a character on the maze map into a <see cref="MazeCellType"/>
	/// </summary>
	/// <param name="c">The character to transpose</param>
	/// <returns>The appropriate <see cref="MazeCellType"/></returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static MazeCellType ToMazeCellType(this char c)
	{
		return c switch
		{
			'.' => MazeCellType.Empty,
			'#' => MazeCellType.Wall,
			'S' => MazeCellType.Start,
			'E' => MazeCellType.End,
			_ => throw new ArgumentOutOfRangeException(nameof(c))
		};
	}

	/// <summary>
	/// Converts the <paramref name="cellType"/> value into a character representation
	/// </summary>
	/// <param name="cellType">The value to transpose</param>
	/// <returns>The character that represents <paramref name="cellType"/></returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static char ToCharacter(this MazeCellType cellType)
	{
		return cellType switch
		{
			MazeCellType.Empty => '.',
			MazeCellType.Wall => '#',
			MazeCellType.Start => 'S',
			MazeCellType.End => 'E',
			_ => throw new ArgumentOutOfRangeException(nameof(cellType))
		};
	}

	/// <summary>
	/// Converts a <see cref="MazeMovement"/> and <see cref="DirectionOfTravel"/> into a new direction
	/// </summary>
	/// <param name="move">The move to be made</param>
	/// <param name="currentDirection">The current direction of travel</param>
	/// <returns>The new direction of travel after executing the move</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static DirectionOfTravel ToDirectionOfTravel(this MazeMovement move, DirectionOfTravel currentDirection)
	{
		return move switch
		{
			MazeMovement.GoForward => currentDirection,
			MazeMovement.TurnLeft => currentDirection switch
			{
				DirectionOfTravel.North => DirectionOfTravel.West,
				DirectionOfTravel.East => DirectionOfTravel.North,
				DirectionOfTravel.South => DirectionOfTravel.East,
				DirectionOfTravel.West => DirectionOfTravel.South,
				_ => throw new ArgumentOutOfRangeException(nameof(currentDirection))
			},
			MazeMovement.TurnRight => currentDirection switch
			{
				DirectionOfTravel.North => DirectionOfTravel.East,
				DirectionOfTravel.East => DirectionOfTravel.South,
				DirectionOfTravel.South => DirectionOfTravel.West,
				DirectionOfTravel.West => DirectionOfTravel.North,
				_ => throw new ArgumentOutOfRangeException(nameof(currentDirection))
			},
			_ => throw new ArgumentOutOfRangeException(nameof(move))
		};
	}

	/// <summary>
	/// Converts a <see cref="DirectionOfTravel"/> value into an offset of x and y coords
	/// </summary>
	/// <param name="direction">The direction of travel</param>
	/// <returns>The offset in coords to apply to a <see cref="MapCoord"/> or similar</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static (int xOffset, int yOffset) ToMapCoordOffset(this DirectionOfTravel direction)
	{
		return direction switch
		{
			DirectionOfTravel.North => (0, -1),
			DirectionOfTravel.East => (1, 0),
			DirectionOfTravel.South => (0, 1),
			DirectionOfTravel.West => (-1, 0),
			_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, $"{nameof(ToMapCoordOffset)}")
		};
	}

	/// <summary>
	/// Converts a <see cref="DirectionOfTravel"/> value into an offset of x and y coords in a reverse direction
	/// </summary>
	/// <param name="direction">The direction of travel to be reversed</param>
	/// <returns>The offset in coords to apply to a <see cref="MapCoord"/> or similar</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static (int xOffset, int yOffset) ToReverseOffset(this DirectionOfTravel direction)
	{
		return direction switch
		{
			DirectionOfTravel.North => (0, 1),
			DirectionOfTravel.East => (-1, 0),
			DirectionOfTravel.South => (0, -1),
			DirectionOfTravel.West => (1, 0),
			_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, $"{nameof(ToMapCoordOffset)}")
		};
	}
}