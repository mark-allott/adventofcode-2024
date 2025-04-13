
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.Enums;
using AdventOfCode.Extensions;

namespace AdventOfCode.Models;

internal class KeypadConundrum
{
	#region Fields

	/// <summary>
	/// Holds details for the numeric keypad - which key is in which coordinate
	/// </summary>
	private Dictionary<char, Coordinate> _numericKeypad = null!;

	/// <summary>
	/// Holds details of the directions needed to move from one key to any other on the numeric keypad
	/// </summary>
	private Dictionary<char, Dictionary<char, string>> _numericDirections = null!;

	/// <summary>
	/// Holds details for the directional keypad - which key is in which coordinate
	/// </summary>
	private Dictionary<char, Coordinate> _directionalKeypad = null!;

	/// <summary>
	/// Holds details of the directions needed to move from one directional key to any other on the directional keypad
	/// </summary>
	private Dictionary<char, Dictionary<char, string>> _directionalDirections = null!;

	/// <summary>
	/// Using the <see cref="DirectionOfTravel"/> enum, place the directional
	/// keys in a precedence order. The most "expensive" direction to move in
	/// after the first level from the start point A is West / &lt;, followed
	/// by South / v. North / ^ and East / &gt; both have equal values. The
	/// order of precedence we have is therefore West, South, East, North
	/// </summary>
	private readonly Dictionary<DirectionOfTravel, int> _directionalPrecedence = Enum.GetValues<DirectionOfTravel>()
		.Where(d => d != DirectionOfTravel.Unknown)
		.OrderByDescending(o => o)
		.Select((d, i) => (d, i))
		.ToDictionary(kvp => kvp.d, kvp => kvp.i);

	#endregion

	#region Constructors
	#endregion

	#region Methods

	/// <summary>
	/// Defines the layouts for the <paramref name="numericKeypad"/> and <paramref name="directionalKeypad"/>
	/// </summary>
	/// <param name="numericKeypad">The layout for the numeric keypad</param>
	/// <param name="directionalKeypad">The layout for the directional keypad</param>
	/// <param name="invalidCharacters">A list/array of characters that represent invalid locations</param>
	public void SetupKeypads(IEnumerable<string> numericKeypad, IEnumerable<string> directionalKeypad, params char[] invalidCharacters)
	{
		//	Set up the keypad dictionary objects
		_numericKeypad = ParseKeypad(numericKeypad);
		_numericDirections = GetKeypadDirectionsEx(_numericKeypad, invalidCharacters);
		_directionalKeypad = ParseKeypad(directionalKeypad);
		_directionalDirections = GetKeypadDirectionsEx(_directionalKeypad, invalidCharacters);
	}

	/// <summary>
	/// Stores the lookups for a given code at the specified depth
	/// </summary>
	private readonly Dictionary<(string, int, int), string> _lookups = new Dictionary<(string, int, int), string>();

	private readonly Dictionary<(string, int, int), long> _longLookups = new Dictionary<(string, int, int), long>();

	private char[] _currentKeyPositions = null!;

	/// <summary>
	/// Computes the keypresses required and the overall complexity score for the
	/// <paramref name="code"/> supplied, using the <paramref name="depth"/>
	/// quantity of intermediate robots
	/// </summary>
	/// <param name="code">The code needed on the numeric keypad</param>
	/// <param name="depth">The number of intermediate robots</param>
	/// <returns>The keypresses needed and the overall complexity score</returns>
	/// <exception cref="Exception"></exception>
	public (string keypresses, long complexity) GetSolution(string code, int depth)
	{
		_lookups.Clear();
		_longLookups.Clear();
		//	All keypads start at the A position
		_currentKeyPositions = new String('A', depth + 1).ToCharArray();

		var presses = GetKeypresses(code, 0, depth);
		if (!int.TryParse(Regex.Replace(code, @"\D", ""), out var result))
			throw new Exception($"Cannot convert '{code}' to an integer");
		var complexity = (long)presses.Length * result;
		return (presses, complexity);
	}

	/// <summary>
	/// Recursive method to work out the keypresses needed to provide the specified
	/// <paramref name="code"/> at the <paramref name="depth"/> needed
	/// </summary>
	/// <param name="code">The code requiring entry onto the keypad</param>
	/// <param name="depth">The depth of intermediate robots</param>
	/// <param name="maxDepth">The maximum depth</param>
	/// <returns>The keypresses needed to execute <paramref name="code"/></returns>
	private string GetKeypresses(string code, int depth, int maxDepth)
	{
		//	Does the code exist already?
		if (_lookups.TryGetValue((code, depth, maxDepth), out var result))
			return result;

		var sb = new StringBuilder();

		var keypad = depth == 0 ? _numericDirections : _directionalDirections;

		try
		{
			foreach (var key in code.ToCharArray())
			{
				//	Get the position of the robot/human at the specified depth
				var current = _currentKeyPositions[depth];

				//	Protect ourselves from trying to get bizarre values
				if (!keypad.TryGetValue(current, out var toKey))
					throw new Exception($"keypad has no '{current}'");

				if (!toKey.TryGetValue(key, out var presses))
					throw new Exception("toKey not found");

				//	Store the new position the robot/human is at at the specified depth
				_currentKeyPositions[depth] = key;
				//	Append results until the maximum depth has been reached
				//	Note: "A" is always appended as this causes the robot to execute the selected keypress
				sb.Append(depth == maxDepth
					? presses + "A"
					: GetKeypresses(presses + "A", depth + 1, maxDepth));
			}
		}
		catch (System.Exception ex)
		{
			Console.WriteLine($"Huh?!? {ex.Message}: [{code}, {depth}, {maxDepth}]");
			throw;
		}
		return _lookups[(code, depth, maxDepth)] = sb.ToString();
	}

	/// <summary>
	/// Work out where all the characters are in relation to the keypad
	/// </summary>
	/// <param name="keypad">The keypad being parsed</param>
	/// <returns>A dictionary of the characters and their locations within a grid</returns>
	private Dictionary<char, Coordinate> ParseKeypad(IEnumerable<string> keypad)
	{
		ArgumentNullException.ThrowIfNull(keypad, nameof(keypad));

		var data = keypad.ToList();

		var charCoords = new Dictionary<char, Coordinate>();
		for (int y = 0; y < data.Count; y++)
		{
			for (var x = 0; x < data[y].Length; x++)
			{
				char c = data[y][x];
				charCoords[c] = new Coordinate(y, x);
			}
		}
		return charCoords;
	}

	/// <summary>
	/// Works out the optimal route from each key to other keys on the
	/// <paramref name="keypad"/>, where any danger locations / invalid
	/// locations are identified by values in <paramref name="blockedCellChars"/>
	/// </summary>
	/// <param name="keypad">The keypad layout being worked upon</param>
	/// <param name="blockedCellChars">An array of characters which represent invalid locations / danger areas</param>
	/// <returns>The optimal routes from one character to all others</returns>
	private Dictionary<char, Dictionary<char, string>> GetKeypadDirections(Dictionary<char, Coordinate> keypad, params char[] blockedCellChars)
	{
		//	Determine which values are valid or blocked
		var allowed = keypad.Where(q => !blockedCellChars.Contains(q.Key)).ToDictionary();
		var blocked = keypad.Where(q => blockedCellChars.Contains(q.Key)).ToDictionary();

		//	Determine the lists of allowed and blocked coordinates for the keys
		var allowedCoords = allowed.Select(s => s.Value).ToList();
		var blockedCoords = blocked.Select(s => s.Value).ToList();

		//	Perform a cartesian join on each allowed key with others
		var combo = allowed.SelectMany(m => allowed, (f, t) => new { From = f, To = t })
			.ToList();

		//	Create a lookup for moving from one key to another
		var directions = new Dictionary<char, Dictionary<char, string>>();

		//	Loop around each combination
		foreach (var key in combo)
		{
			//	Check for previous entry / create a new one
			if (!directions.TryGetValue(key.From.Key, out var toKeys))
			{
				toKeys = new Dictionary<char, string>();
				directions[key.From.Key] = toKeys;
			}

			//	Shortcut: if the key from/to is identical, no moves are needed
			if (key.From.Key == key.To.Key)
			{
				toKeys.TryAdd(key.To.Key, "");
				continue;
			}

			//	Would direct movement horizontally / vertically result in potential dangerous moves?
			var horizontalDanger = blockedCoords.Any(c => c.X == key.To.Value.X && c.Y == key.From.Value.Y);
			var verticalDanger = blockedCoords.Any(c => c.Y == key.To.Value.Y && c.X == key.From.Value.X);

			var sb = new StringBuilder();

			//	Find out the offset movement from one key to the other
			var dC = key.To.Value - key.From.Value;
			//	get the lists of moves needed
			var hMoves = Enumerable.Repeat(dC.X > 0 ? DirectionOfTravel.East : DirectionOfTravel.West, Math.Abs(dC.X)).ToList();
			var vMoves = Enumerable.Repeat(dC.Y > 0 ? DirectionOfTravel.South : DirectionOfTravel.North, Math.Abs(dC.Y)).ToList();

			//	Decide what movements are needed in horizontal / vertical directions
			//	Order is decided by potential dangers of going over the blocked cells
			//	Theory says this ought to select the best route, avoiding dangers
			var movements = new List<DirectionOfTravel>();
			if (horizontalDanger)
			{
				movements.AddRange(vMoves);
				movements.AddRange(hMoves);
			}
			else if (verticalDanger)
			{
				movements.AddRange(vMoves);
				movements.AddRange(hMoves);
			}
			else
			{
				movements.AddRange(hMoves);
				movements.AddRange(vMoves);
				movements = movements.OrderBy(d => _directionalPrecedence[d]).ToList();
			}

			//	Add movements into a queue to process them against permitted locations
			//	This checks that the movements chosen do actually work and won't end up in danger
			//	Practice says that occasionally the dangers aren't obvious immediately
			var movementQueue = new Queue<Coordinate>();
			movements.ForEach(m => movementQueue.Enqueue(m.ToOffset()));
			Coordinate currentCoord = new Coordinate(key.From.Value);

			while (movementQueue.Count > 0)
			{
				//	get the offset and apply to current coordinate
				Coordinate offset = movementQueue.Dequeue();
				Coordinate newCoord = currentCoord.OffsetBy(offset);

				//	Is the new coordinate allowed?
				if (!allowedCoords.Contains(newCoord))
				{
					//	no, re-queue the movement and try next
					movementQueue.Enqueue(offset);
					continue;
				}

				//	yes, get the movement
				var movement = offset.Y switch
				{
					-1 => '^',
					1 => 'v',
					_ => offset.X switch
					{
						-1 => '<',
						1 => '>',
						_ => ' '
					}
				};
				sb.Append(movement);
				currentCoord = newCoord;
			}
			toKeys.TryAdd(key.To.Key, sb.ToString());
		}
		return directions;
	}

	/// <summary>
	/// Computes the keypresses required and the overall complexity score for the
	/// <paramref name="code"/> supplied, using the <paramref name="depth"/>
	/// quantity of intermediate robots
	/// </summary>
	/// <param name="code">The code needed on the numeric keypad</param>
	/// <param name="depth">The number of intermediate robots</param>
	/// <returns>The keypresses needed and the overall complexity score</returns>
	/// <exception cref="Exception"></exception>
	public long GetComplexity(string code, int depth)
	{
		_lookups.Clear();
		_longLookups.Clear();
		//	All keypads start at the A position
		_currentKeyPositions = new String('A', depth + 1).ToCharArray();

		var presses = GetKeypressCount(code, 0, depth);
		if (!int.TryParse(Regex.Replace(code, @"\D", ""), out var result))
			throw new Exception($"Cannot convert '{code}' to an integer");
		return presses * result;
	}

	/// <summary>
	/// Recursive method to work out the keypresses needed to provide the specified
	/// <paramref name="code"/> at the <paramref name="depth"/> needed
	/// </summary>
	/// <param name="code">The code requiring entry onto the keypad</param>
	/// <param name="depth">The depth of intermediate robots</param>
	/// <param name="maxDepth">The maximum depth</param>
	/// <returns>The keypresses needed to execute <paramref name="code"/></returns>
	private long GetKeypressCount(string code, int depth, int maxDepth)
	{
		//	Does the code exist already?
		if (_longLookups.TryGetValue((code, depth, maxDepth), out var result))
			return result;

		var keypad = depth == 0 ? _numericDirections : _directionalDirections;

		try
		{
			result = 0L;
			foreach (var key in code.ToCharArray())
			{
				//	Get the position of the robot/human at the specified depth
				var current = _currentKeyPositions[depth];

				//	Protect ourselves from trying to get bizarre values
				if (!keypad.TryGetValue(current, out var toKey))
					throw new Exception($"keypad has no '{current}'");

				if (!toKey.TryGetValue(key, out var keysToPress))
					throw new Exception("toKey not found");

				//	Store the new position the robot/human is at at the specified depth
				_currentKeyPositions[depth] = key;
				//	Append results until the maximum depth has been reached
				//	Note: "A" is always appended as this causes the robot to execute the selected keypress
				result += depth == maxDepth
					? keysToPress.Length + 1
					: GetKeypressCount(keysToPress + "A", depth + 1, maxDepth);
			}
		}
		catch (System.Exception ex)
		{
			Console.WriteLine($"Huh?!? {ex.Message}: [{code}, {depth}, {maxDepth}]");
			throw;
		}
		return _longLookups[(code, depth, maxDepth)] = result;
	}

	#endregion

	#region Refactoring for part two

	private Dictionary<char, Dictionary<char, string>> GetKeypadDirectionsEx(Dictionary<char, Coordinate> keypad, params char[] blockedCellChars)
	{
		//	Determine which values are valid or blocked
		var allowed = keypad.Where(q => !blockedCellChars.Contains(q.Key)).ToDictionary();
		var blocked = keypad.Where(q => blockedCellChars.Contains(q.Key)).ToDictionary();

		//	Determine the lists of allowed and blocked coordinates for the keys
		var allowedCoords = allowed.Select(s => s.Value).ToList();
		var blockedCoords = blocked.Select(s => s.Value).ToList();

		//	Perform a cartesian join on each allowed key with others
		var combo = allowed.SelectMany(m => allowed, (f, t) => new { From = f, To = t })
			.ToList();

		//	Work out the upper bounds for a grid
		var maxX = Math.Max(allowedCoords.Max(c => c.X), blockedCoords.Max(c => c.X));
		var maxY = Math.Max(allowedCoords.Max(c => c.Y), blockedCoords.Max(c => c.Y));

		//	Create a lookup for moving from one key to another
		var directions = new Dictionary<char, Dictionary<char, string>>();

		//	Loop around each combination
		foreach (var key in combo)
		{
			//	Check for previous entry / create a new one
			if (!directions.TryGetValue(key.From.Key, out var toKeys))
			{
				toKeys = new Dictionary<char, string>();
				directions[key.From.Key] = toKeys;
			}

			var path = "";
			//	Only attempt walking the path if the key from/to is NOT identical
			if (key.From.Key != key.To.Key)
				path = GetOptimalPath(blockedCoords, 1 + maxX, 1 + maxY, key.From.Value, key.To.Value);

			//	Add the optimal path to the lookup
			toKeys.TryAdd(key.To.Key, path);
		}

		return directions;
	}

	private string GetOptimalPath(List<Coordinate> blocked, int x, int y, Coordinate start, Coordinate end)
	{
		//	Create a grid onto which the moves shall be projected
		var keypadGrid = new MazeGrid(x, y);
		//	Storage for best paths found for each direction of movement
		var bestPaths = new List<MazePath<DijkstraNode>>();

		//	Re-use the distance strategy where turning is expensive
		var movementStrategy = new ReindeerMazeDistanceStrategy();

		foreach (var direction in _directionalPrecedence)
		{
			keypadGrid.MakeGridEmpty();
			blocked.ForEach(c => keypadGrid[c] = MazeCellType.Wall);
			keypadGrid[start] = MazeCellType.Start;
			keypadGrid[end] = MazeCellType.End;

			//	Attempt to solve multiple best paths for the "maze"
			var dijkstraSolver = new DijkstraMazeSolver(keypadGrid);
			try
			{
				//	The best path is the one with the lowest "distance to exit" for the current start direction
				var bestPath = dijkstraSolver.SolveMultipleBestPaths(direction.Key, movementStrategy)
					//	Ignore any unsolveable
					.Where(q => q.DistanceToExit != int.MaxValue)
					.OrderBy(o => o.DistanceToExit)
					.FirstOrDefault();

				if (bestPath is not null)
					bestPaths.Add(bestPath);
			}
			catch (NullReferenceException)
			{
				Console.WriteLine($"Solver went bang! Params: [{x},{y}], {start}, {end}");
			}
		}

		//	Now get the "best of the best"
		var bestOfBest = bestPaths.OrderBy(o => o.DistanceToExit).FirstOrDefault();

		if (bestOfBest is null)
			throw new Exception($"No solution for grid");

		var node = bestOfBest.Path.First;
		var sb = new StringBuilder();
		while (node is not null)
		{
			sb.Append(node.Value.Direction.ToChar());
			node = node.Next;
		}
		var path = sb.ToString();
		//	Ignore the first node direction from the path - it isn't needed
		return string.IsNullOrWhiteSpace(path)
			? path
			: path[1..];
	}

	#endregion
}