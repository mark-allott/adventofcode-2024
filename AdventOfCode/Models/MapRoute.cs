namespace AdventOfCode.Models;

/// <summary>
/// Model class to represent a walking route
/// </summary>
internal class MapRoute
	: ICloneable
{
	#region Fields

	/// <summary>
	/// The list of locations visited on the route
	/// </summary>
	private readonly List<MapCoord> _route = new List<MapCoord>();

	#endregion

	#region Properties

	/// <summary>
	/// Holds the starting location on the route
	/// </summary>
	public MapCoord StartPosition { get; } = null!;

	/// <summary>
	/// Provides a readonly list of the coordinates visited
	/// </summary>
	public List<MapCoord> Route => _route.AsReadOnly().ToList();

	/// <summary>
	/// Returns the last position visited on the route
	/// </summary>
	public MapCoord LastPosition => _route.LastOrDefault()!;

	/// <summary>
	/// Indicates whether the route is in-progress or has completed
	/// </summary>
	public bool Walking { get; set; } = true;

	#endregion

	#region Constructor

	public MapRoute(MapCoord startPosition)
	{
		ArgumentNullException.ThrowIfNull(startPosition, nameof(startPosition));

		StartPosition = startPosition.DeepCopy();
		_route.Add(StartPosition);
	}

	/// <summary>
	/// Internal constructor for cloning
	/// </summary>
	private MapRoute()
	{
	}

	#endregion

	#region ICloneable implemantation

	/// <summary>
	/// Creates a new instance of a route based on this route
	/// </summary>
	/// <returns></returns>
	public MapRoute DeepCopy()
	{
		var clone = new MapRoute();
		foreach (var coord in _route)
			clone.Add(coord.DeepCopy());
		clone.Walking = Walking;
		return clone;
	}

	object ICloneable.Clone()
	{
		return DeepCopy();
	}

	#endregion

	#region Overrides from base

	/// <summary>
	/// Debug friendly display of the route being taken
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		return string.Join("=>", _route.Select(r => r.ToString()));
	}

	#endregion

	#region Public methods

	/// <summary>
	/// Adds a position to the route being walked (but only if we are walking)
	/// </summary>
	/// <param name="coord">The coordinate being added</param>
	public void Add(MapCoord coord)
	{
		if (Walking)
			_route.Add(coord);
	}

	#endregion
}