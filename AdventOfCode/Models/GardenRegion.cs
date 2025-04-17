using AdventOfCode.Enums;

namespace AdventOfCode.Models;

internal class GardenRegion
{
	#region Fields

	/// <summary>
	/// Container to holds plots belonging in this region
	/// </summary>
	private List<GardenPlot> _plots = new List<GardenPlot>();

	/// <summary>
	/// Helper property to identify the region
	/// </summary>
	public int ID { get; }

	/// <summary>
	/// Property to return the area of the region (number of plots it contains)
	/// </summary>
	public int Area => _plots.Count;

	/// <summary>
	/// Property to calculate the size of the perimeter of the region
	/// </summary>
	public int Perimeter => GetPerimeter();

	/// <summary>
	/// Property to calculate the cost of fencing the region
	/// </summary>
	public int Cost => Area * Perimeter;

	/// <summary>
	/// Property to return the number of sides within the region
	/// </summary>
	public int Sides => GetSides();

	/// <summary>
	/// New property to calculate the bulk cost for the region
	/// </summary>
	public int BulkCost => Area * Sides;

	#endregion

	#region ctor

	/// <summary>
	/// ctor
	/// </summary>
	/// <param name="id">Sets an ID for the region</param>
	public GardenRegion(int id)
	{
		ID = id;
	}

	#endregion

	#region Methods

	/// <summary>
	/// Adds a plot to the region
	/// </summary>
	/// <param name="plot">The plot to add to this region</param>
	public void AddPlot(GardenPlot plot)
	{
		//	Don't add the same plot twice
		if (_plots.IndexOf(plot) != -1)
			return;
		_plots.Add(plot);
		// Console.WriteLine($" added to Region ID: {ID}");
		plot.SetRegion(this);
	}

	/// <summary>
	/// Calculates the length of the perimeter for the region
	/// </summary>
	/// <returns>The total length of all fence parts</returns>
	private int GetPerimeter()
	{
		var topFences = _plots.Count(p => (p.FencePositions & FencePosition.Top) > 0);
		var bottomFences = _plots.Count(p => (p.FencePositions & FencePosition.Bottom) > 0);
		var leftFences = _plots.Count(p => (p.FencePositions & FencePosition.Left) > 0);
		var rightFences = _plots.Count(p => (p.FencePositions & FencePosition.Right) > 0);

		return topFences + bottomFences + leftFences + rightFences;
	}

	/// <summary>
	/// Determines whether a specific <see cref="Coordinate"/> is located within this region
	/// </summary>
	/// <param name="coord">The coordinate of the location</param>
	/// <returns>True if the location is for a plot within this region, otherwise false</returns>
	public bool InBounds(Coordinate coord)
	{
		return _plots.Any(p => p.Location == coord);
	}

	/// <summary>
	/// Determines whether the <paramref name="plot"/> is within this region
	/// </summary>
	/// <param name="plot">The garden plot to look for</param>
	/// <returns>True if found, otherwise false</returns>
	public bool Contains(GardenPlot? plot)
	{
		return plot is not null && _plots.Contains(plot);
	}

	/// <summary>
	/// Debug helper method: displays the are and perimeter of the region
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		return $"{nameof(Area)}: {Area}, {nameof(Perimeter)}: {Perimeter}";
	}

	/// <summary>
	/// Helper class to calculate sides for a given grouping
	/// </summary>
	private class FenceLocation
	{
		public int Key { get; }
		public FencePosition Position { get; }
		public List<int> Locations { get; }

		public FenceLocation(int key, FencePosition position, IEnumerable<Coordinate> locations)
		{
			var horizontalPositions = FencePosition.Top | FencePosition.Bottom;
			Key = key;
			Position = position;
			Locations = (position & horizontalPositions) > 0
				? locations.Select(s => s.X).OrderBy(o => o).ToList()
				: locations.Select(s => s.Y).OrderBy(o => o).ToList();
		}

		public int Sides => GetSides();

		private int GetSides()
		{
			//	If zero or one location, return the count, nothing else to work out
			if (Locations.Count < 2)
				return Locations.Count;

			//	initialise counters
			var sides = 0;
			var lastLocation = -2;
			foreach (var location in Locations)
			{
				//	If a side it continuous, then the locations increment by
				//	one each time. If a gap appears, then we have a new side present
				if (location > lastLocation + 1)
					sides++;
				lastLocation = location;
			}
			return sides;
		}
	}

	/// <summary>
	/// Helper method to identify the number of sides of fences within the region
	/// </summary>
	/// <returns>The number of sides found</returns>
	/// <remarks>
	/// To locate the sides within the region, first split all of the existing
	/// fences into their component parts, ensure all fence locations are grouped
	/// by common location elements: top/bottom by row and left/right by column,
	/// then in ascending corresponding orders to allow detection of gaps in the
	/// sequences more easily.
	/// </remarks>
	private int GetSides()
	{
		//	No plots, no sides
		if (_plots.Count == 0)
			return 0;

		//	Get the plots where there are "top" fences
		var topFences = _plots
			.Where(p => (p.FencePositions & FencePosition.Top) > 0)
			.GroupBy(p => p.Location.Y)
			.Select(g => new FenceLocation(g.Key, FencePosition.Top, g.Select(s => s.Location)))
			.Sum(s => s.Sides);

		var bottomFences = _plots
			.Where(p => (p.FencePositions & FencePosition.Bottom) > 0)
			.GroupBy(p => p.Location.Y)
			.Select(g => new FenceLocation(g.Key, FencePosition.Bottom, g.Select(s => s.Location)))
			.Sum(s => s.Sides);

		var leftFences = _plots
			.Where(p => (p.FencePositions & FencePosition.Left) > 0)
			.GroupBy(p => p.Location.X)
			.Select(g => new FenceLocation(g.Key, FencePosition.Left, g.Select(s => s.Location)))
			.Sum(s => s.Sides);

		var rightFences = _plots
			.Where(p => (p.FencePositions & FencePosition.Right) > 0)
			.GroupBy(p => p.Location.X)
			.Select(g => new FenceLocation(g.Key, FencePosition.Right, g.Select(s => s.Location)))
			.Sum(s => s.Sides);

		return topFences + bottomFences + leftFences + rightFences;
	}

	#endregion
}