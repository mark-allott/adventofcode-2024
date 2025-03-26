using AdventOfCode.Enums;

namespace AdventOfCode.Interfaces;

internal interface IDirectional
{
	/// <summary>
	/// Describes the direction of travel for an object
	/// </summary>
	DirectionOfTravel Direction { get; }
}