using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AdventOfCode.Models;

/// <summary>
/// <para>Model class to solve the problem posed by the following data:</para>
/// <para>
/// Button A: X+94, Y+34<br/>
/// Button B: X+22, Y+67<br/>
/// Prize: X=8400, Y=5400<br/>
/// </para>
/// <para>
/// This is essentially a bit of simple mathematics in solving a set of simultaneous linear equations and can be expressed as the following:<br/>
/// 94A + 22B = 8400<br/>
/// 34A + 67B = 5400<br/>
/// Where: A represents a press of button A and B represents a press of button B and the result is the X,Y coordinate of the prize's location
/// </para>
/// <para>
/// First step is to normalise the equation(s) such that one of the terms is eliminated, leaving behind a simple equation to yield the result of the other
/// </para>
/// </summary>
internal partial class ClawContraptionSolver
{
	#region Fields

	/// <summary>
	/// Private regex helper to pull the X & Y values from the input string
	/// </summary>
	/// <returns></returns>
	[GeneratedRegex(@"X[=+](\d+).*Y[=+](\d+)", RegexOptions.Compiled)]
	private static partial Regex ValueSplitterRegex();

	private readonly Regex _valueSplitter = ValueSplitterRegex();

	/// <summary>
	/// Holds the directional coordinates for button A
	/// </summary>
	private readonly (int x, int y) _buttonA;

	/// <summary>
	/// Holds the directional coordinates for button B
	/// </summary>
	private readonly (int x, int y) _buttonB;

	/// <summary>
	/// Holds the coordinates for the prize
	/// </summary>
	private (long x, long y) _prize;

	#endregion

	#region Properties

	/// <summary>
	/// indicates whether the puzzle can be solved (initially false and calling <see cref="Solve"/> will update this
	/// </summary>
	public bool IsSolveable { get; private set; }

	/// <summary>
	/// Holds the calculated value for A after a call to <see cref="Solve"/> has been made
	/// </summary>
	public long ValueForA { get; private set; } = -1;

	/// <summary>
	/// Holds the calculated value for B after a call to <see cref="Solve"/> has been made
	/// </summary>
	public long ValueForB { get; private set; } = -1;

	#endregion

	#region ctor

	public ClawContraptionSolver(string aLine, string bLine, string prizeLine)
	{
		_buttonA = GetValues(aLine);
		_buttonB = GetValues(bLine);
		_prize = GetValues(prizeLine);
	}

	#endregion

	#region Methods

	/// <summary>
	/// For the given <paramref name="input"/>, extract the X and Y coordinates
	/// </summary>
	/// <param name="input">The value to parse</param>
	/// <returns>The X and Y coordinates</returns>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	private (int x, int y) GetValues(string input)
	{
		//	No results if blank
		ArgumentException.ThrowIfNullOrWhiteSpace(input);

		//	Run a match using the Regex
		var match = _valueSplitter.Match(input);

		//	If we succeed in finding a match and we have 3 groups (all, capture 1 and capture 2) we're good, otherwise this input is invalid
		if (!match.Success || match.Groups.Count != 3)
			throw new ArgumentException("Input does not validate", nameof(input));

		//	Check if the first value can be parsed to an integer
		if (!int.TryParse(match.Groups[1].Value, out var x))
			throw new ArgumentOutOfRangeException(nameof(input), $"Cannot parse '{match.Groups[1].Value}'.");

		//	Check if the second value can be parsed to an integer
		if (!int.TryParse(match.Groups[2].Value, out var y))
			throw new ArgumentOutOfRangeException(nameof(input), $"Cannot parse '{match.Groups[2].Value}'.");

		//	Values must be positive
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(x, nameof(input));
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(y, nameof(input));

		return (x, y);
	}

	/// <summary>
	/// Solves the linear equations to find the values for A and B (and also updates <see cref="IsSolveable"/> at the same time)
	/// </summary>
	/// <returns>True if the puzzle can be solved (i.e. you can reach the prize by repeated button presses), otherwise false</returns>
	public bool Solve()
	{
		var result = false;

		//	_buttonA, _buttonB and _prize used as part of the linear equations:
		//	(_buttonA.x)A + (_buttonB.x)B = _prize.x
		//	(_buttonA.y)A + (_buttonB.y)B = _prize.y
		//	using the first test example, this "simplifies" to:
		//
		//	94A + 22B = 8400
		//	34A + 67B = 5400
		//
		//	Normalising, we then have:
		//	67(94A + 22B) = 67 * 8400 and
		//	22(34A + 67B) = 22 * 5400
		(long a, long b, long c) normalise1 = (_buttonB.y * _buttonA.x, _buttonB.y * _buttonB.x, _buttonB.y * _prize.x);
		(long a, long b, long c) normalise2 = (_buttonB.x * _buttonA.y, _buttonB.x * _buttonB.y, _buttonB.x * _prize.y);

		//	This should result in the following:
		//	6298A + 1474B = 562800 and
		//	 748A + 1474B = 118800
		//	The above should result in the negation of the B term, so we can do a quick assertion to make sure they are equal before we discard
		Debug.Assert(normalise1.b == normalise2.b);

		(long n, long v) valueForA = (normalise1.a - normalise2.a, normalise1.c - normalise2.c);

		//	Now we reverse the calculation to find the value for b:
		//	Normalising for B this time, we get:
		//	34(94A + 22B) = 34 * 8400 and
		//	94(34A + 67B) = 94 * 5400
		normalise1 = (_buttonA.y * _buttonA.x, _buttonA.y * _buttonB.x, _buttonA.y * _prize.x);
		normalise2 = (_buttonA.x * _buttonA.y, _buttonA.x * _buttonB.y, _buttonA.x * _prize.y);

		//	This should result in the following:
		//	3196A +  748B = 285600 and
		//	3196A + 6298B = 507600
		//	The above should result in the negation of the B term, so we can do a quick assertion to make sure they are equal before we discard
		Debug.Assert(normalise1.a == normalise2.a);

		//	Get the value for B
		(long n, long v) valueForB = (normalise2.b - normalise1.b, normalise2.c - normalise1.c);

		//	Set the values for A and B properties
		//	If the values turned negative above, this should correct it
		ValueForA = valueForA.v / valueForA.n;
		ValueForB = valueForB.v / valueForB.n;

		normalise1 = (_buttonA.x * ValueForA, _buttonB.x * ValueForB, _prize.x);
		normalise2 = (_buttonA.y * ValueForA, _buttonB.y * ValueForB, _prize.y);

		result = (normalise1.a + normalise1.b == normalise1.c) &&
				(normalise2.a + normalise2.b == normalise2.c);
		return IsSolveable = result;
	}

	/// <summary>
	/// Yields the cost in tokens for solving the puzzle. If not possible to solve, the result will be zero
	/// </summary>
	/// <param name="tokensForA">How many tokens a move for A costs</param>
	/// <param name="tokensForB">How many tokens a move for B costs</param>
	/// <returns></returns>
	public long GetCost(int tokensForA, int tokensForB)
	{
		return IsSolveable
			? (tokensForA * ValueForA) + (tokensForB * ValueForB)
			: 0;
	}

	/// <summary>
	/// Adds the offset of 10000000000000 to the prize locations
	/// </summary>
	public void CorrectPrizeLocation()
	{
		_prize.x += 10000000000000;
		_prize.y += 10000000000000;
	}
	#endregion
}