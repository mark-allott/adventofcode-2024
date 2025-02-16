using AdventOfCode.Interfaces;

namespace AdventOfCode.Challenges;

public partial class Day02
	: AbstractDailyChallenge, IAutoRegister
{
	#region Overrides to run part one of challenge

	protected override bool PartOne()
	{
		LoadAndReadFile();

		var reportLists = ParseLinesToIntegerLists(InputFileLines);

		var goodReports = new List<List<int>>();
		reportLists.ForEach(report =>
		{
			if (IsReportGood(report))
				goodReports.Add(report);
		});

		PartOneResult = $"The number of good reports is: {goodReports.Count}";
		return true;
	}

	#endregion

	private bool IsReportGood(List<int> input)
	{
		var deltas = GetDeltas(input);

		//	Cannot contain any elements that are zero
		var nonZero = deltas.All(q => q != 0);
		//	Report deltas must be either all positive or all negative (ascending or descending orders)
		var ascOrDesc = deltas.All(q => q > 0) || deltas.All(q => q < 0);
		//	Report deltas must be between 1 and 3 to be valid
		var goodRange = deltas.All(q => Math.Abs(q) < 4);

		return nonZero && ascOrDesc && goodRange;
	}

	private List<int> GetDeltas(List<int> input)
	{
		var output = new List<int>();

		for (var i = 0; i < input.Count - 1; i++)
		{
			var delta = input[i + 1] - input[i];
			output.Add(delta);
		}

		return output;
	}
}