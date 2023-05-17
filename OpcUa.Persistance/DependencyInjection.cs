using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpcUa.Application.Contratcs.Client;
using Woodnailer.Application.Opc.Client;

namespace OpcUa.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddOpcClientServices(this IServiceCollection services)
	{
		services.AddTransient<IOpcConnector, OpcConnector>();
		services.AddTransient<IOpcInitializer, OpcInitializer>();
		services.AddTransient<IOpcReader, OpcReader>();
		services.AddTransient<IOpcSubscriber, OpcSubscriber>();
		services.AddTransient<IOpcWriter, OpcWriter>();
		services.AddSingleton<IOpcStore, OpcStore>();
		return services;
	}
}
