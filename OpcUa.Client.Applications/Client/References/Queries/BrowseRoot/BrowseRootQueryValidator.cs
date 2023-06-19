using FluentValidation;

namespace OpcUa.Client.Applications.Client.References.Queries.BrowseRoot;

public class BrowseRootQueryValidator : AbstractValidator<BrowseRootQuery>
{
    public BrowseRootQueryValidator()
    {
        RuleFor(query =>
            query.Client).NotNull().Must(s => !s.Session.Disposed);
    }
}


