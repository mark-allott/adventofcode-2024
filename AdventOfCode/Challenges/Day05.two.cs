using System.Diagnostics;
using AdventOfCode.Interfaces;

namespace AdventOfCode.Challenges;

public partial class Day05
	: AbstractDailyChallenge, IAutoRegister, IPartTwoTestable
{
	#region Overrides to run part two of challenge

	/// <summary>
	/// Overrides the base implementation to provide the solution for part two
	/// </summary>
	/// <returns></returns>
	protected override bool PartTwo()
	{
		Reset();

		LoadAndReadFile();
		ReadInputData();

		//	Load in the rules and page orders for the challenge
		LoadPageOrderingRules(_rawOrderingRules);
		LoadPagesToProduce(_rawPagesToProduce);

		var invalidPageOrders = GetInvalidPageOrders(_pagesToProduce);
		var validPageOrders = MakeValidPageOrders(invalidPageOrders);

		var middlePageNumbers = GetMiddlePages(validPageOrders);
		var total = middlePageNumbers.Sum(s => s);
		PartTwoResult = $"The sum of middle pages for re-ordered requests is {total}";
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
		ReadInputData(testData, true);
		LoadPageOrderingRules(_rawOrderingRules);
		LoadPagesToProduce(_rawPagesToProduce);

		var invalid1 = new List<int>() { 75, 97, 47, 61, 53 };
		var expected1 = new List<int>() { 97, 75, 47, 61, 53 };
		var invalid2 = new List<int>() { 61, 13, 29 };
		var expected2 = new List<int>() { 61, 29, 13 };
		var invalid3 = new List<int>() { 97, 13, 75, 29, 47 };
		var expected3 = new List<int>() { 97, 75, 47, 29, 13 };

		var result1 = MakeValidPageOrder(invalid1);
		var result2 = MakeValidPageOrder(invalid2);
		var result3 = MakeValidPageOrder(invalid3);

		Debug.Assert(AreListsEquivalent(expected1, result1));
		Debug.Assert(AreListsEquivalent(expected2, result2));
		Debug.Assert(AreListsEquivalent(expected3, result3));
	}

	/// <summary>
	/// Perform a check to see if two lists of integers are equivalent
	/// </summary>
	/// <param name="a">The expected results</param>
	/// <param name="b">The results being tested</param>
	/// <returns>True if all as expected, otherwise false</returns>
	private bool AreListsEquivalent(List<int> a, List<int> b)
	{
		try
		{
			ArgumentOutOfRangeException.ThrowIfNotEqual(a.Count, b.Count);
			for (var i = 0; i < a.Count; i++)
				ArgumentOutOfRangeException.ThrowIfNotEqual(a[i], b[i]);
		}
		catch (Exception e)
		{
			Debug.WriteLine($"{nameof(AreListsEquivalent)} assertion failure: {e.Message}");
			return false;
		}
		return true;
	}

	#endregion

	/// <summary>
	/// Finds the list of invalid print orders from the list, ready to be fixed
	/// </summary>
	/// <param name="pageOrders">The list of pages being ordered</param>
	/// <returns>A list of page orders that do not satisfy the production rules</returns>
	private List<List<int>> GetInvalidPageOrders(List<List<int>> pageOrders)
	{
		var result = new List<List<int>>();

		foreach (var pageOrder in pageOrders)
		{
			if (!IsPagesToProduceCorrect(pageOrder))
				result.Add(pageOrder);
		}
		return result;
	}

	/// <summary>
	/// Given the list of invalid page orders in <paramref name="invalidOrders"/>,
	/// loop around and make a valid list based on the rules supplied
	/// </summary>
	/// <param name="invalidOrders">The list of invalid page orders</param>
	/// <returns>The rectified list of page orders</returns>
	private List<List<int>> MakeValidPageOrders(List<List<int>> invalidOrders)
	{
		var newOrders = new List<List<int>>();

		foreach (var pageOrder in invalidOrders)
		{
			newOrders.Add(MakeValidPageOrder(pageOrder));
		}

		return newOrders;
	}

	/// <summary>
	/// For a given list of page orders (assumed to be invalid), re-order the
	/// list into a valid list of page orders
	/// </summary>
	/// <param name="current">The invalid list</param>
	/// <returns>The corrected list of page orders</returns>
	/// <exception cref="ArgumentException"></exception>
	private List<int> MakeValidPageOrder(List<int> current)
	{
		var newOrder = new List<int>();

		foreach (var page in current)
		{
			//	Add the page to the newOrder
			newOrder.Add(page);

			//	If this is the first page in the newOrder list, skip any other
			//	processing as it will always be correct on first run (there
			//	being no other entry to conflict)
			if (newOrder.Count == 1)
				continue;

			//	Check to see if the current order is good, if so, continue to next number
			if (IsPagesToProduceCorrect(newOrder))
				continue;

			//	Extract the index of the page
			var index = newOrder.IndexOf(page);

			for (var i = index; i > 0; i--)
			{
				int swap = newOrder[i - 1];
				newOrder[i - 1] = page;
				newOrder[i] = swap;

				//	Check the new order to see if it is correct
				if (IsPagesToProduceCorrect(newOrder))
					break;
			}
		}

		//	If we escaped the loop without managing to create a correct order
		//	then we need to raise an exception as the run will fail
		if (!IsPagesToProduceCorrect(newOrder))
			throw new ArgumentException("Unable to create valid order");

		//	All good, so return the newly minted order
		return newOrder;
	}
}