namespace AdventOfCode.Interfaces;

internal interface ICellRenderer<T>
{
	char ToCharacter(T value);
}