namespace AdventOfCode.Interfaces;

public interface ITestable
	: IPartOneTestable, IPartTwoTestable
{
}

public interface IResettableTest
	: ITestable, IResettable
{
}