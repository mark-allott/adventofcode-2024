using AdventOfCode.Enums;
using AdventOfCode.Interfaces;

namespace AdventOfCode.Models;

internal class ReindeerMazeDistanceStrategy
	: IDijkstraDistanceStrategy<MazeMovement>
{
	/// <inheritdoc/>
	public int GetDistance(MazeMovement movement)
	{
		return movement switch
		{
			MazeMovement.GoForward => 1,
			MazeMovement.TurnLeft => 1001,
			MazeMovement.TurnRight => 1001,
			_ => throw new ArgumentOutOfRangeException(nameof(movement), movement, $"Not a permissible value"),
		};
	}
}