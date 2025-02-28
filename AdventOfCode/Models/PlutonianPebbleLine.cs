using AdventOfCode.Extensions;

namespace AdventOfCode.Models;

internal class PlutonianPebbleLine
{
	#region Fields

	/// <summary>
	/// Container for the pebbles
	/// </summary>
	private List<PlutonianPebble> _pebbles = new List<PlutonianPebble>();

	#endregion

	#region Properties

	/// <summary>
	/// Provide read-only access to the pebbles
	/// </summary>
	public List<PlutonianPebble> Pebbles => _pebbles.AsReadOnly().ToList();

	/// <summary>
	/// Returns the number of pebbles in the container
	/// </summary>
	public long PebbleCount => _pebbles.Count;

	#endregion

	#region Constructor

	public PlutonianPebbleLine(string initialiser)
	{
		ArgumentNullException.ThrowIfNullOrWhiteSpace(initialiser, nameof(initialiser));

		foreach (var part in initialiser.ParseStringToListOfInt())
		{
			_pebbles.Add(new PlutonianPebble(part));
		}
	}

	#endregion

	#region Overrides from base

	public override string ToString()
	{
		return string.Join(" ", _pebbles);
	}

	#endregion

	#region Methods

	/// <summary>
	/// Performs the blink operation across all pebbles just once
	/// </summary>
	public void Blink()
	{
		var _newPebbles = new List<PlutonianPebble>();
		foreach (var pebble in _pebbles)
		{
			_newPebbles.AddRange(pebble.Blink());
		}
		_pebbles = _newPebbles;
	}

	/// <summary>
	/// Performs the blink operation across all pebbles for <paramref name="count"/> iterations
	/// </summary>
	/// <param name="count">The number of blink operations to perform</param>
	public void Blink(int count)
	{
		for (var i = 0; i < count; i++)
			Blink();
	}

	#endregion
}