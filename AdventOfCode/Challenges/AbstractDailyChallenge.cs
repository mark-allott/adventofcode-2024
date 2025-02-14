using System;
using System.Diagnostics;
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

	public int DayNumber { get; protected set; }

	public string Filename { get; protected set; }

	public string PartOneResult { get; protected set; }

	public string PartTwoResult { get; protected set; }

	#region IDailyChallenge implementation

	/// <inheritdoc/>
	public bool Execute()
	{
		try
		{
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
		catch (System.Exception e)
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
	public virtual bool ExecutePartTwo()
	{
		try
		{
			return PartTwo();
		}
		catch (System.Exception e)
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

	#endregion
}