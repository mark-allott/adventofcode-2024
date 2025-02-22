using System;
using AdventOfCode.Challenges;
using AdventOfCode.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AdventOfCode;

public class DailyChallengeRunner
	: IHostedService
{
	private IConfiguration Configuration { get; }
	private IServiceProvider ServiceProvider { get; }

	#region IHostedService implementation

	public Task StartAsync(CancellationToken cancellationToken)
	{
		//	Try to find out what needs to be run
		var x = Configuration["run"];
		var challenges = GetDailyChallenges();

		//	Check: are there any challenges to be run?
		if (challenges.Count == 0)
		{
			Console.WriteLine("No challenges to run!");
			return Task.CompletedTask;
		}

		//	run using the different options
		if (string.IsNullOrWhiteSpace(x))
		{
			//	Nothing specific is supplied, so execute the challenge with the greatest day number value
			RunChallenge(challenges.Last());
		}
		else if (x.Equals("all", StringComparison.OrdinalIgnoreCase))
		{
			//	All challenges are requested, so run them in DayNumber order
			challenges.ForEach(c => RunChallenge(c));
		}
		else
		{
			//	If here then we assume a list of days is present, so we need to
			//	convert to integers to find out what these days are
			var days = x.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => int.TryParse(s, out var result) ? result : int.MinValue)
				.Where(q => q != int.MinValue)
				.OrderBy(o => o)
				.ToList();

			//	Filter the challenges to be run that match any specified days
			var challengesToRun = challenges.Where(c => days.Contains(c.DayNumber))
				.ToList();

			//	Run the requested daily challenges
			challengesToRun.ForEach(c => RunChallenge(c));
		}

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	#endregion

	#region ctor

	public DailyChallengeRunner(IConfiguration configuration, IServiceProvider serviceProvider)
	{
		//	Verify the inputs to make certain they are valid
		Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
	}

	#endregion

	/// <summary>
	/// Provide a list of the challenges that the application contains in ascending <see cref="IDailyChallenge.DayNumber"/> order
	/// </summary>
	/// <returns>All challenges registered in <see cref="ServiceProvider"/></returns>
	private List<IDailyChallenge> GetDailyChallenges()
	{
		//	Assuming the services provider is valid, obtain a list of daily challenges ordered by day number
		return ServiceProvider.GetServices<IDailyChallenge>()
			.OrderBy(o => o.DayNumber)
			.ToList();
	}

	/// <summary>
	/// Executes the specified challenge, sending output to the console
	/// </summary>
	/// <param name="challenge">The challenge being executed</param>
	/// <exception cref="ArgumentNullException"></exception>
	private static void RunChallenge(IDailyChallenge challenge)
	{
		//	No challenge?? Throw an exception
		ArgumentNullException.ThrowIfNull(challenge, nameof(challenge));

		//	Write some output to the console stating which challenge is being run
		Console.WriteLine($"Running challenge for day {challenge.DayNumber}");

		//	Run the challenge, state whether it was completed successfully
		//	(both part one and part two must be completed successfully to be
		//	stated as completely successful) and display the results
		var result = challenge.Execute();
		Console.WriteLine($"Challenge completed {(result ? "" : "un")}successfully");
		Console.WriteLine($"Part One solution: {challenge.PartOneResult}");
		Console.WriteLine($"Part Two solution: {challenge.PartTwoResult}");

		//	Print a separator line to distinguish between the end of this
		//	challenge and any challenges following it
		Console.WriteLine("{0}{1}{2}{1}{0}", new String('=', 5), new String(' ', 5), new String('-', 60));
	}
}