using AdventOfCode.Extensions;

namespace AdventOfCode.Models;

internal class TopographicMap
{
	/// <summary>
	/// Storage for the map heights
	/// </summary>
	private int[,] _map;

	/// <summary>
	/// Indicates the maximum value for the row (aka Y-coord)
	/// </summary>
	private int _maxRow = 0;

	/// <summary>
	/// Indicates the maximum value for the column (aka X-coord)
	/// </summary>
	private int _maxCol = 0;

	/// <summary>
	/// Contains a list of starting positions, where the height is zero
	/// </summary>
	private List<Coordinate> _startPositions = new List<Coordinate>();

	/// <summary>
	/// Public accessor to the start positions as a read-only list
	/// </summary>
	public List<Coordinate> StartPositions => _startPositions.AsReadOnly().ToList();

	/// <summary>
	/// ctor
	/// </summary>
	/// <param name="input">The textual representation of maps topography</param>
	public TopographicMap(IEnumerable<string> input)
	{
		ArgumentNullException.ThrowIfNull(input, nameof(input));
		var inputAsList = input.ToList();
		ArgumentOutOfRangeException.ThrowIfZero(inputAsList.Count, nameof(input));
		_map = GetTopographicMap(inputAsList);
	}

	/// <summary>
	/// Convert the <paramref name="input"/> into an array of map heights
	/// </summary>
	/// <param name="input">The textual representation of map heights</param>
	/// <returns></returns>
	/// <exception cref="ArgumentException"></exception>
	private int[,] GetTopographicMap(List<string> input)
	{
		//	Validate input
		if (!input.Select(s => s.Length).All(l => l == input[0].Length))
			throw new ArgumentException("Cannot support ragged maps");

		//	Determine max values
		var rows = input.Count;
		var columns = input[0].Length;

		//	set bounds
		_maxRow = rows;
		_maxCol = columns;

		//	Create an array for the map heights
		var map = new int[rows, columns];

		//	Loop around each element and convert to an int
		for (var row = 0; row < input.Count; row++)
		{
			var line = input[row];
			for (var column = 0; column < columns; column++)
			{
				if (!int.TryParse(line.AsSpan(column, 1), out var result))
					throw new ArgumentException($"{nameof(GetTopographicMap)} failed when parsing value: {line.AsSpan(column, 1)}");
				map[row, column] = result;

				//	If the height is zero, add to the start positions
				if (result == 0)
					_startPositions.Add(new Coordinate(row, column));
			}
		}
		return map;
	}

	/// <summary>
	/// Creates a list of valid offset moves from the current position in terms of offsets to the current row/column
	/// </summary>
	private readonly List<(int rowOffset, int colOffset)> moveOffsets = new List<(int rowOffset, int colOffset)>()
	{
		(-1,0),	//	move "north" from current position
		(0,1),	//	move "east" from current position
		(1,0),	//	move "south" from current position
		(0,-1)	//	move "west" from current position
	};

	/// <summary>
	/// Determines the list of valid moves from <paramref name="location"/>, based on the movement rules
	/// </summary>
	/// <param name="location">The current location on the map</param>
	/// <returns>A list of the next possible locations</returns>
	public List<Coordinate> GetPossibleMovesFrom(Coordinate location)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(location.Y, nameof(location));
		ArgumentOutOfRangeException.ThrowIfGreaterThan(location.Y, _maxRow, nameof(location));
		ArgumentOutOfRangeException.ThrowIfNegative(location.X, nameof(location));
		ArgumentOutOfRangeException.ThrowIfGreaterThan(location.X, _maxCol, nameof(location));

		var moves = new List<Coordinate>();

		//	A valid move will result in a location in any direction where the
		//	height is one more than the current height
		var currentHeight = _map[location.Y, location.X];
		foreach (var (rowOffset, colOffset) in moveOffsets)
		{
			var newCoord = location.OffsetBy(rowOffset, colOffset);
			if (newCoord.InBounds(_maxRow, _maxCol) &&
				_map[newCoord.Y, newCoord.X] == currentHeight + 1)
				moves.Add(newCoord);
		}
		return moves;
	}

	public List<MapRoute> FindRoutes(Coordinate start)
	{
		var routes = new List<MapRoute>()
		{
			new MapRoute(start)
		};

		//	Continue to loop whilst at least one route can still move
		while (routes.Any(q => q.Walking))
		{
			//	Create a new list for routes that can hold any additional routes found
			var newRoutes = new List<MapRoute>();

			//	Loop around each of the current routes
			foreach (var route in routes)
			{
				//	If the route has already completed, no need to do any more work
				if (!route.Walking)
				{
					newRoutes.Add(route);
					continue;
				}

				//	Work from last coordinate on route
				var coord = route.Route.LastOrDefault();
				if (coord is null)
					throw new ArgumentNullException(nameof(coord));

				//	Determine whether it is possible to move further from this position
				var moves = GetPossibleMovesFrom(coord);

				//	No moves means the route has finished - either at top of trail or can't move higher
				if (moves.Count == 0)
				{
					route.Walking = false;
					newRoutes.Add(route);
					continue;
				}

				//	Loop for each possible move, creating a new route for each possible move
				foreach (var move in moves)
				{
					//	Duplicate the current route, add the new move and add to the new routes
					var newRoute = route.DeepCopy();
					newRoute.Add(move);
					newRoutes.Add(newRoute);
				}
			}
			//	Update the route list
			routes = newRoutes;
		}
		return routes;
	}

	/// <summary>
	/// Performs a walk along a <paramref name="route"/>, ensuring the route
	/// specified follows the expected rules and terminates at the top of the
	/// trail (height = 9)
	/// </summary>
	/// <param name="route">The route to walk</param>
	/// <returns>True if a correct route was detected, otherwise false</returns>
	public bool IsValidRouteToTop(MapRoute route)
	{
		ArgumentNullException.ThrowIfNull(route, nameof(route));

		//	Cannot be at the top if still walking
		if (route.Walking)
			return false;

		//	If any position is out of bounds this won't be valid
		if (route.Route.Any(q => !q.InBounds(_maxRow, _maxCol)))
			return false;

		//	Get a list of heights from the locations
		var heights = route.Route
			.Where(q => q.InBounds(_maxRow, _maxCol))
			.Select(c => _map[c.Y, c.X])
			.ToList();

		//	Start at zero and make sure the height is the expected
		var currentHeight = 0;
		for (var i = 0; i < heights.Count - 1; i++)
		{
			//	If height is not expected height, stop
			if (heights[i] != currentHeight)
				break;
			currentHeight++;
		}

		//	Return a positive result only if the height reached was 9
		return currentHeight == 9;
	}
}