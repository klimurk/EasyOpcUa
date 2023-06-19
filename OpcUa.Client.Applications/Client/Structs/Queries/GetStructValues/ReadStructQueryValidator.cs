using FluentValidation;

namespace OpcUa.Client.Applications.Client.Structs.Queries.GetStructValues;

public class ReadStructQueryValidator : AbstractValidator<ReadStructQuery>
{
    public ReadStructQueryValidator()
    {
        RuleFor(query => query.client).NotNull().Must(s => !s.Session.Disposed);
        RuleFor(query => query.nodeId).NotEmpty();

    }
}