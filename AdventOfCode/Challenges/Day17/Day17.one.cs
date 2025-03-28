using System.Diagnostics;
using System.Text.RegularExpressions;
using AdventOfCode.Interfaces;
using AdventOfCode.Models;

namespace AdventOfCode.Challenges.Day17;

public partial class Day17
	: AbstractDailyChallenge, IAutoRegister, IPartOneTestable
{
	#region Overrides to run part one of challenge

	/// <summary>
	/// Override the base implementation to provide the actual answer
	/// </summary>
	/// <returns>True if successful</returns>
	protected override bool PartOne()
	{
		LoadAndReadFile();
		var (a, b, c, program) = ExtractSetup();

		var computer = new ChronospatialComputer();
		computer.SetRegisters(a, b, c);
		computer.LoadProgram(program);
		computer.Run();
		string output = computer.Output;
		PartOneResult = $"{ChallengeTitle} outputs = {output}";
		return true;
	}

	#endregion

	#region IPartOneTestable implementation

	/// <summary>
	/// Using rules from the challenge, run a test to ensure the code matches the rules
	/// </summary>
	public void PartOneTest()
	{
		/*
		Here are some examples of instruction operation:

			If register C contains 9, the program 2,6 would set register B to 1.
			If register A contains 10, the program 5,0,5,1,5,4 would output 0,1,2.
			If register A contains 2024, the program 0,1,5,4,3,0 would output 4,2,5,6,7,7,7,7,3,1,0 and leave 0 in register A.
			If register B contains 29, the program 1,7 would set register B to 26.
			If register B contains 2024 and register C contains 43690, the program 4,0 would set register B to 44354.
		*/
		var computer = new ChronospatialComputer();
		computer.SetRegisters(0, 0, 9);
		computer.ExecuteOpcode(2, 6);
		Debug.Assert(computer.RegisterB == 1);

		computer.SetRegisters(10, 0, 0);
		computer.LoadProgram("5,0,5,1,5,4");
		computer.Run();
		Debug.Assert(computer.Output == "0,1,2");

		computer.SetRegisters(2024, 0, 0);
		computer.LoadProgram("0,1,5,4,3,0");
		computer.Run();
		Debug.Assert(computer.Output == "4,2,5,6,7,7,7,7,3,1,0");
		Debug.Assert(computer.RegisterA == 0);

		computer.SetRegisters(0, 29, 0);
		computer.ExecuteOpcode(1, 7);
		Debug.Assert(computer.RegisterB == 26);

		computer.SetRegisters(0, 2024, 43690);
		computer.ExecuteOpcode(4, 0);
		Debug.Assert(computer.RegisterB == 44354);

		computer.SetRegisters(729, 0, 0);
		computer.LoadProgram("0,1,5,4,3,0");
		computer.Run();
		Debug.Assert(computer.Output == "4,6,3,5,6,3,5,2,1,0");
	}

	#endregion

	#region Methods

	private (long a, long b, long c, string program) ExtractSetup()
	{
		long a = 0;
		long b = 0;
		long c = 0;
		string program = string.Empty;

		foreach (var line in InputFileLines)
		{
			if (line.Contains("Register"))
			{
				var digit = Regex.Replace(line, @"\D", " ").Trim();
				if (int.TryParse(digit, out var value))
				{
					if (line.Contains("A:"))
						a = value;
					else if (line.Contains("B:"))
						b = value;
					else if (line.Contains("C:"))
						c = value;
				}
			}
			else if (line.Contains("Program"))
			{
				var digits = Regex.Replace(line, @"\D", " ").Trim();
				program = digits.Replace(' ', ',');
			}
		}

		return (a, b, c, program);
	}

	#endregion
}