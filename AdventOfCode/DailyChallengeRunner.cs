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
		var challenges = GetDailyChallenges();

		//	TODO: implement different options

		//	Execute the challenge with the greatest day number value
		RunChallenge(challenges.Last());

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

	private List<IDailyChallenge> GetDailyChallenges()
	{
		//	Assuming the services provider is valid, obtain a list of daily challenges ordered by day number
		return ServiceProvider.GetServices<IDailyChallenge>()
			.OrderBy(o => o.DayNumber)
			.ToList();
	}

	private void RunChallenge(IDailyChallenge challenge)
	{
		ArgumentNullException.ThrowIfNull(challenge, nameof(challenge));
		Console.WriteLine($"Running challenge for day {challenge.DayNumber}");
		var result = challenge.Execute();
		Console.WriteLine($"Challenge completed {(result ? "" : "un")}successfully");
		Console.WriteLine($"Part One solution: {challenge.PartOneResult}");
		Console.WriteLine($"Part Two solution: {challenge.PartTwoResult}");
		Console.WriteLine("{0}{1}{2}{1}{0}", new String('=', 5), new String(' ', 5), new String('-', 60));
	}
}