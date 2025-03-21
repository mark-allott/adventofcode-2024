using AdventOfCode.Enums;

namespace AdventOfCode.Models;

/// <summary>
/// Represents the overall "garden" containing plots and regions of planting
/// </summary>
internal class Garden
{
	/// <summary>
	/// Container for all plots contained in the garden
	/// </summary>
	private readonly GardenPlot[,] _plots = null!;

	/// <summary>
	/// Container for the planting regions this garden encompasses
	/// </summary>
	private readonly List<GardenRegion> _regions = new List<GardenRegion>();

	/// <summary>
	/// The total number of rows of plants (aka max Y-coord)
	/// </summary>
	public int RowCount { get; }

	/// <summary>
	/// The total number of columns of plants (aka max X-coord)
	/// </summary>
	public int ColumnCount { get; }

	/// <summary>
	/// Publically accessible readonly list of regions within the garden
	/// </summary>
	public List<GardenRegion> Regions => _regions.AsReadOnly().ToList();

	#region ctor

	/// <summary>
	/// ctor
	/// </summary>
	/// <param name="input">The list of strings that make up the garden</param>
	/// <exception cref="ArgumentException"></exception>
	public Garden(IEnumerable<string> input)
	{
		ArgumentNullException.ThrowIfNull(nameof(input));

		var rows = input.ToList();
		RowCount = rows.Count;
		ColumnCount = rows[0].Length;

		if (!rows.All(r => r.Length == ColumnCount))
			throw new ArgumentException("Cannot support ragged areas", nameof(input));

		_plots = new GardenPlot[RowCount, ColumnCount];
		LoadPlots(rows);
	}

	#endregion

	#region Methods

	/// <summary>
	/// Performs the loading of the textual representation of the garden and
	/// conversion into individual <see cref="GardenPlot"/> objects
	/// </summary>
	/// <param name="plots">The textual representation of the garden</param>
	private void LoadPlots(List<string> plots)
	{
		for (var row = 0; row < RowCount; row++)
		{
			var plants = plots[row].ToCharArray();
			for (var column = 0; column < ColumnCount; column++)
			{
				var coord = new Coordinate(row, column);
				_plots[row, column] = new GardenPlot(coord, plants[column]);
			}
		}
	}

	/// <summary>
	/// Creates a list of valid offset moves from the current position in terms of offsets to the current row/column
	/// </summary>
	private readonly List<(int rowOffset, int colOffset, FencePosition fence)> plotOffsets = new List<(int, int, FencePosition)>()
	{
		(-1,0, FencePosition.Top),		//	move "up" from current position
		(0,-1, FencePosition.Left),		//	move "left" from current position
		(0,1, FencePosition.Right),		//	move "right" from current position
		(1,0, FencePosition.Bottom),	//	move "down" from current position
	};

	/// <summary>
	/// Works through the plots in the garden and adds fencing around them
	/// </summary>
	public void SetFences()
	{
		for (var row = 0; row < RowCount; row++)
		{
			for (var column = 0; column < ColumnCount; column++)
			{
				List<(int r, int c, FencePosition f)> adjacent = plotOffsets
					.Select(o => (row + o.rowOffset, column + o.colOffset, o.fence))
					.ToList();
				var currentPlot = _plots[row, column];
				var currentPlant = currentPlot.Plant;
				var fences = FencePosition.None;

				foreach (var (r, c, f) in adjacent)
				{
					//	Get the coord to be checked relative to the current plot (up/down/left/right)
					var coord = new Coordinate(r, c);
					//	If the coord is out of bounds, or there is a different plant, add the appropriate fence
					if (!coord.InBounds(RowCount, ColumnCount) || _plots[r, c].Plant != currentPlant)
						fences |= f;
				}
				currentPlot.SetFences(fences);
			}
		}
	}

	/// <summary>
	/// Works through the plots in the garden and adds them into planting regions
	/// </summary>
	public void SetRegions()
	{
		//	Walk across all plots adding to regions
		for (var row = 0; row < RowCount; row++)
		{
			for (var column = 0; column < ColumnCount; column++)
			{
				//	Get plot at [row,column]
				var currentPlot = _plots[row, column];

				SetRegion(currentPlot);
			}
		}
	}

	/// <summary>
	/// Checks a plot and moves it into a planting region, if not already set
	/// </summary>
	/// <param name="plot">The plot to assign a region to</param>
	private void SetRegion(GardenPlot plot)
	{
		// Console.Write($"Checking Plot {plot}");
		//	If there's a region assigned to this plot already, move to next
		if (plot.Region is not null)
		{
			// Console.WriteLine(" already handled");
			return;
		}

		//	Get the current location
		var row = plot.Location.Y;
		var column = plot.Location.X;

		//	Find adjacent plots within bounds and with same plant
		var adjacentPlots = plotOffsets
			.Select(o => new Coordinate(row + o.rowOffset, column + o.colOffset))
			.Where(c => c.InBounds(RowCount, ColumnCount))
			.Select(c => _plots[c.Y, c.X])
			.Where(p => p.Plant == plot.Plant)
			.ToList();

		var neighbourPlot = adjacentPlots
			.FirstOrDefault(p => p.Region is not null);

		//	If our neighbour with the same plant has a region, use that
		if (neighbourPlot is not null)
		{
			neighbourPlot.Region.AddPlot(plot);
		}

		//	If we have no region, create a new one
		if (plot.Region is null)
		{
			var counter = _regions.Count + 1;
			var region = new GardenRegion(counter);
			_regions.Add(region);
			region.AddPlot(plot);
		}

		adjacentPlots
			.Where(p => p.Region is null)
			.ToList()
			.ForEach(p => SetRegion(p));
	}

	#endregion
}