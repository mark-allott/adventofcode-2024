namespace AdventOfCode.Interfaces;

internal interface IDijkstraDistanceStrategy<T>
	where T : struct
{
	/// <summary>
	/// Returns the distance value for a given movement
	/// </summary>
	/// <param name="movement">The movement being made</param>
	/// <returns>A distance value</returns>
	int GetDistance(T movement);
}
