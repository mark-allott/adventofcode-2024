using System.Text;
using AdventOfCode.Enums;

namespace AdventOfCode.Models;

internal class CalibrationProblem
{
	private long? _a;
	private long _b;
	private CalibrationOperator _operator = CalibrationOperator.Unknown;
	private CalibrationProblem _operation = null!;

	/// <summary>
	/// ctor - uses two values and an operator
	/// </summary>
	/// <param name="a">The first value</param>
	/// <param name="b">The second value</param>
	/// <param name="calibrationOperator">The operator that applies to the problem</param>
	public CalibrationProblem(long a, long b, CalibrationOperator calibrationOperator)
	{
		_a = a;
		_b = b;
		_operator = calibrationOperator;
	}

	/// <summary>
	/// ctor - uses another <see cref="CalibrationProblem"/> for part a and a second value plus operator
	/// </summary>
	/// <param name="a">An existing <see cref="CalibrationProblem"/> (from which the frist value can be determined)</param>
	/// <param name="b">The second value</param>
	/// <param name="calibrationOperator">The operator to apply to the problem</param>
	/// <exception cref="ArgumentNullException"></exception>
	public CalibrationProblem(CalibrationProblem a, long b, CalibrationOperator calibrationOperator)
	{
		_operation = a ?? throw new ArgumentNullException(nameof(a));
		_b = b;
		_operator = calibrationOperator;
	}

	/// <summary>
	/// Property accessor that yields the value of this part of the problem space
	/// </summary>
	public long Value => GetValue();

	/// <summary>
	/// Performs the required calculation(s) to determine the value to be returned
	/// </summary>
	/// <returns>The value for the problem</returns>
	private long GetValue()
	{
		var a = GetValueForA();
		return _operator switch
		{
			CalibrationOperator.Add => a + _b,
			CalibrationOperator.Multiply => a * _b,
			CalibrationOperator.Concatenate => long.Parse($"{a}{_b}"),
			_ => 0
		};
	}

	/// <summary>
	/// Obtains the value for the "a" part of the problem
	/// </summary>
	/// <returns>The calculated value for the "a" part</returns>
	private long GetValueForA()
	{
		return _a ?? _operation?.Value ?? 0;
	}

	/// <summary>
	/// Provides a nice way of printing the problem
	/// </summary>
	/// <returns>The textual representation of the problem</returns>
	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.Append(_a.HasValue ? _a.Value : _operation?.ToString() ?? "")
			.Append(' ')
			.Append(_operator switch
			{
				CalibrationOperator.Add => '+',
				CalibrationOperator.Multiply => '*',
				CalibrationOperator.Subtract => '-',
				CalibrationOperator.Divide => '/',
				CalibrationOperator.Concatenate => "||",
				_ => '?'
			})
			.Append(' ')
			.Append(_b);
		return sb.ToString();
	}
}