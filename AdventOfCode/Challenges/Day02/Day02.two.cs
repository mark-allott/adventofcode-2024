using AdventOfCode.Interfaces;

namespace AdventOfCode.Challenges.Day02;

public partial class Day02
	: AbstractDailyChallenge, IAutoRegister
{
	#region Overrides to run part two of challenge

	protected override bool PartTwo()
	{
		LoadAndReadFile();

		var reportLists = ParseLinesToIntegerLists(InputFileLines);

		var safeReports = new List<List<int>>();
		reportLists.ForEach(report =>
		{
			if (IsReportSafe(report))
				safeReports.Add(report);
		});

		PartTwoResult = $"The number of safe reports is: {safeReports.Count}";
		return true;
	}

	#endregion

	private bool IsReportSafe(List<int> report)
	{
		//	If the report is good based on Part One logic, it is still good now
		if (IsReportGood(report))
			return true;

		//	Loop around the different report values, removing one element from the report
		for (int i = 0; i < report.Count; i++)
		{
			//	Make a copy of the original report
			var fixedReport = new List<int>(report);

			//	Mask the entry at index i as to be removed
			fixedReport.RemoveAt(i);

			//	Now the element has been removed in the correct location, test again
			if (IsReportGood(fixedReport))
				return true;
		}

		//	Removing only one item is still invalid, so this must be bad
		return false;
	}
}