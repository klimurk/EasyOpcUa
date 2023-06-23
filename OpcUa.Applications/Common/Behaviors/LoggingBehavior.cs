using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text;

namespace OpcUa.Applications.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase, new()
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        LogRequest(request);

        TResponse? response = await next();

        LogResponse(response);

        return response;
    }

    private void LogResponse(TResponse response)
    {
        StringBuilder sb = new();
        if (response.IsFailed)
        {

            response.Errors.ForEach(s => sb.AppendLine(s.Message));
            _logger.LogWarning(sb.ToString());
        }
        else
        {
            response.Successes.ForEach(s => sb.AppendLine(s.Message));
            _logger.LogInformation(sb.ToString());
        }
    }

    private void LogRequest(TRequest request)
    {
        Type myType = request.GetType();
        StringBuilder sb = new($"Starting request {typeof(TRequest).Name} with properties:");
        IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
        foreach (PropertyInfo prop in props)
        {
            sb.AppendLine($" {prop.Name} - {prop.GetValue(request, null)}");
        }
        _logger.LogInformation(sb.ToString());
    }
}
public class ErrorReasonsMetadataBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase, new()
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse? response = await next();

        if (response.IsSuccess) return response;
        Type myType = request.GetType();
        IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
        foreach (var error in response.Errors)
        {
            foreach (PropertyInfo prop in props)
            {
                error.Metadata.TryAdd(prop.Name, prop.GetValue(request, index: null));
            }
        }
        return response;
    }
}