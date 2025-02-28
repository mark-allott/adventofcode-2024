using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day08;

public partial class Day08
	: AbstractDailyChallenge, IAutoRegister, IPartOneTestable
{
	#region Overrides to run part one of challenge

	/// <summary>
	/// Override the base implementation to provide the actual answer
	/// </summary>
	/// <returns>True if successful</returns>
	protected override bool PartOne()
	{
		LoadAndReadFile();

		var antennaList = GetAntennas(InputFileLines);
		(int row, int col) bounds = (InputFileLines.Count, InputFileLines[0].Length);
		var antinodeList = GetAntinodes(antennaList, bounds);
		var antinodesWithinBounds = antinodeList.Where(c => c.InBounds).ToList();
		var uniqueAntinodeLocations = antinodesWithinBounds
			.Distinct(new AntinodeEqualityComparer())
			.OrderBy(o => o.Row)
			.ThenBy(o => o.Column)
			.ToList();

		long total = uniqueAntinodeLocations.Count;
		PartOneResult = $"Total number of antinodes = {total}";
		return true;
	}

	#endregion

	#region IPartOneTestable implementation

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	public void PartOneTest()
	{
		//	Initial load of data should locate 7 antenna
		var antennaList = GetAntennas(_partOneTestInput);
		Debug.Assert(7 == antennaList.Count);

		//	Determine upper bounds of the map
		(int row, int col) bounds = (_partOneTestInput.Count, _partOneTestInput[0].Length);

		//	Locate all antinodes based on part one specification
		var antinodeList = GetAntinodes(antennaList, bounds);

		//	Filter for antinodes physically on the map
		var antinodesWithinBounds = antinodeList.Where(c => c.InBounds).ToList();

		//	Filter for unique locations
		var uniqueAntinodeLocations = antinodesWithinBounds
			.Distinct(new AntinodeEqualityComparer())
			.OrderBy(o => o.Row)
			.ThenBy(o => o.Column)
			.ToList();

		//	Test specs state there should be 14 locations
		Debug.Assert(14 == uniqueAntinodeLocations.Count);
	}

	/// <summary>
	/// test data, as per challenge specifications
	/// </summary>
	private List<string> _partOneTestInput = new List<string>()
	{
		"............",
		"........0...",
		".....0......",
		".......0....",
		"....0.......",
		"......A.....",
		"............",
		"............",
		"........A...",
		".........A..",
		"............",
		"............"
	};

	#endregion

	#region Part One code

	/// <summary>
	/// Hepler method to load map information and detect locations of antennae
	/// </summary>
	/// <param name="input">The map data</param>
	/// <returns>A list of <see cref="Antenna"/> objects</returns>
	private static List<Antenna> GetAntennas(List<string> input)
	{
		ArgumentNullException.ThrowIfNull(input, nameof(input));

		var antennae = new List<Antenna>();
		var row = 0;

		//	Loop for each line of input (y-coordinates)
		foreach (var line in input)
		{
			var col = 0;
			//	Loop for each character (x-coordinates)
			foreach (var c in line.ToCharArray())
			{
				//	Anything which is a letter or digit represents an antenna of that frequency
				if (char.IsLetterOrDigit(c))
					antennae.Add(new Antenna(c, row, col));
				col++;
			}
			row++;
		}
		return antennae;
	}

	/// <summary>
	/// Helper method to determine the list of antinodes based on the list of antennae
	/// </summary>
	/// <param name="antennae">The list of broadcast antennae on the map</param>
	/// <param name="bounds">The upper-bounds of the map coordinates</param>
	/// <returns>A list of all antinodes created by the antennae</returns>
	private static List<Antinode> GetAntinodes(List<Antenna> antennae, (int row, int column) bounds)
	{
		var p = new List<Antenna>(antennae);
		var pairedAntennae = antennae
			.Join(p, b => b.Frequency, p => p.Frequency, (b, p) => new { Broadcast = b, Paired = p })
			.Where(q => q.Broadcast != q.Paired)
			.ToList();

		//	Based on rules for part one, create new Antinode objects
		return pairedAntennae.Select(s => new Antinode(s.Broadcast, s.Paired, bounds))
			.ToList();
	}

	#endregion
}