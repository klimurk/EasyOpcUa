using Microsoft.Extensions.DependencyInjection;

namespace OpcUa.Client.Applications;

public static class DependencyInjection
{
	public static IServiceCollection AddClientApplication(this IServiceCollection services)
	{
		//services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
		//services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() });
		//services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
		//services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
		return services;
	}
}
