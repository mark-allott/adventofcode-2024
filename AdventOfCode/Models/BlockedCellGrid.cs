using AdventOfCode.Interfaces;

namespace AdventOfCode.Models;

internal class BlockedCellGrid
	: GenericGrid<bool>
{
	public BlockedCellGrid(int x, int y)
		: base(x, y, new BlockedCellRenderer())
	{
	}

	public BlockedCellGrid(Coordinate coords)
		: base(coords, new BlockedCellRenderer())
	{
	}
}

internal class BlockedCellRenderer
	: ICellRenderer<bool>
{
	public char ToCharacter(bool value)
	{
		return value ? 'X' : ' ';
	}
}