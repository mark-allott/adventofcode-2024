using System.Collections.Immutable;

namespace AdventOfCode.Models;

internal class PageOrderingRule
{
	public int PageNumber { get; private set; }

	public ReadOnlySpan<int> Pages
	{
		get
		{
			return _pages.ToImmutableArray().AsSpan();
		}
	}

	private List<int> _pages = new List<int>();

	public PageOrderingRule(int pageNumber)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageNumber, nameof(pageNumber));

		PageNumber = pageNumber;
	}

	public void AddRule(int pageNumber)
	{
		_pages.Add(pageNumber);
	}
}