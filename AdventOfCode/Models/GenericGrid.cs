using System.Text;
using AdventOfCode.Interfaces;

namespace AdventOfCode.Models;

internal class GenericGrid<T>
{
	#region Fields

	/// <summary>
	/// Holds the contents of the grid
	/// </summary>
	private readonly T[,] _grid = null!;

	/// <summary>
	/// Holds the upper bounds of the grid
	/// </summary>
	private Coordinate _bounds = null!;

	/// <summary>
	/// Allows cells to be rendered in a custom manner, rather than using standard ToString calls
	/// </summary>
	private readonly ICellRenderer<T> _cellRenderer = null!;

	#endregion

	#region Property Accessors

	/// <summary>
	/// Indexer accessor, using separate X and Y coords
	/// </summary>
	/// <param name="x">The x-Coord of the cell</param>
	/// <param name="y">The y-Coord of the cell</param>
	/// <returns>The type occupying the cell</returns>
	public T this[int x, int y]
	{
		get
		{
			ValidateGridCoords(x, y);
			return _grid[y, x];
		}
		set
		{
			ValidateGridCoords(x, y);
			_grid[y, x] = value;
		}
	}

	/// <summary>
	/// Alternate Indexer accessor, using a <see cref="IGraphCoordinate"/> to get/set the cell
	/// </summary>
	/// <param name="coord">The location to access</param>
	/// <returns>The type occupying the cell</returns>
	public T this[IGraphCoordinate coord]
	{
		get => this[coord.X, coord.Y];
		set => this[coord.X, coord.Y] = value;
	}

	/// <summary>
	/// Yields the upper bounds of the grid
	/// </summary>
	public Coordinate Bounds => _bounds?.DeepCopy() ?? null!;

	#endregion

	#region ctor

	/// <summary>
	/// Default constructor, sets the bounds of the grid
	/// </summary>
	/// <param name="x">The number of x-coords/columns in the grid</param>
	/// <param name="y">The number of y-coords/rows in the grid</param>
	public GenericGrid(int x, int y, ICellRenderer<T> cellRenderer = null!)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(x, nameof(x));
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(y, nameof(y));
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(x, 1000, nameof(x));
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(y, 1000, nameof(y));
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(x * y, 100000, $"{nameof(x)} * {nameof(y)} exceeds storage");
		_bounds = new Coordinate(y, x);
		_grid = new T[y, x];
		_cellRenderer = cellRenderer;
	}

	/// <summary>
	/// Alternate ctor using a <see cref="Coordinate"/> as the bounds of the grid
	/// </summary>
	/// <param name="coord">The <see cref="Coordinate"/> representing the bounds of the grid</param>
	public GenericGrid(Coordinate coord, ICellRenderer<T> cellRenderer = null!)
		: this(coord.Y, coord.X, cellRenderer)
	{ }

	#endregion

	#region Methods

	/// <summary>
	/// Common validation of <paramref name="x"/> and <paramref name="y"/>
	/// values as well as making sure the maze if initialised etc.
	/// </summary>
	/// <param name="x">The x-Coord in the grid</param>
	/// <param name="y">The y-Coord in the grid</param>
	/// <exception cref="ArgumentNullException"></exception>
	private void ValidateGridCoords(int x, int y)
	{
		if (_grid is null)
			throw new ArgumentNullException(nameof(_grid), "Grid is uninitialised");
		if (_bounds is null)
			throw new ArgumentNullException(nameof(_bounds), "Grid is not initialised correctly");
		ArgumentOutOfRangeException.ThrowIfNegative(x, nameof(x));
		ArgumentOutOfRangeException.ThrowIfNegative(y, nameof(y));
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(x, _bounds.X, nameof(x));
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(y, _bounds.Y, nameof(y));
	}

	#endregion

	#region Overrides from base

	/// <summary>
	/// Returns a text representation of the grid
	/// </summary>
	/// <returns>The grid expressed as a string (each row of the grid occupies new line in string)</returns>
	public override string ToString()
	{
		var sb = new StringBuilder();

		for (var y = 0; y < _bounds.Y; y++)
		{
			for (var x = 0; x < _bounds.X; x++)
			{
				var cell = this[x, y];
				sb.Append(cell is null ? '?' : _cellRenderer is null ? cell.ToString() : _cellRenderer.ToCharacter(cell));
			}
			sb.AppendLine();
		}
		return sb.ToString();
	}

	#endregion
}