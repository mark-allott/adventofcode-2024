using System.Reflection;
using AdventOfCode.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AdventOfCode.Configuration;

public static class AutoRegisterExtensions
{
	public static IServiceCollection AddAutoRegistrationDailyChallenges(this IServiceCollection services, params Assembly[] assemblies)
	{
		if (assemblies.Length == 0)
			throw new ArgumentException("No assemblies selected", nameof(assemblies));

		//	Define the interface(s) to detect for auto-registration
		var interfaceTypes = new[] { typeof(IAutoRegister) };

		// Locate challenges with class inheritance of the interfaceTypes which are not abstract classes,
		// then locate all classes whose interfaces that are not in the interfaceTypes list and present
		// for auto-registration
		var autoRegistrations = assemblies
			.SelectMany(s => s.ExportedTypes)
			.Where(q =>	q.IsClass &&
						!q.IsAbstract &&
						q.BaseType is not null)
			.Select(s => new
			{
				ExportedType = s,
				ImplementedTypes = s.GetInterfaces()
					.Where(q => interfaceTypes.Contains(q))
					.ToList()
			})
			.Where(q => q.ImplementedTypes.Count > 0)
			.Select(s => new
			{
				s.ExportedType,
				Interfaces = s.ExportedType.GetInterfaces()
					.Where(q => !interfaceTypes.Contains(q))
					.ToList()
			})
			.Where(q => q.Interfaces.Count > 0)
			.ToList();

		// For the list of auto-registrations found, add each class as a transient service
		autoRegistrations.ForEach(c =>
		{
			services.AddTransient(c.ExportedType);
			c.Interfaces.ForEach(i =>
			{
				services.AddTransient(i, c.ExportedType);
			});
		});
		return services;
	}
}