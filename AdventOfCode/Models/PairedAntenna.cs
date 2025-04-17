namespace AdventOfCode.Models;

internal class PairedAntenna
{
	private Antenna _broadcast;
	private Antenna _paired;
	private (int maxRow, int maxCol) _bounds;


	/// <summary>
	/// ctor - takes the pair of <see cref="Antenna"/> and bounds for the map size
	/// </summary>
	/// <param name="broadcast">The broadcasting antenna</param>
	/// <param name="paired">The paired antenna</param>
	/// <param name="bounds">The upper bounds of the map</param>
	public PairedAntenna(Antenna broadcast, Antenna paired, (int maxRow, int maxCol) bounds)
	{
		ArgumentNullException.ThrowIfNull(broadcast, nameof(broadcast));
		ArgumentNullException.ThrowIfNull(paired, nameof(paired));
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bounds.maxRow, nameof(bounds.maxRow));
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bounds.maxCol, nameof(bounds.maxCol));

		_broadcast = broadcast;
		_paired = paired;
		_bounds = bounds;
	}

	/// <summary>
	/// Method to calculate the location of the antinodes for the anntena pair
	/// </summary>
	/// <returns>The list of antinodes</returns>
	public List<Antinode> GetAntinodes()
	{
		var antinodes = new List<Antinode>();

		var offsetRow = _paired.Coordinate.row - _broadcast.Coordinate.row;
		var offsetColumn = _paired.Coordinate.col - _broadcast.Coordinate.col;

		var nodeRow = _broadcast.Coordinate.row + offsetRow;
		var nodeCol = _broadcast.Coordinate.col + offsetColumn;

		while (nodeRow >= 0 && nodeRow < _bounds.maxRow &&
				nodeCol >= 0 && nodeCol < _bounds.maxCol)
		{
			antinodes.Add(new Antinode(_broadcast, _paired, nodeRow, nodeCol, _bounds));
			nodeRow += offsetRow;
			nodeCol += offsetColumn;
		}
		return antinodes;
	}
}