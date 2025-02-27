using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day08;

public partial class Day08
	: AbstractDailyChallenge, IAutoRegister, IPartTwoTestable
{
	#region Overrides to run part two of challenge

	/// <summary>
	/// Overrides the base implementation to provide the solution for part two
	/// </summary>
	/// <returns></returns>
	protected override bool PartTwo()
	{
		LoadAndReadFile();

		var antennaList = GetAntennas(InputFileLines);
		(int row, int col) bounds = (InputFileLines.Count, InputFileLines[0].Length);
		var antinodeList = GetAntinodesWithHarmonics(antennaList, bounds);
		var antinodesWithinBounds = antinodeList.Where(c => c.InBounds).ToList();
		var uniqueAntinodeLocations = antinodesWithinBounds
			.Distinct(new AntinodeEqualityComparer())
			.OrderBy(o => o.Row)
			.ThenBy(o => o.Column)
			.ToList();

		long total = uniqueAntinodeLocations.Count;
		PartTwoResult = $"Total number of antinodes = {total}";
		return true;
	}

	#endregion

	#region Test data for part two

	/// <summary>
	/// Using rules set out in the challenge, run some tests to make sure the
	/// code behaves as expected
	/// </summary>
	public void PartTwoTest()
	{
		//	Initial load of data should locate 7 antenna
		var antennaList = GetAntennas(_partOneTestInput);
		Debug.Assert(7 == antennaList.Count);

		//	Determine upper bounds of the map
		(int row, int col) bounds = (_partOneTestInput.Count, _partOneTestInput[0].Length);

		var antinodeList = GetAntinodesWithHarmonics(antennaList, bounds);

		//	Filter for antinodes physically on the map
		var antinodesWithinBounds = antinodeList.Where(c => c.InBounds).ToList();

		//	Filter for unique locations
		var uniqueAntinodeLocations = antinodesWithinBounds
			.Distinct(new AntinodeEqualityComparer())
			.OrderBy(o => o.Row)
			.ThenBy(o => o.Column)
			.ToList();

		//	Test specs state there should be 34 locations due to harmonics
		Debug.Assert(34 == uniqueAntinodeLocations.Count);
	}

	#endregion

	/// <summary>
	/// Helper method to determine the list of antinodes based on the list of antennae
	/// </summary>
	/// <param name="antennae">The list of broadcast antennae on the map</param>
	/// <param name="bounds">The upper-bounds of the map coordinates</param>
	/// <returns>A list of all antinodes created by the antennae</returns>
	private static List<Antinode> GetAntinodesWithHarmonics(List<Antenna> antennae, (int row, int column) bounds)
	{
		var p = new List<Antenna>(antennae);
		var pairedAntennae = antennae
			.Join(p, b => b.Frequency, p => p.Frequency, (b, p) => new { Broadcast = b, Paired = p })
			.Where(q => q.Broadcast != q.Paired)
			.Select(s => new PairedAntenna(s.Broadcast, s.Paired, bounds))
			.ToList();

		var antinodes = new List<Antinode>();
		//	Based on rules for part two, create new Antinode objects
		foreach (var pair in pairedAntennae)
			antinodes.AddRange(pair.GetAntinodes());

		return antinodes;
	}

}