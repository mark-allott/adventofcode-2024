using System.Diagnostics;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges;

public partial class Day05
	: AbstractDailyChallenge, IAutoRegister
{
	#region Overrides to run part one of challenge

	/// <summary>
	/// Override the base implementation to provide the actual answer
	/// </summary>
	/// <returns>True if successful</returns>
	protected override bool PartOne()
	{
		//	Validate code matches expected rules
		RunPartOneTest();

		LoadAndReadFile();
		ReadInputData();

		//	Load in the rules and page orders for the challenge
		LoadPageOrderingRules(_rawOrderingRules);
		LoadPagesToProduce(_rawPagesToProduce);

		var validPageOrders = GetValidPageOrders(_pagesToProduce);
		var middlePageNumbers = GetMiddlePages(validPageOrders);
		var total = middlePageNumbers.Sum(s => s);
		PartOneResult = $"Total value of middle pages = {total} (from {validPageOrders.Count} requests)";
		return true;
	}

	#endregion

	#region Test Data from challenge

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	private void RunPartOneTest()
	{
		Console.WriteLine($"Executing tests in {nameof(RunPartOneTest)}");
		ReadInputData(testData, true);
		LoadPageOrderingRules(_rawOrderingRules);
		LoadPagesToProduce(_rawPagesToProduce);

		var l1 = _pagesToProduce[0];
		var l2 = _pagesToProduce[1];
		var l3 = _pagesToProduce[2];
		var l4 = _pagesToProduce[3];
		var l5 = _pagesToProduce[4];
		var l6 = _pagesToProduce[5];

		Debug.Assert(true == IsPagesToProduceCorrect(l1));
		Debug.Assert(true == IsPagesToProduceCorrect(l2));
		Debug.Assert(true == IsPagesToProduceCorrect(l3));
		Debug.Assert(false == IsPagesToProduceCorrect(l4));
		Debug.Assert(false == IsPagesToProduceCorrect(l5));
		Debug.Assert(false == IsPagesToProduceCorrect(l6));

		var validPageOrders = GetValidPageOrders(_pagesToProduce);
		Debug.Assert(3 == validPageOrders.Count);

		var middlePages = GetMiddlePages(validPageOrders);
		Debug.Assert(3 == middlePages.Count);
		Debug.Assert(61 == middlePages[0]);
		Debug.Assert(53 == middlePages[1]);
		Debug.Assert(29 == middlePages[2]);

		//	Be nice and Reset the rules/pages from test
		Reset();
		Console.WriteLine($"{nameof(RunPartOneTest)} tests completed");
	}

	/// <summary>
	/// Test data, as provided on the challenge website
	/// </summary>
	private List<string> testData = new List<string>()
	{
		"29|13",
		"47|13",
		"47|29",
		"47|53",
		"47|61",
		"53|13",
		"53|29",
		"61|13",
		"61|29",
		"61|53",
		"75|13",
		"75|29",
		"75|47",
		"75|53",
		"75|61",
		"97|13",
		"97|29",
		"97|47",
		"97|53",
		"97|61",
		"97|75",
		"",
		"75,47,61,53,29",
		"97,61,53,29,13",
		"75,29,13",
		"75,97,47,61,53",
		"61,13,29",
		"97,13,75,29,47"
	};

	#endregion

	/// <summary>
	/// Runs checks on the <paramref name="pages"/> supplied to see if all conditions for producing the pages in the correct order are met
	/// </summary>
	/// <param name="pages">The current list of pages needing to be checked</param>
	/// <returns>True if all pages validate against the rules, otherwise false</returns>
	private bool IsPagesToProduceCorrect(List<int> pages)
	{
		var result = true;
		foreach (var page in pages)
		{
			var rule = _pageOrderingRules.GetPageOrderingRule(page);
			result = result && IsOrderingCorrect(rule, pages);
		}
		return result;
	}

	/// <summary>
	/// Validates the ordering of the <paramref name="pages"/> against the <paramref name="rule"/>
	/// </summary>
	/// <param name="rule">The ruleset relating to the page being tested</param>
	/// <param name="pages">The list of pages under test</param>
	/// <returns>True if the page is in the correct order, otherwise false</returns>
	private bool IsOrderingCorrect(PageOrderingRule rule, List<int> pages)
	{
		//	Get the index of the page being tested
		var indexOfPage = pages.IndexOf(rule.PageNumber);

		//	Loop around each rule in the ruleset
		foreach (var page in rule.Pages)
		{
			//	Attempt to get the index of the page in the rule
			//	Any page in the rule that is not part of the list will result in a -1 index
			var indexOfTestPage = pages.IndexOf(page);

			//	If the page being tested was in the list (indexOfTestPage != -1)
			//	and appears before the index position of rule.PageNumber
			//	(indexOfPage), then we fail the test
			if (indexOfTestPage >= 0 && indexOfTestPage < indexOfPage)
				return false;
		}
		//	All rules have been checked, therefore it passes checks
		return true;
	}

	/// <summary>
	/// Finds the list of valid print orders from the list
	/// </summary>
	/// <param name="pageOrders">The list of pages being ordered</param>
	/// <returns>A list of page orders that satisfy the production rules</returns>
	private List<List<int>> GetValidPageOrders(List<List<int>> pageOrders)
	{
		var result = new List<List<int>>();

		foreach (var pageOrder in pageOrders)
		{
			if (IsPagesToProduceCorrect(pageOrder))
				result.Add(pageOrder);
		}
		return result;
	}

	/// <summary>
	/// From the supplied <paramref name="pageOrders"/>, determine which page
	/// number is in the middle of the list for each order and return those values
	/// </summary>
	/// <param name="pageOrders">The list of page orders to return</param>
	/// <returns>The list of middle page numbers</returns>
	private List<int> GetMiddlePages(List<List<int>> pageOrders)
	{
		var middlePages = new List<int>();

		foreach (var order in pageOrders)
		{
			var index = order.Count / 2;
			middlePages.Add(order[index]);
		}
		return middlePages;
	}
}