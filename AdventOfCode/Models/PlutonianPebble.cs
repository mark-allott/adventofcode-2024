namespace AdventOfCode.Models;

internal class PlutonianPebble
{
	#region Fields
	#endregion

	#region Properties

	public long PebbleValue { get; }

	#endregion

	#region Constructor

	public PlutonianPebble(string intialiser)
	{
		if (!long.TryParse(intialiser, out long value))
			throw new ArgumentException($"The value '{intialiser}' is not valid", nameof(intialiser));
		ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(intialiser));

		PebbleValue = value;
	}

	public PlutonianPebble(long initialiser)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(initialiser, nameof(initialiser));
		PebbleValue = initialiser;
	}

	#endregion

	#region Overrides from base

	public override string ToString()
	{
		return $"{PebbleValue}";
	}

	#endregion

	#region Methods

	/// <summary>
	/// Performs the required operation for the "blink" taking place, returning the relevant <see cref="PlutonianPebble"/>
	/// </summary>
	/// <returns>The new pebble value(s)</returns>
	public List<PlutonianPebble> Blink()
	{
		//	If zero, return pebble of 1
		if (PebbleValue == 0)
			return new List<PlutonianPebble>() { new(1) };

		//	If an even number of digits, split into two pebbles
		var pebbleLength = this.ToString().Length;
		if (pebbleLength % 2 == 0)
		{
			var splitLength = pebbleLength / 2;
			var p1 = this.ToString()[..splitLength];
			var p2 = this.ToString()[splitLength..];

			return new List<PlutonianPebble>()
			{
				new(p1),
				new(p2)
			};
		}

		return new List<PlutonianPebble>() { new(PebbleValue * 2024) };
	}

	#endregion
}