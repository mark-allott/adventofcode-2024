using AdventOfCode.Enums;
using AdventOfCode.Extensions;
using AdventOfCode.Interfaces;

namespace AdventOfCode.Models;

internal class ReindeerMazeGrid
	: GenericGrid<ReindeerMazeCellType>
{
	#region ctor

	public ReindeerMazeGrid(int x, int y)
		: base(x, y, new ReindeerMazeCellRenderer())
	{
	}

	public ReindeerMazeGrid(MapCoord coords)
		: base(coords, new ReindeerMazeCellRenderer())
	{
	}

	#endregion
}

internal class ReindeerMazeCellRenderer
	: ICellRenderer<ReindeerMazeCellType>
{
	public char ToCharacter(ReindeerMazeCellType value)
	{
		return value.ToCharacter();
	}
}