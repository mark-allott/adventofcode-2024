using System;
using System.Diagnostics;
using AdventOfCode.Extensions;
using AdventOfCode.Interfaces;

namespace AdventOfCode.Challenges;

public abstract class AbstractDailyChallenge
	: IDailyChallenge
{
	#region ctor

	protected AbstractDailyChallenge(int dayNumber, string filename = "")
	{
		ArgumentOutOfRangeException.ThrowIfLessThan<int>(dayNumber, 1, nameof(dayNumber));
		ArgumentOutOfRangeException.ThrowIfGreaterThan<int>(dayNumber, 25, nameof(dayNumber));
		ArgumentNullException.ThrowIfNullOrWhiteSpace(filename, nameof(filename));

		DayNumber = dayNumber;
		Filename = filename;
		//	Blank out the result strings
		PartOneResult = PartTwoResult = string.Empty;
	}

	#endregion

	#region IDailyChallenge implementation

	public int DayNumber { get; protected set; }

	public string Filename { get; protected set; }

	public string PartOneResult { get; protected set; }

	public string PartTwoResult { get; protected set; }

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
			return PartOne();
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
			return PartTwo();
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

		if (isPartOneTestable)
		{
			Console.WriteLine($"Executing tests in {nameof(IPartOneTestable.PartOneTest)}");
			((IPartOneTestable)this).PartOneTest();
			if (isResettable)
				((IResettable)this).Reset();
			Console.WriteLine($"{nameof(IPartOneTestable.PartOneTest)} tests completed");
		}

		if (isPartTwoTestable)
		{
			Console.WriteLine($"Executing tests in {nameof(IPartTwoTestable.PartTwoTest)}");
			((IPartTwoTestable)this).PartTwoTest();
			if (isResettable)
				((IResettable)this).Reset();
			Console.WriteLine($"{nameof(IPartTwoTestable.PartTwoTest)} tests completed");
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