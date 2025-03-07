using System.Numerics;
using System.Text;

namespace AdventOfCode.Models;

internal class SecurityBotGrid
{
	#region Fields

	private readonly Vector2 bounds;

	private readonly List<SecurityBot> _bots = new List<SecurityBot>();

	#endregion

	#region Properties

	public Vector2 GridBounds => bounds;

	public long SafetyFactor => GetSafetyFactor();

	public int BotCount => _bots.Count;

	#endregion

	#region Constructors

	public SecurityBotGrid(int width, int height)
	{
		bounds = new Vector2(width, height);
	}

	#endregion

	#region Methods

	/// <summary>
	/// Adds a new bot to the grid
	/// </summary>
	/// <param name="x">X-Coord of start location</param>
	/// <param name="y">Y-Coord of start location</param>
	/// <param name="dx">X-offset of velocity</param>
	/// <param name="dy">Y-offset of velocity</param>
	public SecurityBot AddBot(int x, int y, int dx, int dy)
	{
		var bot = new SecurityBot(x, y, dx, dy, this);
		_bots.Add(bot);

		return bot;
	}

	/// <summary>
	/// Moves all bots on the grid by the specified number of moves
	/// </summary>
	/// <param name="count">The number of moves to be made</param>
	public void Move(int count = 1)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count, nameof(count));
		_bots.ForEach(b => b.MoveFor(count));
	}

	/// <summary>
	/// Calculate the safety factor by multiplying the number of bots in each quadrant
	/// </summary>
	/// <returns>The safety factor of the grid</returns>
	private long GetSafetyFactor()
	{
		var quadrants = _bots
			.Where(q => q.Quadrant != 0)
			.GroupBy(g => g.Quadrant)
			.Select(g => g.ToList().Count)
			.ToList();
		var result = 1L;

		quadrants.ForEach(i => result *= i);
		return result;
	}

	/// <summary>
	/// Performs a reset of the grid, putting all bots to their start positions as if they have never moved
	/// </summary>
	public void Reset()
	{
		_bots.ForEach(b => b.Reset());
	}

	/// <summary>
	/// Helper class to hold details of bot positions in the grid that are all on the same row
	/// </summary>
	private class OrderedBots
	{
		public int Row { get; set; }
		public List<int> XCoords { get; set; } = null!;

		public override string ToString()
		{
			return $"{Row}: [{(XCoords is null || XCoords.Count == 0 ? "" : string.Join(", ", XCoords))}]";
		}
	}

	/// <summary>
	/// Method to "locate" a possible christmas tree shape in the bot movement(s)
	/// </summary>
	/// <param name="depth">How many rows deep to stop searching for a match</param>
	/// <returns>The number of moves needed to show the shape</returns>
	public int ElapsedUntilEasterEgg(int depth = 0)
	{
		//	Keep working until the "tree" has been found
		var working = true;
		do
		{
			//	Perform moves one at a time to locate the shape
			Move();

			//	Extract bot positions into a new helper class
			//	Helper class holds x-coords for all bots on the given row
			var orderedBots = _bots
				.GroupBy(g => g.CurrentPosition.Y)
				.Select(s => new OrderedBots() { Row = (int)s.Key, XCoords = s.Select(s => (int)s.CurrentPosition.X).OrderBy(o => o).ToList() })
				.OrderBy(o => o.Row)
				.ToList();

			//	Loop for all rows of bots
			orderedBots.ForEach(i =>
			{
				//	Check the current "top" position of the tree
				if (CheckPositions(i.Row, depth, orderedBots))
				{
					//	Found a possible "tree" - stop work and report it
					working = false;
					return;
				}
			});
		} while (working);

		//	Pick the first bot and return the number of moves that have been made
		return _bots[0].MovesMade;
	}

	/// <summary>
	/// Helper class to check the grid for a possible "tree"
	/// </summary>
	/// <param name="row">The row being checked (at the top of the tree)</param>
	/// <param name="depth">How many rows need to be checked to ensure a "tree" is present</param>
	/// <param name="bots">The list of <see cref="OrderedBots"/> objects with bot positions</param>
	/// <returns></returns>
	private bool CheckPositions(int row, int depth, List<OrderedBots> bots)
	{
		//	Get the row being worked on
		var botRow = bots.FirstOrDefault(q => q.Row == row);

		//	If the row results in a null, then there's a problem!
		ArgumentNullException.ThrowIfNull(botRow, nameof(row));

		//	loop for each x-coord in the row
		foreach (var x in botRow.XCoords)
		{
			//	If a "tree" was found, stop now
			if (CheckOuterPoints(row, x, depth, bots))
				return true;
		}

		//	All checked, no "tree" found
		return false;
	}

	/// <summary>
	/// Recursive method to walk along the outer edges of the tree shape
	/// </summary>
	/// <param name="row">The row being checked</param>
	/// <param name="centre">The x-coord of the centre-point of the tree</param>
	/// <param name="depth">How many more rows need to be checked</param>
	/// <param name="bots">The list of bots in order</param>
	/// <returns></returns>
	private bool CheckOuterPoints(int row, int centre, int depth, List<OrderedBots> bots)
	{
		//	Get the row being checked
		var botRow = bots.FirstOrDefault(q => q.Row == row);

		//	If it is null, then we must be out of bounds or there's no bots on that row
		//	Either way, check fails now.
		if (botRow is null)
			return false;

		//	If we are the "top most" depth - i.e. the top of the tree, then check that the centre position is found
		if (depth == 0)
			return botRow.XCoords.Contains(centre);

		//	Next, check the next "depth" rows are contiguous
		//	Starting at row, for depth iterations, get the range of values to match
		var contiguousRowNumbers = Enumerable.Range(row, depth).ToList();
		//	bots must be present on all rows to continue checks
		if (!contiguousRowNumbers.All(q => bots.Any(a => a.Row == q)))
			return false;

		//	Check if the edges of the tree are present at "depth" x-coords to
		//	the left and right of centre and continue down the tree, checking
		//	the next row down also contains bots at the required positions
		return botRow.XCoords.Contains(centre - depth) &&
				botRow.XCoords.Contains(centre + depth) &&
				CheckOuterPoints(row + 1, centre, depth - 1, bots);
	}

	/// <summary>
	/// Helper method to output the current positions of bots in the grid
	/// </summary>
	public void WriteResultToFile()
	{
		//	Initialise the container for the textual representation of the map
		var output = new StringBuilder($"{DateTime.UtcNow:O}: output from {nameof(SecurityBotGrid)}.{nameof(WriteResultToFile)}" + Environment.NewLine);
		var rowNum = 0;

		//	Create an ordered list of bot positions
		var orderedBots = _bots
			.GroupBy(g => g.CurrentPosition.Y)
			.Select(s => new OrderedBots() { Row = (int)s.Key, XCoords = s.Select(s => (int)s.CurrentPosition.X).OrderBy(o => o).ToList() })
			.OrderBy(o => o.Row)
			.ToList();

		//	loop for all rows in the ordered list
		foreach (var botRow in orderedBots)
		{
			//	If not at the row expected, add blank lines until we are
			while (rowNum < botRow.Row)
			{
				output.AppendLine("");
				rowNum++;
			}

			//	Create a buffer to hold points on the row where a bot is present
			var line = new char[(int)bounds.X];
			//	Loop along the row, adding the appropriate character in the location
			for (var i = 0; i < line.Length; i++)
				line[i] = botRow.XCoords.Contains(i)
					? '*'
					: ' ';
			//	Append the new row to the output
			output.AppendLine(new string(line));
			rowNum++;
		}

		//	Write the map detail to a file
		var cwd = Directory.GetCurrentDirectory();
		var outputFilePath = Path.Combine(cwd, "data", "debug", $"BotGrid-{_bots[0].MovesMade}.txt");

		if (File.Exists(outputFilePath))
			File.Delete(outputFilePath);

		File.WriteAllText(outputFilePath, output.ToString());
	}

	#endregion
}