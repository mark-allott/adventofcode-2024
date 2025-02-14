using AdventOfCode.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AdventOfCode;

internal class Program
{
	private static void Main(string[] args)
	{
		var host = GetHostBuilder(args)
			.Build();
		host.RunAsync();
	}

	private static IHostBuilder GetHostBuilder(string[] args)
	{
		var hostBuilder = Host.CreateDefaultBuilder(args);

		//	Add services to the application
		hostBuilder.ConfigureServices(services =>
		{
			//	Add the hosted service to run the challenge(s)
			services.AddHostedService<DailyChallengeRunner>();

			//	Auto-register the daily challenges that can do so
			services.AddAutoRegistrationDailyChallenges(typeof(Program).Assembly);
		});
		return hostBuilder;
	}
}
