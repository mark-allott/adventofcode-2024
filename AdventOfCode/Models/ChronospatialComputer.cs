using System.Text.RegularExpressions;
using AdventOfCode.Enums;

namespace AdventOfCode.Models;

internal class ChronospatialComputer
{
	#region Fields

	/// <summary>
	/// Holds the output values from the computer
	/// </summary>
	private readonly List<int> _outputs = new List<int>();

	/// <summary>
	/// Holds the program to be executed
	/// </summary>
	private readonly List<int> _program = new List<int>();

	/// <summary>
	/// Holds the instruction pointer for the program execution
	/// </summary>
	private int _instructionPointer = 0;

	#endregion

	#region Properties

	/// <summary>
	/// Holds the value of the A register
	/// </summary>
	public long RegisterA { get; private set; }

	/// <summary>
	/// Holds the value of the B register
	/// </summary>
	public long RegisterB { get; private set; }

	/// <summary>
	/// Holds the value of the C register
	/// </summary>
	public long RegisterC { get; private set; }

	/// <summary>
	/// Converts the output integers into a comma separated string
	/// </summary>
	public string Output => string.Join(",", _outputs);

	#endregion

	#region Constructors

	public ChronospatialComputer()
	{
	}

	#endregion

	#region Methods

	/// <summary>
	/// Runs the program loaded into the computer
	/// </summary>
	public void Run()
	{
		_instructionPointer = 0;
		_outputs.Clear();

		try
		{
			while (_instructionPointer < _program.Count)
			{
				var opcode = (ChronospatialComputerOpcode)_program[_instructionPointer++];
				var operand = _program[_instructionPointer++];
				ExecuteOpcode(opcode, operand);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			_outputs.Clear();
		}
	}

	public void ClearOutput()
	{
		_outputs.Clear();
	}

	/// <summary>
	/// Sets the register values for the computer
	/// </summary>
	/// <param name="aValue">The value for A</param>
	/// <param name="bValue">The value for B</param>
	/// <param name="cValue">The value for C</param>
	public void SetRegisters(long aValue = 0, long bValue = 0, long cValue = 0)
	{
		RegisterA = aValue;
		RegisterB = bValue;
		RegisterC = cValue;
	}

	/// <summary>
	/// Loads the program in <paramref name="text"/>
	/// </summary>
	/// <param name="text">The program to be loaded</param>
	public void LoadProgram(string text)
	{
		_program.Clear();

		//	replace all non-digit with spaces
		var digits = Regex.Replace(text, @"\D", " ").Trim();
		foreach (var digit in digits.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
		{
			//	Provided the digit does parse to an int, add to program
			if (int.TryParse(digit, out var value))
				_program.Add(value);
		}

		//	Program must contain an even number of digits (opcode + operand)
		if (_program.Count % 2 != 0)
		{
			_program.Clear();
			throw new ArgumentException("Invalid program due to odd number of inputs");
		}

		//	Any value not fitting 3-bits cannot work
		if (_program.Any(a => a > 7))
			throw new ArgumentOutOfRangeException(nameof(text), $"Program contains an invalid opcode or operand");
	}

	/// <summary>
	/// Debug helper - executes the <paramref name="opcode"/> with <paramref name="operand"/> in the computer, setting state appropriately
	/// </summary>
	/// <param name="opcode">The opcode to run</param>
	/// <param name="operand">The value of the operand</param>
	public void ExecuteOpcode(int opcode, int operand)
	{
		ExecuteOpcode((ChronospatialComputerOpcode)opcode, operand);
	}

	/// <summary>
	/// Executes the opcode in the computer, updating state as needed
	/// </summary>
	/// <param name="opcode">The instruction to execute</param>
	/// <param name="operand">The operand for the instruction</param>
	private void ExecuteOpcode(ChronospatialComputerOpcode opcode, int operand)
	{
		long denominator;
		switch (opcode)
		{
			//	Division => A = A/(2^combo)
			case ChronospatialComputerOpcode.adv:
				denominator = (long)Math.Pow(2, GetComboOperand(operand));
				RegisterA = RegisterA / denominator;
				break;

			//	XOR => B = B XOR operand
			case ChronospatialComputerOpcode.bxl:
				RegisterB ^= operand;
				break;

			//	B = combo MOD 8
			case ChronospatialComputerOpcode.bst:
				RegisterB = GetComboOperand(operand) % 8;
				break;

			//	Jump non-zero => A =/= 0, set IP to operand
			case ChronospatialComputerOpcode.jnz:
				if (RegisterA != 0)
					_instructionPointer = operand;
				break;

			//	XOR B/C => B = B XOR C
			case ChronospatialComputerOpcode.bxc:
				RegisterB ^= RegisterC;
				break;

			//	output => add (combo MOD 8) to output
			case ChronospatialComputerOpcode.@out:
				_outputs.Add((int)(GetComboOperand(operand) % 8));
				break;

			//	Division => B = A/(2^combo)
			case ChronospatialComputerOpcode.bdv:
				denominator = (long)Math.Pow(2, GetComboOperand(operand));
				RegisterB = RegisterA / denominator;
				break;

			//	Division => C = A/(2^combo)
			case ChronospatialComputerOpcode.cdv:
				denominator = (long)Math.Pow(2, GetComboOperand(operand));
				RegisterC = RegisterA / denominator;
				break;
		}
	}

	/// <summary>
	/// Determines what the "combo operand" value is from <paramref name="operand"/>
	/// </summary>
	/// <param name="operand">The input operand value</param>
	/// <returns>The value to be used as the operand for the opcode</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	private long GetComboOperand(int operand)
	{
		return operand switch
		{
			< 4 => operand,
			4 => RegisterA,
			5 => RegisterB,
			6 => RegisterC,
			_ => throw new ArgumentOutOfRangeException(nameof(operand), operand, "Invalid instruction"),
		};
	}
	#endregion
}