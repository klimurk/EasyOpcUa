using MediatR.NotificationPublishers;
using Microsoft.Extensions.DependencyInjection;
using OpcUa.Applications.Common.Behaviors;
using FluentValidation;

namespace OpcUa.Persistance;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        //services.AddTransient<IOpcConnector, OpcConnector>();
        //services.AddTransient<IOpcInitializer, OpcInitializer>();
        //services.AddTransient<IOpcReader, OpcReader>();
        //services.AddTransient<IOpcSubscriber, OpcSubscriber>();
        //services.AddTransient<IOpcWriter, OpcWriter>();
        //services.AddSingleton<IOpcStore, OpcStore>();
        services.AddValidatorsFromAssemblies(new[] { typeof(Client.Applications.DependencyInjection).Assembly });
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(Client.Applications.DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.NotificationPublisher = new ForeachAwaitPublisher();
        });
        services.AddSingleton<OpcControllerService>();
        return services;
    }
}
