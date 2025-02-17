namespace AdventOfCode.Models;

internal class Operation
{
	public string Instruction { get; private set; } = string.Empty;
	public List<string> Parameters { get; private set; } = new List<string>();

	public Operation(string instruction, string parameters)
	{
		ArgumentNullException.ThrowIfNullOrWhiteSpace(instruction);

		Instruction = instruction.ToLowerInvariant();
		if (!string.IsNullOrWhiteSpace(parameters))
			Parameters.AddRange(parameters.Split([',', ' '], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
	}

	public override string ToString()
	{
		return $"{Instruction}({string.Join(',', Parameters)})";
	}
}