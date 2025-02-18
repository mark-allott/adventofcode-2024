using AdventOfCode.Extensions;

namespace AdventOfCode.Models;

internal class PageOrderingRules
{
	private Dictionary<int, PageOrderingRule> _ruleset;
	private int _ruleCount = 0;

	#region ctor


	public PageOrderingRules()
	{
		_ruleset = new Dictionary<int, PageOrderingRule>();
	}

	#endregion

	public PageOrderingRule GetPageOrderingRule(int pageIndex)
	{
		//	If there is already a ruleset for the page, return that
		if (_ruleset.TryGetValue(pageIndex, out var rule))
			return rule;

		//	No ruleset exists, so create a new blank one and return that
		var newRule = new PageOrderingRule(pageIndex);
		_ruleset[pageIndex] = newRule;
		return newRule;
	}

	public void AddPageOrderingRule(string rule)
	{
		//	Error if the rule is blank
		ArgumentNullException.ThrowIfNullOrWhiteSpace(rule);

		//	Rules should be separated by the pipe character and there must be 2 parts afterwards
		var parts = rule.Split('|');
		ArgumentOutOfRangeException.ThrowIfNotEqual(parts.Length, 2, nameof(rule));

		//	Convert the strings into ints - anything not capable of being converted will generate an exception
		var pageValues = parts.ParseEnumerableOfStringToListOfInt();

		AddPageOrderingRule(pageValues[0], pageValues[1]);
	}

	public void AddPageOrderingRule(int page, int mustAppearBefore)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(page, 1, nameof(page));
		ArgumentOutOfRangeException.ThrowIfLessThan(mustAppearBefore, 1, nameof(mustAppearBefore));
		var ruleset = GetPageOrderingRule(page);
		ruleset.AddRule(mustAppearBefore);
		_ruleCount++;
	}

	public bool PageHasRule(int page)
	{
		return _ruleset.TryGetValue(page, out var rule);
	}

	public int RuleCount => _ruleCount;

	public void Clear()
	{
		_ruleset.Clear();
		_ruleCount = 0;
	}
}