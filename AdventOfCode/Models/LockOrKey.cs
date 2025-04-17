namespace AdventOfCode.Models;

internal class LockOrKey
{
	#region Fields
	#endregion

	#region Properties

	public bool IsLock { get; }

	public int[] Heights { get; }

	public int MaxHeight { get; }

	#endregion

	#region Ctor

	/// <summary>
	/// Constructor - takes the schematics from <paramref name="input"/> and assembles the model
	/// </summary>
	/// <param name="input">The schematics for the lock or key</param>
	/// <exception cref="ArgumentException"></exception>
	public LockOrKey(IEnumerable<string> input)
	{
		//	Must have something!!
		ArgumentNullException.ThrowIfNull(nameof(input));
		var data = input.ToList();
		//	Grab the length of the top row
		var targetLength = data[0].Length;
		//	Every line must be equal length
		if (!data.All(d => d.Length == targetLength))
			throw new ArgumentException("Jagged locks/keys are not permitted", nameof(input));

		//	Locks are identified by the top row being all '#' characters
		IsLock = data[0].ToCharArray().All(c => c.Equals('#'));

		Heights = new int[targetLength];

		//	Pull height data from the input, discarding top and bottom rows
		var heightData = data[1..^1];
		MaxHeight = heightData.Count;
		SetHeights(heightData);
	}

	#endregion

	#region Overrides

	/// <summary>
	/// Debug helper - shows the type of object and heights
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		return $"{(IsLock ? "Lock" : "Key ")} [{string.Join(",", Heights)}]";
	}

	#endregion

	#region Methods

	/// <summary>
	/// Sets the heights of each column of the lock/key
	/// </summary>
	/// <param name="input">The schematic data</param>
	private void SetHeights(List<string> input)
	{
		foreach (var row in input)
			for (var i = 0; i < row.Length; i++)
				Heights[i] += row[i] == '#' ? 1 : 0;
	}

	/// <summary>
	/// Performs a simple check on the lock/key to see if a key will fit in a lock
	/// </summary>
	/// <param name="other">The other part of the combination</param>
	/// <returns>True if a key will fit into a lock, otherwise false</returns>
	public bool IsHeightFit(LockOrKey other)
	{
		return other is not null &&
			IsLock != other.IsLock &&
			MaxHeight == other.MaxHeight &&
			Heights.Length == other.Heights.Length &&
			Enumerable.Range(0, Heights.Length).All(i => Heights[i] + other.Heights[i] <= MaxHeight);
	}

	#endregion
}