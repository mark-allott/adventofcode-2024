using AdventOfCode.Enums;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day04;

public partial class Day04
	: AbstractDailyChallenge, IAutoRegister
{
	#region Overrides to run part two of challenge

	/// <summary>
	/// Overrides the base implementation to provide the solution for part two
	/// </summary>
	/// <returns></returns>
	protected override bool PartTwo()
	{
		LoadAndReadFile();

		var grid = new WordGrid();
		grid.LoadFromEnumerableOfString(InputFileLines);

		var results = grid.SearchGridForWord("MAS");
		var onlyDiagonalResults = FindDiagonalResults(results);
		var xmasResults = FindXmasResults(onlyDiagonalResults, grid.MaxRow, grid.MaxColumn);
		var total = xmasResults.Count;
		PartTwoResult = $"The number of X-MASes found is {total}";
		return true;
	}

	#endregion

	//	Provide a fixed list of the valid directions for the search of the word
	private readonly WordGridDirection[] diagonalDirections = new[]
	{
		WordGridDirection.NorthEast,
		WordGridDirection.SouthEast,
		WordGridDirection.SouthWest,
		WordGridDirection.NorthWest
	};

	/// <summary>
	/// Filter the results to return only those that match the phrase and are diagonal (so can form a stroke of an X)
	/// </summary>
	/// <param name="results">The incoming matches</param>
	/// <returns>The matches that have diagonal directions</returns>
	private List<WordGridResult> FindDiagonalResults(List<WordGridResult> results)
	{
		//	Must have something to work with...
		ArgumentNullException.ThrowIfNull(results, nameof(results));

		//	If no matches exist, there cannot be any results
		if (results.Count == 0)
			return null!;

		//	Locate only the results which are in diagonal directions (and can therefore form an X-MAS)
		return results.Where(q => diagonalDirections.Contains(q.Direction))
			.OrderBy(q => q.RowNumber)
			.ThenBy(q => q.ColumnNumber)
			.ToList();
	}

	/// <summary>
	/// Provide a list of matches that will result in the two strokes of the "X" for an X-MAS match
	/// </summary>
	/// <param name="diagonalResults">The subset of results that have only diagonal directions</param>
	/// <param name="maxRow">The maximum grid row</param>
	/// <param name="maxColumn">The maximum grid column</param>
	/// <returns>The list of X-MAS matches</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	private List<(WordGridResult one, WordGridResult two)> FindXmasResults(List<WordGridResult> diagonalResults, int maxRow, int maxColumn)
	{
		//	Verify we do not have any horizontal or vertical components in the list
		if (diagonalResults.Any(q => !diagonalDirections.Contains(q.Direction)))
			throw new ArgumentOutOfRangeException(nameof(diagonalResults), "Results contains non-diagonal entries");

		//	Placeholder for the X-MAS matches
		var results = new List<(WordGridResult one, WordGridResult two)>();

		//	for each result we have check the possible matches to make a X-MAS shape
		foreach (var result in diagonalResults)
		{
			//	Start with blank results
			List<WordGridResult> pairedResults = null!;

			//	Based on the direction of the current result, we know the matching pair of possibilities that make up both strokes of an "X" shape
			switch (result.Direction)
			{
				case WordGridDirection.NorthEast:
					pairedResults = CheckForPairedResult(result, [WordGridDirection.SouthEast, WordGridDirection.NorthWest], diagonalResults, maxRow, maxColumn);
					break;
				case WordGridDirection.SouthEast:
					pairedResults = CheckForPairedResult(result, [WordGridDirection.SouthWest, WordGridDirection.NorthEast], diagonalResults, maxRow, maxColumn);
					break;
				case WordGridDirection.SouthWest:
					pairedResults = CheckForPairedResult(result, [WordGridDirection.SouthEast, WordGridDirection.NorthWest], diagonalResults, maxRow, maxColumn);
					break;
				case WordGridDirection.NorthWest:
					pairedResults = CheckForPairedResult(result, [WordGridDirection.NorthEast, WordGridDirection.SouthWest], diagonalResults, maxRow, maxColumn);
					break;
			}

			//	Check the results passed back
			if (pairedResults is not null && pairedResults.Count > 0)
			{
				//	for each paired result, order consistently so they can be verified unique later
				foreach (var pairedResult in pairedResults)
				{
					var entry = new List<WordGridResult>() { result, pairedResult };
					entry = entry.OrderBy(o => o.ColumnNumber)
						.ThenBy(o => o.RowNumber)
						.ToList();
					results.Add((entry[0], entry[1]));
				}
			}
		}

		//	Return only unique combinations
		return results.Distinct().ToList();
	}

	/// <summary>
	/// For a given result, check the other possible locations in the grid for a corresponding match that forms another stroke of the "X"
	/// </summary>
	/// <param name="result">The current match being tested</param>
	/// <param name="directions">The other directions in which to check</param>
	/// <param name="allResults">All results</param>
	/// <param name="maxRow">The maximum number of rows in the grid</param>
	/// <param name="maxColumn">The maximum number of columns in the grid</param>
	/// <returns>Any other matches that can be paired with <paramref name="result"/></returns>
	private List<WordGridResult> CheckForPairedResult(WordGridResult result, WordGridDirection[] directions, List<WordGridResult> allResults, int maxRow, int maxColumn)
	{
		var pairedResults = new List<WordGridResult>();

		foreach (var direction in directions)
		{
			//	Get the new coords from current entry in relevant direction
			var newCoords = GetNewCoords(result, direction);

			//	Only proceed if valid coords returned
			if (newCoords.x < 0 || newCoords.x > maxRow ||
				newCoords.y < 0 || newCoords.y > maxColumn)
				continue;

			//	There should only ever be a single result in the location specified and the required direction
			var testResult = allResults
				.SingleOrDefault(q => q.ColumnNumber == newCoords.x &&
										q.RowNumber == newCoords.y &&
										q.Direction == direction);
			if (testResult is not null)
				pairedResults.Add(testResult);
		}
		return pairedResults;
	}

	/// <summary>
	/// Based on the current <paramref name="result"/> and <paramref name="direction"/>,
	/// calculate the new co-ordinates in the grid where a match should be
	/// located to form a matching stroke of the "X"
	/// </summary>
	/// <param name="result">One part of the "X"</param>
	/// <param name="direction">The direction to match with</param>
	/// <returns>New co-ordinates in the grid, or (-1, -1) to indicate a bad request</returns>
	private (int x, int y) GetNewCoords(WordGridResult result, WordGridDirection direction)
	{
		//	Results are based on the direction of the current result and allow only certain combinations
		return result.Direction switch
		{
			WordGridDirection.NorthEast => direction switch
			{
				WordGridDirection.SouthEast => (result.ColumnNumber, result.RowNumber - 2),
				WordGridDirection.NorthWest => (result.ColumnNumber + 2, result.RowNumber),
				_ => (-1, -1)
			},
			WordGridDirection.SouthEast => direction switch
			{
				WordGridDirection.SouthWest => (result.ColumnNumber + 2, result.RowNumber),
				WordGridDirection.NorthEast => (result.ColumnNumber, result.RowNumber + 2),
				_ => (-1, -1)
			},
			WordGridDirection.SouthWest => direction switch
			{
				WordGridDirection.SouthEast => (result.ColumnNumber - 2, result.RowNumber),
				WordGridDirection.NorthWest => (result.ColumnNumber, result.RowNumber + 2),
				_ => (-1, -1)
			},
			WordGridDirection.NorthWest => direction switch
			{
				WordGridDirection.NorthEast => (result.ColumnNumber - 2, result.RowNumber),
				WordGridDirection.SouthWest => (result.ColumnNumber, result.RowNumber - 2),
				_ => (-1, -1)
			},
			_ => (-1, -1),
		};
	}
}