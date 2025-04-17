namespace AdventOfCode.Extensions;

public static class StringExtensions
{
	/// <summary>
	/// Parse the input into a list of integer values. Values can be separated by spaces or commas
	/// </summary>
	/// <param name="line">The string to parse</param>
	/// <param name="rowNumber">(Optional) specifies the row number in a file</param>
	/// <returns>The list of integer parts</returns>
	/// <exception cref="ArgumentException"></exception>
	public static List<int> ParseStringToListOfInt(this string line, int rowNumber = 0)
	{
		var parts = line.Split([' ', ','], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		return parts.ParseEnumerableOfStringToListOfInt(rowNumber);
	}

	/// <summary>
	/// Parse the input into a list of integer values. Values can be separated by spaces or commas
	/// </summary>
	/// <param name="input">The list of strings to parse</param>
	/// <param name="rowNumber">(Optional) specifies the row number in a file</param>
	/// <returns>The list of integer parts</returns>
	/// <exception cref="ArgumentException"></exception>
	public static List<int> ParseEnumerableOfStringToListOfInt(this IEnumerable<string> input, int rowNumber = 0)
	{
		var counter = 1;
		var rowValues = new List<int>();
		foreach (var part in input)
		{
			if (int.TryParse(part, out var v))
			{
				counter++;
				rowValues.Add(v);
			}
			else
			{
				var message = $"Data error with line {(rowNumber != 0 ? $"{rowNumber}" : $"{string.Join(',', input)}")} and part #{counter} => '{part}'";
				Console.WriteLine(message);
				throw new ArgumentException(message);
			}
		}
		return rowValues;
	}

	/// <summary>
	/// Iterate over the <paramref name="input"/>, splitting into sections identified by empty lines
	/// </summary>
	/// <param name="input">The strings to be parsed</param>
	/// <returns>The <paramref name="input"/>, split into sections</returns>
	public static List<List<string>> ParseEnumerableOfStringToListOfListOfString(this IEnumerable<string> input)
	{
		var result = new List<List<string>>();
		var section = new List<string>();

		foreach (var line in input)
		{
			if (string.IsNullOrWhiteSpace(line))
			{
				if (section.Count > 0)
				{
					result.Add(section);
					section = new List<string>();
				}
			}
			else
				section.Add(line);
		}
		if (section.Count > 0)
			result.Add(section);

		return result;
	}
}