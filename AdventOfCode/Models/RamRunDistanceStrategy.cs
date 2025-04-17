using AdventOfCode.Enums;
using AdventOfCode.Interfaces;

namespace AdventOfCode.Models;

internal class RamRunDistanceStrategy
	: IDijkstraDistanceStrategy<MazeMovement>
{
	/// <inheritdoc/>
	public int GetDistance(MazeMovement movement)
	{
		return movement switch
		{
			MazeMovement.GoForward => 1,
			MazeMovement.TurnLeft => 1,
			MazeMovement.TurnRight => 1,
			_ => throw new ArgumentOutOfRangeException(nameof(movement), movement, $"Not a permissible value"),
		};
	}
}