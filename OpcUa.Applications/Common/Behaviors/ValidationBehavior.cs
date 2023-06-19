using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace OpcUa.Applications.Common.Behaviors;


public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
	where TResponse : ResultBase, new()
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;
	public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{

		var validationResult = await ValidateAsync(request);
		if (validationResult.IsFailed)
		{
			var result = new TResponse();

			foreach (var reason in validationResult.Reasons)
				result.Reasons.Add(reason);

			return result;
		}
		return await next();
	}

	private Task<Result> ValidateAsync(TRequest request)
	{
		var context = new ValidationContext<TRequest>(request);
		var failers = _validators.Select(v => v.Validate(context)).SelectMany(r => r.Errors).Where(fail => fail is not null).ToList();

		return failers.Count != 0 ?
			Task.FromResult(Result.Fail("Validation failed")) :
			Task.FromResult(Result.Ok());
	}
}
public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
	where TResponse : Result, new()
{
	private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
	public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger) => _logger = logger;

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		//Request
		_logger.LogInformation("Starting request {@RequestName}", typeof(TRequest).Name);
		//Type myType = request.GetType();
		//IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
		//foreach (PropertyInfo prop in props)
		//{
		//	object propValue = prop.GetValue(request, null);
		//	_logger.LogInformation("{Property} : {@Value}", prop.Name, propValue);
		//}
		TResponse? response = await next();

		if (response.IsFailed)
		{
			response.Errors.ForEach(s => response.Log(LogLevel.Warning));
		}
		else
		{
			response.Log(LogLevel.Information);
		}
		return response;
	}
}
