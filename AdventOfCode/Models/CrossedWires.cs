namespace AdventOfCode.Models;

internal class CrossedWires
{
	#region Fields

	/// <summary>
	/// Holds all values for the "gates"
	/// </summary>
	private Dictionary<string, int> _gates = new Dictionary<string, int>();

	/// <summary>
	/// Holds all operations to be executed
	/// </summary>
	private List<string> _operations = new List<string>();

	#endregion

	#region Properties

	/// <summary>
	/// Returns a dictionary of the values for the "z" gates
	/// </summary>
	public Dictionary<string, int> ZGates => _gates.Keys
		.Where(q => q.StartsWith('z'))
		.OrderByDescending(o => o)
		.ToDictionary(d => d, d => _gates[d]);

	/// <summary>
	/// Returns the operations to be performed as a Queue
	/// </summary>
	public Queue<(string gate1, string gate2, string op, string outGate)> OperationQueue
	{
		get
		{
			var q = new Queue<(string, string, string, string)>();
			_operations.ForEach(o => q.Enqueue(GetOperationParts(o)));
			return q;
		}
	}

	#endregion

	#region Ctor
	#endregion

	#region Methods

	/// <summary>
	/// Initilises the model with the gate values and operations taken from <paramref name="input"/>
	/// </summary>
	/// <param name="input">The input data</param>
	/// <exception cref="ArgumentException"></exception>
	public void Initialise(IEnumerable<string> input)
	{
		ArgumentNullException.ThrowIfNull(input);

		var gates = input.Where(q => q.Contains(':')).ToList();

		if (gates.Count == 0)
			throw new ArgumentException("Input contains no gate information", nameof(input));

		_operations = input.Where(q => !q.Contains(':'))
			.Where(q => !string.IsNullOrWhiteSpace(q))
			.ToList();

		if (_operations.Count == 0)
			throw new ArgumentException("Input contains no operations", nameof(input));

		_gates = gates.Select(s => s.Split(':', StringSplitOptions.TrimEntries).ToArray())
			.Where(q => q.Length == 2)
			.Select(s => new { k = s[0], v = s[1] })
			.ToDictionary(kvp => kvp.k, kvp => int.Parse(kvp.v));
	}

	/// <summary>
	/// Performs the required operations to obtain the final result
	/// </summary>
	/// <returns>The result of all operations</returns>
	/// <exception cref="InvalidOperationException"></exception>
	public ulong SolvePartOne()
	{
		var queue = OperationQueue;

		//	Loop whilst operations are still to be executed
		while (queue.Count > 0)
		{
			var operation = queue.Dequeue();
			var (gate1, gate2, op, outGate) = (operation.gate1, operation.gate2, operation.op, operation.outGate);
			//	Are there values for both gates?
			if (!(_gates.TryGetValue(gate1, out var v1) && _gates.TryGetValue(gate2, out var v2)))
			{
				queue.Enqueue(operation);
				continue;
			}

			var v = op switch
			{
				"AND" => v1 & v2,
				"OR" => v1 | v2,
				"XOR" => v1 ^ v2,
				_ => throw new InvalidOperationException($"Unknown operation: '{op}")
			};
			_gates[outGate] = v;
		}

		ulong result = 0;
		ZGates.Values.ToList().ForEach(i => { result <<= 1; result |= (uint)i; });
		return result;
	}

	/// <summary>
	/// Splits the operation into separate elements for easier processing
	/// </summary>
	/// <param name="operation"></param>
	/// <returns></returns>
	private static (string gate1, string gate2, string op, string outGate) GetOperationParts(string operation)
	{
		var parts = operation.Split(' ');
		return (parts[0], parts[2], parts[1], parts[4]);
	}

	#endregion
}