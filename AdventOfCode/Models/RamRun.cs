using AdventOfCode.Enums;

namespace AdventOfCode.Models;

internal class RamRun
{
	#region Fields

	private readonly MazeGrid _mazeGrid;

	#endregion

	#region Properties
	#endregion

	#region Constructors

	public RamRun(int width, int height)
	{
		_mazeGrid = new MazeGrid(width, height);

		//	Start location is top-left corner
		_mazeGrid[0, 0] = MazeCellType.Start;
		//	End location is bottom-right corner
		_mazeGrid[width - 1, height - 1] = MazeCellType.End;
	}

	#endregion

	#region Overloads / Overrides

	/// <summary>
	/// Debug helper to pretty-print the maze
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		return _mazeGrid.ToString();
	}

	#endregion

	#region  Methods

	/// <summary>
	/// Loads a set up corrupted bytes into the maze grid up to the <paramref name="maximum"/> number of bytes
	/// </summary>
	/// <param name="coordinates">The coordinates of corrupted memory</param>
	/// <param name="maximum">The maximum number of bytes to corrupt</param>
	public void LoadCorruption(List<Coordinate> coordinates, int maximum)
	{
		var loaded = 0;
		foreach (Coordinate coordinate in coordinates)
		{
			if (SetCorruptedCoordinate(coordinate))
			{
				loaded++;
				if (loaded >= maximum)
					break;
			}
		}
	}

	/// <summary>
	/// Sets the location at <paramref name="coordinate"/> as corrupted
	/// </summary>
	/// <param name="coordinate">The coordinate of the location</param>
	/// <returns>True if the memory location was corrupted, otherwise false</returns>
	public bool SetCorruptedCoordinate(Coordinate coordinate)
	{
		if (!coordinate.InBounds(_mazeGrid.Bounds))
			return false;
		_mazeGrid[coordinate] = MazeCellType.Wall;
		return true;
	}

	/// <summary>
	/// Returns the shortest number of steps required to solve the maze
	/// </summary>
	/// <returns>The number of steps needed, or <see cref="int.MaxValue"/> if no solution is found</returns>
	public int GetShortestPath()
	{
		var solver = new DijkstraMazeSolver(_mazeGrid);
		var strategy = new RamRunDistanceStrategy();

		return solver.Solve(DirectionOfTravel.East, null!, strategy);
	}

	#endregion
}