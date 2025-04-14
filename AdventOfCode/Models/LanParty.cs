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
			//	Get all links from "A" to "B"
			var triplets = _computerNamesAndConnections[computerName]
				.Select(s => new { A = computerName, B = s })
				//	Get all links from "B" to "C", retain "A"
				.SelectMany(s => _computerNamesAndConnections[s.B].Select(cs => new { A = s.A, B = s.B, C = cs }))
				//	Filter for connections where "C" links back to "A"
				.Where(q => _computerNamesAndConnections[q.C].Contains(computerName))
				.ToList();

			//	Convert the triplets to text and take only distinct paths
			var links = triplets.Select(s => new List<string>() { s.A, s.B, s.C })
				.Select(s => string.Join(",", s.Order()))
				.Distinct()
				.ToList();
			result.AddRange(links);
		}
		//	There may be reverse paths we have found, so make sure only truly distinct ones are returned
		return result.Distinct()
			.Order()
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
	#endregion
}