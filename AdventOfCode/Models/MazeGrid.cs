using AdventOfCode.Enums;
using AdventOfCode.Extensions;
using AdventOfCode.Interfaces;

namespace AdventOfCode.Models;

internal class MazeGrid
	: GenericGrid<MazeCellType>
{
	#region ctor

	public MazeGrid(int x, int y)
		: base(x, y, new MazeCellTypeRenderer())
	{
	}

	public MazeGrid(Coordinate coords)
		: base(coords, new MazeCellTypeRenderer())
	{
	}

	#endregion

	/// <summary>
	/// Produces a list of <see cref="IMazeNode"/> objects that represent the navigable parts of the maze
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns>The list of navigable nodes in the maze, including the start and end positions</returns>
	public (List<T> allNodes, T startNode, T endNode) GetMazeNodes<T>() where T : IMazeNode, new()
	{
		T startNode = default!;
		T endNode = default!;
		var allNodes = new List<T>();

		for (var y = 0; y < Bounds.Y; y++)
			for (var x = 0; x < Bounds.X; x++)
			{
				var location = new Coordinate(y, x);
				switch (this[location])
				{
					case MazeCellType.Empty:
						allNodes.Add(new T() { Location = location, Distance = int.MaxValue });
						break;
					case MazeCellType.End:
						endNode = new T() { Location = location, Distance = int.MaxValue };
						allNodes.Add(endNode);
						break;
					case MazeCellType.Start:
						startNode = new T() { Location = location, Distance = 0 };
						allNodes.Add(startNode);
						break;
					default:
						break;
				}
			}

		return (allNodes, startNode, endNode);
	}

	/// <summary>
	/// Simple but effective way of ensuring all cells in the grid are empty
	/// </summary>
	public void MakeGridEmpty()
	{
		for (var y = 0; y < Bounds.Y; y++)
			for (var x = 0; x < Bounds.X; x++)
				this[x, y] = MazeCellType.Empty;
	}
}

internal class MazeCellTypeRenderer
	: ICellRenderer<MazeCellType>
{
	public char ToCharacter(MazeCellType value)
	{
		return value.ToCharacter();
	}
}