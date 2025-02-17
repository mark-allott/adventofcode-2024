using AdventOfCode.Enums;

namespace AdventOfCode.Models;

internal class WordGrid
{
	/// <summary>
	/// Holds details of the characters in the grid
	/// </summary>
	private List<List<char>> _grid = new List<List<char>>();

	/// <summary>
	/// Used to determine the maximum number of rows for the grid
	/// </summary>
	private int _rowCount = 0;

	/// <summary>
	/// Use to determine the maximum number of columns in the grid
	/// </summary>
	private int _columnCount = 0;

	/// <summary>
	/// Loads the grid from a list of strings
	/// </summary>
	/// <param name="strings">The source of the grid characters</param>
	/// <exception cref="ArgumentException"></exception>
	/// <remarks>If all rows do not have the same number of columns, the load process fails (ragged rows are not permitted)</remarks>
	public void LoadFromEnumerableOfString(IEnumerable<string> strings)
	{
		_grid.Clear();
		strings.ToList().ForEach(s => _grid.Add(new List<char>(s.ToCharArray())));

		var gridLengths = _grid.Select(s => s.Count).ToList();
		if (!gridLengths.All(a => a == _grid[0].Count))
			throw new ArgumentException("Grid has unequal line lengths", nameof(strings));

		//	Set the counters for maximum values
		_rowCount = gridLengths.Count;
		_columnCount = gridLengths[0];
	}

	/// <summary>
	/// Returns the next co-ordinate for searching along the word based on the current location and direction of search
	/// </summary>
	/// <param name="row">The current row being searched</param>
	/// <param name="column">The current column being searched</param>
	/// <param name="direction">The direction of travel</param>
	/// <returns>The next co-ordinate</returns>
	private (int row, int column) GetNextCoord(int row, int column, WordGridDirection direction)
	{
		switch (direction)
		{
			case WordGridDirection.North: return (row - 1, column);
			case WordGridDirection.NorthEast: return (row - 1, column + 1);
			case WordGridDirection.East: return (row, column + 1);
			case WordGridDirection.SouthEast: return (row + 1, column + 1);
			case WordGridDirection.South: return (row + 1, column);
			case WordGridDirection.SouthWest: return (row + 1, column - 1);
			case WordGridDirection.West: return (row, column - 1);
			case WordGridDirection.NorthWest: return (row - 1, column - 1);
			default: throw new ArgumentOutOfRangeException(nameof(direction), "Unknown is not a valid direction of travel");
		}
	}

	/// <summary>
	/// Performs the grid search, looking for instances of the <paramref name="word"/> within the grid
	/// </summary>
	/// <param name="word">The word to be searched for</param>
	/// <returns>A list of found instances of the <paramref name="word"/> in the grid, where they are located and which direction the match travels</returns>
	public List<WordGridResult> SearchGridForWord(string word)
	{
		var gridResults = new List<WordGridResult>();

		//	Although resulting in a large search (8 * x * y iterations maximum),
		//	this is the maximum theoretical limit for the number of iterations
		//	and will actually result in a much lower number being performed.
		//	Most searches will fail due to:
		//		* co-ordinates being "out of bounds"
		//		* the location not containing the correct character
		for (int i = 0; i < _rowCount; i++)
			for (var j = 0; j < _columnCount; j++)
			{
				foreach (var direction in directions)
				{
					if (IsPhraseAtCoord(i, j, direction, word))
						gridResults.Add(new WordGridResult(i, j, direction));
				}
			}

		return gridResults;
	}

	//	Provide a fixed list of the valid directions for the search of the word
	private readonly WordGridDirection[] directions = new[]
	{
		WordGridDirection.North,
		WordGridDirection.NorthEast,
		WordGridDirection.East,
		WordGridDirection.SouthEast,
		WordGridDirection.South,
		WordGridDirection.SouthWest,
		WordGridDirection.West,
		WordGridDirection.NorthWest
	};

	/// <summary>
	/// Determines whether the phrase being looked for is contained at the specified location
	/// </summary>
	/// <param name="row">The x-coordinate for the start of the search</param>
	/// <param name="column">The y-coordinate for the start of the search</param>
	/// <param name="direction">Which direction the search is taking place in</param>
	/// <param name="phrase">The word to be found</param>
	/// <returns>True if the word was found, otherwise false</returns>
	/// <remarks>
	/// The method is recursive in nature, calling itself with successively
	/// fewer characters until all characters have been found in the given
	/// direction of travel.
	/// </remarks>
	private bool IsPhraseAtCoord(int row, int column, WordGridDirection direction, string phrase)
	{
		//	Get the character at position row,column and compare to first
		var charAtCoord = _grid[row][column];
		if (!charAtCoord.Equals(phrase.ToCharArray()[0]))
			return false;

		//	If only one character remains, we have a match
		if (phrase.Length == 1)
			return true;

		//	Find the next co-ordinate for checks
		var (newRow, newColumn) = GetNextCoord(row, column, direction);
		//	if the new co-ordinate is out of bounds, we have no match
		if (newRow < 0 || newRow >= _rowCount ||
			newColumn < 0 || newColumn >= _columnCount)
			return false;

		//	Continue to search at next co-ordinate, using same direction and one character less in the phrase
		return IsPhraseAtCoord(newRow, newColumn, direction, phrase[1..]);
	}
}