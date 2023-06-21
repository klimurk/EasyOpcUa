using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace OpcUa.Applications.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase, new()
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        //Request
        Type myType = request.GetType();
        IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
        Dictionary<string, object> propsDictionary = new Dictionary<string, object>();
        foreach (PropertyInfo prop in props)
        {
            object propValue = prop.GetValue(request, null);
            propsDictionary.Add(prop.Name, propValue);
            //_logger.LogInformation("{Property} : {@Value}", prop.Name, propValue);
        }
        _logger.LogInformation("Starting request {@RequestName} with properties {@props}", typeof(TRequest).Name, propsDictionary);

        TResponse? response = await next();

        if (response.IsFailed)
        {
            response.Errors.ForEach(s => _logger.LogWarning(s.Message));
        }
        else
        {
            response.Successes.ForEach(s => _logger.LogInformation(s.Message));
        }
        return response;
    }
}