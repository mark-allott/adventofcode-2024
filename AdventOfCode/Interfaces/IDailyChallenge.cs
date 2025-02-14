namespace AdventOfCode.Interfaces;

public interface IDailyChallenge
{
	/// <summary>
	/// Executes all parts of the daily challenge
	/// </summary>
	/// <returns>True if all successful, false if any part fails</returns>
	bool Execute();

	/// <summary>
	/// Executes the required steps to perform part one of the daily challenge
	/// </summary>
	/// <returns>True if executed successfully, false if an error occurs</returns>
	bool ExecutePartOne();

	/// <summary>
	/// Executes the required steps to perform part two of the daily challenge
	/// </summary>
	/// <returns>True if executed successfully, false if an error occurs</returns>
	bool ExecutePartTwo();

	/// <summary>
	/// Holds the day number of the challenge
	/// </summary>
	/// <remarks>The day number validity range is between 1 and 25 (as if it were an Advent calendar)</remarks>
	int DayNumber { get; }

	/// <summary>
	/// Holds the name of the input data for the daily challenge
	/// </summary>
	string Filename { get; }

	/// <summary>
	/// Will hold the solution to part one of the daily challenge
	/// </summary>
	string PartOneResult { get; }

	/// <summary>
	/// Will hold the solution to part two of the daily challenge
	/// </summary>
	string PartTwoResult { get; }
}