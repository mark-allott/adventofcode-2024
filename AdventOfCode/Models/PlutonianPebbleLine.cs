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

/// <summary>
/// <para>
/// Refactored class to use lookups for pebble expansions
/// Each pebble can be considered as its own "line" and for a given number of
/// iterations, a pebble of the same value will always result in the same
/// result, no matter how many iterations are being calculated in total
/// </para>
/// <para>
/// e.g.<br/>
/// pebble: 	0 => 1 => 2024 => 20 24 => 2 0 2 4 => 4048 1 4048 8096<br/>
/// For this example, we know that after 4 iterations, the 0 expands to 2 0 2 4
/// therefore, if we encounter another instance of 0 with 4 iterations to
/// perform, we have already pre-computed the total number of pebbles it will
/// result in and the lookup will save us time having to compute it again
/// </para>
/// </summary>
internal class PlutonianPebbleLineEx
{
	/// <summary>
	/// Holds the initial set of pebbles
	/// </summary>
	private List<int> _pebbles = new List<int>();

	/// <summary>
	/// Holds lookups of pre-calculated counts of pebbles for a given pebbles value and iteration count
	/// </summary>
	private Dictionary<(long pebble, int count), long> _lookups = new Dictionary<(long, int), long>();

	#region ctor

	public PlutonianPebbleLineEx(string initialiser)
	{
		ArgumentNullException.ThrowIfNullOrWhiteSpace(initialiser, nameof(initialiser));

		foreach (var part in initialiser.ParseStringToListOfInt())
		{
			_pebbles.Add(part);
		}
	}

	#endregion

	/// <summary>
	/// Perform the blink operation for <paramref name="count"/> iterations and
	/// return the number of pebbles it produces in the line
	/// </summary>
	/// <param name="count">The number of iterations to execute for</param>
	/// <returns>The total number of pebbles in the line</returns>
	public long Blink(int count)
	{
		var result = 0L;

		_pebbles.ForEach(pebble => result += Blink(pebble, count));

		return result;
	}

	/// <summary>
	/// Performs the "blink" operation for a given <paramref name="pebble"/>, over <paramref name="count"/> iterations
	/// </summary>
	/// <param name="pebble">The pebble to "blink"</param>
	/// <param name="count">The number of "blinks" to perform</param>
	/// <returns>The total number of pebbles in the line if <paramref name="pebble"/> blinks <paramref name="count"/> times</returns>
	private long Blink(long pebble, int count)
	{
		//	No more steps left, so only 1 pebble
		if (count == 0)
			return 1L;

		//	Result already computed?
		if (_lookups.TryGetValue((pebble, count), out var result))
			return result;

		//	If pebble is zero, convert to a 1
		if (pebble == 0)
		{
			result = Blink(1, count - 1);
		}
		//	If an even number of digits, split into two pebbles
		else if ($"{pebble}".Length % 2 == 0)
		{
			var pebbleString = $"{pebble}";
			var splitLength = pebbleString.Length / 2;

			var p1 = int.Parse(pebbleString[..splitLength]);
			var p2 = int.Parse(pebbleString[splitLength..]);

			result = Blink(p1, count - 1) + Blink(p2, count - 1);
		}
		else
		{
			//	Multiply value by 2024 and compute
			result = Blink(pebble * 2024, count - 1);
		}

		//	Store computed result
		_lookups[(pebble, count)] = result;
		return result;
	}
}