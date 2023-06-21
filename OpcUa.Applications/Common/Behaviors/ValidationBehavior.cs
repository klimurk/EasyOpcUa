using FluentResults;
using FluentValidation;
using MediatR;

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
