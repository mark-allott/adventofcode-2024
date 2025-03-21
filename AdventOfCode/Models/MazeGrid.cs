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
}

internal class MazeCellTypeRenderer
	: ICellRenderer<MazeCellType>
{
	public char ToCharacter(MazeCellType value)
	{
		return value.ToCharacter();
	}
}