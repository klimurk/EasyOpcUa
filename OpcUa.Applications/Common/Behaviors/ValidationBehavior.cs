using FluentResults;
using FluentValidation;
using MediatR;
using OpcUa.Domain.Errors;

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
            result.Reasons.AddRange(validationResult.Reasons);

            return result;
        }
        return await next();

    }

    private Task<Result> ValidateAsync(TRequest request)
    {
        var context = new ValidationContext<TRequest>(request);
        var fails = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(fail => fail is not null)
            .ToDictionary(s => s.PropertyName,
                s => s.AttemptedValue,
                StringComparer.OrdinalIgnoreCase);

        return fails.Count != 0 ?
            Task.FromResult(Result.Fail(DomainErrors.Validation.ValidationFailedError.WithMetadata(fails))) :
            Task.FromResult(Result.Ok());
    }
}
