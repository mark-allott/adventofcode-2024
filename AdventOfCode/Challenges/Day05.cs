using AdventOfCode.Extensions;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges;

public partial class Day05
	: AbstractDailyChallenge, IAutoRegister, IResettable
{
	#region Overrides of AbstractDailyChallenge

	public Day05()
		: base(5, "day05-input.txt")
	{
	}

	#endregion

	/// <summary>
	/// Holds the set of rules to be followed
	/// </summary>
	private PageOrderingRules _pageOrderingRules = new PageOrderingRules();

	/// <summary>
	/// Holds the list of pages to be produced
	/// </summary>
	private List<List<int>> _pagesToProduce = new List<List<int>>();

	/// <summary>
	/// Holds the raw data for the ordering rules
	/// </summary>
	private List<string> _rawOrderingRules = new List<string>();

	/// <summary>
	/// Holds the raw data for the pages to be produced
	/// </summary>
	private List<string> _rawPagesToProduce = new List<string>();

	/// <summary>
	/// Reads the data from the supplied source (or from <see cref="AbstractDailyChallenge.InputFileLines"/>)
	/// </summary>
	/// <param name="data"></param>
	/// <param name="forceReload"></param>
	private void ReadInputData(IEnumerable<string> data = null!, bool forceReload = false)
	{
		//	Use data provided, or the contents of the file previously read
		data ??= InputFileLines;

		//	If we already have data loaded, don't reload unless we need to
		if (!forceReload &&
			_rawOrderingRules.Count > 0 &&
			_rawPagesToProduce.Count > 0)
			return;

		//	ensure we have a set of clean working datasets to begin with
		var readOrderingRules = true;
		_rawOrderingRules.Clear();
		_rawPagesToProduce.Clear();
		_pageOrderingRules.Clear();
		_pagesToProduce.Clear();

		//	Read through the data supplied
		foreach (var line in data)
		{
			//	A blank line indicates a switch in processing
			if (string.IsNullOrWhiteSpace(line))
			{
				readOrderingRules = !readOrderingRules;
				continue;
			}

			//	Load the loine of data into the appropriate raw dataset
			if (readOrderingRules)
				_rawOrderingRules.Add(line);
			else
				_rawPagesToProduce.Add(line);
		}
	}

	/// <summary>
	/// From data supplied in <paramref name="lines"/>, add the page ordering rules to the system
	/// </summary>
	/// <param name="lines">The rules to be loaded</param>
	private void LoadPageOrderingRules(IEnumerable<string> lines)
	{
		foreach (var line in lines)
		{
			_pageOrderingRules.AddPageOrderingRule(line);
		}
		Console.WriteLine($"{nameof(LoadPageOrderingRules)} completed. Loaded {_pageOrderingRules.RuleCount} rules.");
	}

	/// <summary>
	/// From data supplied in <paramref name="lines"/>, load the list of pages to be produced
	/// </summary>
	/// <param name="lines">The list of pages to be produced</param>
	private void LoadPagesToProduce(IEnumerable<string> lines)
	{
		foreach (var line in lines)
		{
			var pageList = line.ParseStringToListOfInt();
			_pagesToProduce.Add(pageList);
		}
	}

	#region IResettable implementation

	/// <summary>
	/// Resets the datasets to allow processing of a different set of rules
	/// </summary>
	public void Reset()
	{
		Console.WriteLine($"Executing {nameof(Day05)}.{nameof(Reset)}");
		_pageOrderingRules?.Clear();
		_pagesToProduce?.Clear();
		_rawOrderingRules?.Clear();
		_rawPagesToProduce?.Clear();
		InputFileLines?.Clear();
		Console.WriteLine($"{nameof(Day05)}.{nameof(Reset)} completed");
	}

	#endregion
}