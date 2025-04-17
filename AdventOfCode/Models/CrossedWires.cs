namespace AdventOfCode.Models;

internal class CrossedWires
{
	#region Fields

	/// <summary>
	/// Holds all values for the "sum" registers
	/// </summary>
	private Dictionary<string, int> _sumRegisters = new Dictionary<string, int>();

	/// <summary>
	/// Holds all operations to be executed
	/// </summary>
	private List<string> _operations = new List<string>();

	#endregion

	#region Properties

	/// <summary>
	/// Returns a dictionary of the values for the "z" gates
	/// </summary>
	public Dictionary<string, int> ZGates => this['z'];

	/// <summary>
	/// Returns the operations to be performed as a Queue
	/// </summary>
	public Queue<(string gate1, string op, string gate2, string outGate)> OperationQueue
	{
		get
		{
			var q = new Queue<(string, string, string, string)>();
			_operations.ForEach(o => q.Enqueue(GetOperationParts(o)));
			return q;
		}
	}

	/// <summary>
	/// Returns a dictionary of gates that begin with <paramref name="c"/>
	/// </summary>
	/// <param name="c">The character the gates begin with</param>
	/// <returns>A dictionary of the gates</returns>
	public Dictionary<string, int> this[char c]
	{
		get
		{
			return _sumRegisters.Keys
						.Where(q => q.StartsWith(c))
						.OrderByDescending(o => o)
						.ToDictionary(d => d, d => _sumRegisters[d]);
		}
	}

	/// <summary>
	/// Returns a dictionary of the values for the "x" gates
	/// </summary>
	public Dictionary<string, int> XGates => this['x'];

	/// <summary>
	/// Returns a dictionary of the values for the "y" gates
	/// </summary>
	public Dictionary<string, int> YGates => this['y'];

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

		_sumRegisters = gates.Select(s => s.Split(':', StringSplitOptions.TrimEntries).ToArray())
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
		return DoWork(OperationQueue);
	}

	/// <summary>
	/// Refactor to extract working method from the solution for part one
	/// </summary>
	/// <param name="queue">The work to be performed</param>
	/// <returns>The output from the calculation</returns>
	/// <exception cref="InvalidOperationException"></exception>
	private ulong DoWork(Queue<(string gate1, string op, string gate2, string outGate)> queue)
	{
		//	Loop whilst operations are still to be executed
		while (queue.Count > 0)
		{
			var operation = queue.Dequeue();
			var (gate1, op, gate2, outGate) = (operation.gate1, operation.op, operation.gate2, operation.outGate);
			//	Are there values for both gates?
			if (!(_sumRegisters.TryGetValue(gate1, out var v1) && _sumRegisters.TryGetValue(gate2, out var v2)))
			{
				queue.Enqueue(operation);
				continue;
			}

			var v = op switch
			{
				"AND" => v1 & v2,
				"OR" => v1 | v2,
				"XOR" => v1 ^ v2,
				_ => throw new InvalidOperationException($"Unknown operation: '{op}'")
			};
			_sumRegisters[outGate] = v;
		}

		return GetResult(ZGates);
	}

	/// <summary>
	/// Splits the operation into separate elements for easier processing
	/// </summary>
	/// <param name="operation"></param>
	/// <returns></returns>
	private static (string gate1, string op, string gate2, string outGate) GetOperationParts(string operation)
	{
		var parts = operation.Split(' ');
		return (parts[0], parts[1], parts[2], parts[4]);
	}

	/// <summary>
	/// Extracts the individual bitwise results from <paramref name="input"/> and returns the unsigned long value
	/// </summary>
	/// <param name="input">The bitwise values</param>
	/// <returns>The ulong value for <paramref name="input"/></returns>
	private static ulong GetResult(Dictionary<string, int> input)
	{
		ulong result = 0;
		input.Values.ToList().ForEach(i => { result <<= 1; result |= (uint)i; });
		return result;
	}

	/// <summary>
	/// Perform work to solve part two
	/// </summary>
	/// <returns>The swapped elements in alphabetical order</returns>
	public string SolvePartTwo()
	{
		//	As the solution for part two does no calculations, time to do some
		//	refactoring of the inputs as this is where the problem lies!

		//	Taking the operations, flatten then into a tuple structure first
		var flattenedOps = _operations
			.Select(o => o.Split(' '))
			//	Mangle here to individual elements, except for the inputs, where they get ordered alphabetically
			.Select(s => (sum: s[^1], ins: new[] { s[0], s[2] }.Order().ToArray(), op: s[1], ix: int.TryParse(s[^1][1..], out var i) ? i : -1))
			//	Re-mangle to have inA as the first alphabetical input and inB as the last (for inputs of x and y, they always appear in that order)
			.Select(s => (s.sum, inA: s.ins[0], inB: s.ins[1], s.op, s.ix))
			.ToList();
		//	using the flattened operations, reassemble and add the child operations for the inA and inB
		var fwc = flattenedOps
			.Select(s => new
			{
				s.sum,
				s.inA,
				s.inB,
				s.op,
				s.ix,
				children = flattenedOps.Where(q => new[] { s.inA, s.inB }
					.Contains(q.sum))
					.OrderByDescending(o => o.op)	//	This gets operations ordered XOR, OR, then AND
					.ThenByDescending(o => o.inA)
					.ToList(),
				//	Helper: any "final" operation stores the result in a Z register
				IsFinal = s.sum.StartsWith('z'),
				//	Helper: any "direct" operation uses only the Xn and Yn inputs
				IsDirect = s.inA.StartsWith('x') && s.inB.StartsWith('y'),
			})
			.ToList();

		//	Scrubbed register lines will be different from the following algorithm,
		//	based on the "full-adder" logic gate definitions:
		//	n  => the index number of the bit operated on
		//	Cp => CARRY from previous bit
		//	An => input A
		//	Bn => input B
		//	Zn => SUM of A, B inputs
		//	Cn => CARRY of A, B inputs
		//
		//	A "full-adder" logic circuit is comprised of 3 inputs (CARRY, A and B)
		//	and 2 outputs (SUM and CARRY). To get these outputs, 2 "half-adder"
		//	circuits are required: one to perform the SUM operation to calculate
		//	Zn and the other to calculate the CARRY (Cn) for further operations.
		//		SUM operations XOR the values of two inputs
		//		CARRY operations AND the values of two inputs
		//
		//	Therefore, we end up with:
		//	Zn = Cp XOR (An XOR Bn)
		//	Cn = (An AND Bn) OR (Cp AND (An XOR Bn))
		//
		//	Some simplification here:
		//	To get Zn, there MUST be an equivalent of (A XOR B) in the map - this
		//	performs the operation for the first half-adder circuit SUM operation.
		//	Any definitions for z-registers which do not match this (except for
		//	the final z-register which has no corresponding input as it is
		//	effectively the final "carry" bit of the addition). Similarly, any
		//	XOR operation that does NOT go into one of the Zn values MUST be
		//	comprised of the product of Xn and Yn inputs

		//	Locate all XOR operations
		var xorOps = fwc.Where(q => q.op.Equals("XOR")).ToList();

		//	Find XOR operations that do not conform to the algorithm:
		var xorSwaps = fwc
			//	Zn (except the last) MUST be an XOR operation
			.Where(q => q.IsFinal)
			.Where(q => q.ix < XGates.Count)
			.Where(q => !q.op.Equals("XOR"))
			//	Non-Zn XOR ops MUST include Xn and Yn inputs
			.Concat(xorOps.Where(q => !q.IsFinal && !q.IsDirect))
			//	All XOR ops themselves (except direct Xn/Yn manipulation) should
			//	themselves be made up of an OR and XOR operation (again, except
			//	for the z01 register)
			.Concat(xorOps.Where(q => !q.IsDirect)
				.Where(q => q.ix != 1)
				.Where(q => q.children.Any(c => c.op.Equals("AND")))
				.SelectMany(m => m.children)
				.SelectMany(m => fwc.Where(q => q.sum.Equals(m.sum)))
				.Where(q => q.op.Equals("AND")))
			//	Extract the problematic register name
			.Select(s => s.sum)
			.ToList();

		//	To get the CARRY operations verified, there needs to be the equivalent
		//	of an (A' OR B') operation performed, where:
		//		A' is (Cp AND (An XOR Bn)) and
		//		B' is (An AND Bn)
		var carrySwaps = fwc
			//	Find all OR operations
			.Where(q => q.op.Equals("OR"))
			//	Select all children for those operations
			.SelectMany(q => q.children)
			//	Find the ones which do NOT perform AND operations
			.Where(q => !q.op.Equals("AND"))
			//	Extract the problematic register name
			.Select(s => s.sum)
			.ToList();

		return string.Join(",", carrySwaps.Concat(xorSwaps).Distinct().Order());
	}

	#endregion
}