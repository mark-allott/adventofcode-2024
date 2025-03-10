using System;
using System.Diagnostics;
using AdventOfCode.Extensions;
using AdventOfCode.Interfaces;

namespace AdventOfCode.Challenges;

public abstract class AbstractDailyChallenge
	: IDailyChallenge
{
	#region ctor

	protected AbstractDailyChallenge(int dayNumber, string filename = "", string title = "")
	{
		ArgumentOutOfRangeException.ThrowIfLessThan<int>(dayNumber, 1, nameof(dayNumber));
		ArgumentOutOfRangeException.ThrowIfGreaterThan<int>(dayNumber, 25, nameof(dayNumber));
		ArgumentNullException.ThrowIfNullOrWhiteSpace(filename, nameof(filename));

		DayNumber = dayNumber;
		Filename = filename;
		ChallengeTitle = title;
		//	Blank out the result strings
		PartOneResult = PartTwoResult = string.Empty;
	}

	#endregion

	#region IDailyChallenge implementation

	public int DayNumber { get; protected set; }

	public string Filename { get; protected set; }

	public string PartOneResult { get; protected set; }

	public string PartTwoResult { get; protected set; }

	public string ChallengeTitle { get; private set; }

	/// <inheritdoc/>
	public bool Execute()
	{
		try
		{
			ExecuteTests();
			var result1 = ExecutePartOne();
			var result2 = ExecutePartTwo();
			return result1 && result2;
		}
		catch (Exception e)
		{
			Debug.WriteLine(e.Message);
			return false;
		}
	}

	/// <inheritdoc/>
	public bool ExecutePartOne()
	{
		try
		{
			var sw = Stopwatch.StartNew();
			try
			{
				return PartOne();
			}
			finally
			{
				sw.Stop();
				PartOneResult = $"{PartOneResult} ({sw.Elapsed.Ticks} Ticks)";
			}
		}
		catch (Exception e)
		{
			PartOneResult = e.Message;
			return false;
		}
	}

	/// <summary>
	/// Performs the actual steps required to provide the solution to part one of the daily challenge.
	// The solution should be stored in the property <see cref="PartOneResult"/>
	/// </summary>
	/// <returns>True if successful, false if not</returns>
	/// <exception cref="NotImplementedException"></exception>
	protected virtual bool PartOne()
	{
		throw new NotImplementedException($"{nameof(ExecutePartOne)} is not implemented");
	}

	/// <inheritdoc/>
	public bool ExecutePartTwo()
	{
		try
		{
			var sw = Stopwatch.StartNew();
			try
			{
				return PartTwo();
			}
			finally
			{
				sw.Stop();
				PartTwoResult = $"{PartTwoResult} ({sw.Elapsed.Ticks} Ticks)";
			}
		}
		catch (Exception e)
		{
			PartTwoResult = e.Message;
			return false;
		}
	}

	/// <summary>
	/// Performs the actual steps required to provide the solution to part two of the daily challenge.
	// The solution should be stored in the property <see cref="PartTwoResult"/>
	/// </summary>
	/// <returns>True if successful, false if not</returns>
	/// <exception cref="NotImplementedException"></exception>
	protected virtual bool PartTwo()
	{
		throw new NotImplementedException($"{nameof(ExecutePartTwo)} is not implemented");
	}

	/// <summary>
	/// If the class is testable (by decorating with IPartOneTestable, IPartTwoTestable or ITestable), run those tests
	/// </summary>
	private void ExecuteTests()
	{
		var isPartOneTestable = this.GetType()
			.GetInterfaces()
			.Any(x => x.IsAssignableFrom(typeof(IPartOneTestable)));
		var isPartTwoTestable = this.GetType()
			.GetInterfaces()
			.Any(x => x.IsAssignableFrom(typeof(IPartTwoTestable)));

		var isResettable = this.GetType()
			.GetInterfaces()
			.Any(x => x.IsAssignableFrom(typeof(IResettable)));

		var sw = new Stopwatch();
		if (isPartOneTestable)
		{
			Console.WriteLine($"Executing tests in {nameof(IPartOneTestable.PartOneTest)}");
			sw.Start();
			((IPartOneTestable)this).PartOneTest();
			sw.Stop();
			if (isResettable)
				((IResettable)this).Reset();
			Console.WriteLine($"{nameof(IPartOneTestable.PartOneTest)} tests completed ({sw.Elapsed.Ticks} Ticks)");
		}

		if (isPartTwoTestable)
		{
			sw.Reset();
			Console.WriteLine($"Executing tests in {nameof(IPartTwoTestable.PartTwoTest)}");
			sw.Start();
			((IPartTwoTestable)this).PartTwoTest();
			sw.Stop();
			if (isResettable)
				((IResettable)this).Reset();
			Console.WriteLine($"{nameof(IPartTwoTestable.PartTwoTest)} tests completed ({sw.Elapsed.Ticks} Ticks)");
		}
	}

	#endregion

	#region Utility methods

	/// <summary>
	/// Holds the "raw" contents of the input file
	/// </summary>
	protected List<string> InputFileLines = null!;

	/// <summary>
	/// Loads the file specified in the constructor from the data folder
	/// </summary>
	/// <param name="forceReload">Indicates whether a reload of the data is required</param>
	protected void LoadAndReadFile(bool forceReload = false)
	{
		//	Don't re-read data if already present, or forced
		if (!(InputFileLines is null || InputFileLines.Count == 0 || forceReload))
			return;

		try
		{
			var cwd = Directory.GetCurrentDirectory();
			var inputFilePath = Path.Combine(cwd, "data", Filename);
			InputFileLines = new List<string>(File.ReadAllLines(inputFilePath));
		}
		catch (IOException iox)
		{
			Console.WriteLine(iox.Message);
		}
	}

	/// <summary>
	/// Parses the <paramref name="lines"/> supplied and converts in to a list of integer lists
	/// </summary>
	/// <param name="lines">The strings forming the lists to be parsed</param>
	/// <returns>A list of integer lists, representing the contents of the <paramref name="lines"/></returns>
	/// <exception cref="ArgumentException"></exception>
	protected static List<List<int>> ParseLinesToIntegerLists(IEnumerable<string> lines)
	{
		var results = new List<List<int>>();
		var rowNumber = 1;

		foreach (var line in lines)
		{
			var rowValues = line.ParseStringToListOfInt(rowNumber);
			results.Add(rowValues);
			rowNumber++;
		}
		return results;
	}
	#endregion
}