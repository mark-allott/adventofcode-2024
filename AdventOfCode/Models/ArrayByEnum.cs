using System.Collections;

namespace AdventOfCode.Models;

internal class ArrayByEnum<T, K>
	: IEnumerable<T>
	where K : Enum
{
	#region Fields

	/// <summary>
	/// Stores a lookup between the enum value and the index within the array
	/// </summary>
	private readonly Dictionary<K, int> _lookup;

	/// <summary>
	/// Container for values being stored against their enum values
	/// </summary>
	private readonly T[] _values;

	#endregion

	#region Properties

	/// <summary>
	/// Indexer access to the underlying values
	/// </summary>
	/// <param name="key">The key to access the value</param>
	/// <returns>The stored value</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public T this[K key]
	{
		get
		{
			//	Attempt to retrieve from the lookup dictionary, if not index found, key is unsupported
			if (!_lookup.TryGetValue(key, out var index))
				throw new ArgumentOutOfRangeException(nameof(key), key, $"Index not supported");
			return _values[index];
		}
		set
		{
			//	Attempt to retrieve from the lookup dictionary, if not index found, key is unsupported
			if (!_lookup.TryGetValue(key, out var index))
				throw new ArgumentOutOfRangeException(nameof(key), key, $"Index not supported");
			_values[index] = value;
		}
	}
	#endregion

	#region Ctor

	/// <summary>
	/// Array constructor
	/// </summary>
	/// <param name="ignored">Any enum values that should be ignored</param>
	public ArrayByEnum(params K[] ignored)
	{
		//	Get all enum values
		var enumValues = Enum.GetValues(typeof(K)).Cast<K>().ToList();

		//	If there are any to be ignored, remove from the list
		if (ignored.Length > 0)
			enumValues = enumValues.Where(q => !ignored.Contains(q)).OrderBy(o => o).ToList();

		//	Create a list of KVP values with the key and an indexer value for the values array
		var lookups = Enumerable.Range(0, enumValues.Count).Select(s => new KeyValuePair<K, int>(enumValues[s], s)).ToList();
		_lookup = new Dictionary<K, int>(lookups);
		_values = new T[lookups.Count];
	}

	#endregion

	#region IEnumerable implementation

	public IEnumerator<T> GetEnumerator()
	{
		return _lookup.Keys.Select(k => this[k]).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	#endregion
}