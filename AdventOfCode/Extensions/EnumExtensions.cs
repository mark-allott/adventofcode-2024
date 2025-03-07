using AdventOfCode.Enums;

namespace AdventOfCode.Extensions;

internal static class EnumExtensions
{
	/// <summary>
	/// Converts the character representation of the direction of travel to a <see cref="DirectionOfTravel"/>
	/// </summary>
	/// <param name="c">The character to transpose</param>
	/// <returns>The direction of travel</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static DirectionOfTravel ToDirectionOfTravel(this char c)
	{
		return c switch
		{
			'^' => DirectionOfTravel.North,
			'>' => DirectionOfTravel.East,
			'v' => DirectionOfTravel.South,
			'<' => DirectionOfTravel.West,
			_ => throw new ArgumentOutOfRangeException(nameof(c))
		};
	}
}