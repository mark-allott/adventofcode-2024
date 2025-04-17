using System.Diagnostics;
using AdventOfCode.Extensions;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day25;

public partial class Day25
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
		var lockAndKeyData = InputFileLines.ParseEnumerableOfStringToListOfListOfString();
		var locksAndKeys = lockAndKeyData.Select(s => new LockOrKey(s)).ToList();
		var locks = locksAndKeys.Where(q => q.IsLock).ToList();
		var keys = locksAndKeys.Where(q => !q.IsLock).ToList();

		var result = locks.SelectMany(l => keys, (l, k) => new { Lock = l, Key = k })
			.Where(q => q.Lock.IsHeightFit(q.Key))
			.Count();
		PartOneResult = $"{ChallengeTitle} result = {result}";
		return true;
	}

	#endregion

	#region IPartOneTestable implementation

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	public void PartOneTest()
	{
		var lockAndKeyData = _partOneInputData.ParseEnumerableOfStringToListOfListOfString();
		var locksAndKeys = lockAndKeyData.Select(s => new LockOrKey(s)).ToList();
		var locks = locksAndKeys.Where(q => q.IsLock).ToList();
		var keys = locksAndKeys.Where(q => !q.IsLock).ToList();

		Debug.Assert(5 == locksAndKeys.Count);
		Debug.Assert(2 == locks.Count);
		Debug.Assert(3 == keys.Count);
		Debug.Assert(locksAndKeys.All(a => a.MaxHeight == 5));
		Debug.Assert(locksAndKeys.All(a => a.Heights.Length == 5));

		var heightMatches = locks.SelectMany(l => keys, (l, k) => new { Lock = l, Key = k }).ToList();
		var actualMatches = heightMatches.Where(lk => lk.Lock.IsHeightFit(lk.Key)).ToList();
	}

	private List<string> _partOneInputData = new List<string>()
	{
		"#####",
		".####",
		".####",
		".####",
		".#.#.",
		".#...",
		".....",
		"",
		"#####",
		"##.##",
		".#.##",
		"...##",
		"...#.",
		"...#.",
		".....",
		"",
		".....",
		"#....",
		"#....",
		"#...#",
		"#.#.#",
		"#.###",
		"#####",
		"",
		".....",
		".....",
		"#.#..",
		"###..",
		"###.#",
		"###.#",
		"#####",
		"",
		".....",
		".....",
		".....",
		"#....",
		"#.#..",
		"#.#.#",
		"#####"
	};

	#endregion

	#region Methods

	#endregion
}