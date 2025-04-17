namespace AdventOfCode.Models;

internal class LanParty
{
	#region Fields

	private Dictionary<string, List<string>> _computerNamesAndConnections = new Dictionary<string, List<string>>();

	#endregion

	#region Properties
	#endregion

	#region Ctor
	#endregion

	#region Methods

	/// <summary>
	/// Using data previously read into <paramref name="input"/>, parse the data to find computers and connections
	/// </summary>
	/// <param name="input">A list of strings representing pairs of computers</param>
	public void LoadFromInput(IEnumerable<string> input)
	{
		//	Remove any previous data
		_computerNamesAndConnections.Clear();

		foreach (var computerNames in input)
		{
			//	Computers are in pairs expressed by the scheme aa-bb, so extract each computer name
			var pair = computerNames.Split('-')
				//	Ensure names are ordered alphabetically
				.Order().ToArray();

			//	Assign the link to computer "B" to existing links from "A"
			if (!_computerNamesAndConnections.TryGetValue(pair[0], out var connA))
				_computerNamesAndConnections[pair[0]] = connA = new List<string>();
			connA.Add(pair[1]);

			//	Assign the reverse link to computer "A" to existing links from "B"
			if (!_computerNamesAndConnections.TryGetValue(pair[1], out var connB))
				_computerNamesAndConnections[pair[1]] = connB = new List<string>();
			connB.Add(pair[0]);
		}
	}

	/// <summary>
	/// Returns the list of computers that are "triplets" - i.e. they link in a circular fashion akin to: A -> B -> C -> A
	/// </summary>
	/// <returns>The list of triplets in alphabetic order</returns>
	public List<string> GetTripletLinks()
	{
		var result = new List<string>();

		//	Iterate over the computer names alphabetically
		foreach (var computerName in _computerNamesAndConnections.Keys.Order())
		{
			result.AddRange(GetTripletFor(computerName));
		}
		//	There may be reverse paths we have found, so make sure only truly distinct ones are returned
		return result.Distinct()
			.Order()
			.ToList();
	}

	private List<string> GetTripletFor(string computerName)
	{
		//	Get all links from "A" to "B"
		var triplets = _computerNamesAndConnections[computerName]
			.Select(s => new { A = computerName, B = s })
			//	Get all links from "B" to "C", retain "A"
			.SelectMany(s => _computerNamesAndConnections[s.B].Select(cs => new { A = s.A, B = s.B, C = cs }))
			//	Filter for connections where "C" links back to "A"
			.Where(q => _computerNamesAndConnections[q.C].Contains(computerName))
			.ToList();

		//	Convert the triplets to text and take only distinct paths
		return triplets.Select(s => new List<string>() { s.A, s.B, s.C })
			.Select(s => string.Join(",", s.Order()))
			.Distinct()
			.ToList();
	}

	/// <summary>
	/// Returns the list of computer triplets where one computer in the triplet starts with the letter <paramref name="startLetter"/>
	/// </summary>
	/// <param name="startLetter">The starting letter of the computer which must be part of the triplet</param>
	/// <returns>All triplets matching the requirements in alphabetical order</returns>
	public List<string> GetTripletLinksWithComputerNamesStartingWith(char startLetter)
	{
		var triplets = GetTripletLinks();

		var x = triplets.Select(s => new { triplet = s, computers = s.Split(",") })
			.Where(q => q.computers.Any(n => n.StartsWith(startLetter)))
			.Select(s => s.triplet)
			.ToList();
		return x;
	}

	/// <summary>
	/// Get the password for the LAN Party based on largest network
	/// </summary>
	/// <returns></returns>
	public string GetLanPartyPassword()
	{
		var candidates = GetNetworks();
		return candidates.FirstOrDefault() ?? "";
	}

	/// <summary>
	/// Holds lookups for network nodes and best connections found
	/// </summary>
	private Dictionary<string, List<string>> _lookups = null!;

	/// <summary>
	/// Finds all networks for the computers in the problem
	/// </summary>
	/// <returns>All networks, ordered by largest to smallest</returns>
	private List<string> GetNetworks()
	{
		//	Initialise
		_lookups = new Dictionary<string, List<string>>();
		var results = new List<string>();

		//	look for networks
		_computerNamesAndConnections.Keys
			.ToList()
			.ForEach(n => results.AddRange(GetNetworkFor(n)));

		//	Get the distinct results, order and return
		results = results.Distinct()
			.OrderByDescending(o => o.Length)
			.ThenBy(o => o)
			.ToList();
		return results;
	}

	/// <summary>
	/// Get the network of computers from the given list of computers
	/// </summary>
	/// <param name="computerList">The computer (or computers) in the current network</param>
	/// <returns>All computers in the network that contain <paramref name="computerList"/></returns>
	private List<string> GetNetworkFor(string computerList)
	{
		//	Do we have a suitable return value for this key?
		if (_lookups.TryGetValue(computerList, out var result))
			return result;

		//	Split into list for lookups
		var computers = computerList.Split(",").ToList();
		var connections = _computerNamesAndConnections.Keys
			//	Connections must contain ALL nodes
			.Where(key => computers.All(cn => _computerNamesAndConnections[key].Contains(cn)))
			//	Recombine computer names for iterative lookup
			.Select(s => string.Join(",", new List<string>(computers) { s }.Order()))
			.ToList();

		//	If no connections were found, then we have the result
		if (connections.Count == 0)
			return AddLookup(computerList, computerList);

		//	Perform a recurisive lookup for all new candidate connections
		var results = connections
			.Select(s => GetNetworkFor(s))
			.SelectMany(m => m)
			.Distinct()
			.ToList();
		//	Find the best result (or results)
		var best = results.Where(q => q.Length == results.Max(m => m.Length))
			.Distinct()
			.ToList();
		return AddLookup(computerList, best);
	}

	/// <summary>
	/// Adds the <paramref name="value"/> to the dictionary for the given <paramref name="key"/>
	/// </summary>
	/// <param name="key">The key to be updated</param>
	/// <param name="value">The value to be added</param>
	/// <returns>The distinct entries for the <paramref name="key"/></returns>
	private List<string> AddLookup(string key, string value)
	{
		if (!_lookups.TryGetValue(key, out var existing))
			existing = new List<string>();
		existing.Add(value);
		return _lookups[key] = existing.Distinct().ToList();
	}

	/// <summary>
	/// Adds the given <paramref name="values"/> to the dictionary for the specified <paramref name="key"/>
	/// </summary>
	/// <param name="key">The key to be updated</param>
	/// <param name="values">Entries to be added</param>
	/// <returns>The distinct entries for the <paramref name="key"/></returns>
	private List<string> AddLookup(string key, IEnumerable<string> values)
	{
		if (!_lookups.TryGetValue(key, out var existing))
			existing = new List<string>();
		existing.AddRange(values);
		return _lookups[key] = existing.Distinct().ToList();
	}

	#endregion
}