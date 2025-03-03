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
	/// Determines whether a specific <see cref="MapCoord"/> is located within this region
	/// </summary>
	/// <param name="coord">The coordinate of the location</param>
	/// <returns>True if the location is for a plot within this region, otherwise false</returns>
	public bool InBounds(MapCoord coord)
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

	#endregion
}