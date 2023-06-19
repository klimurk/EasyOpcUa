using FluentValidation;

namespace OpcUa.Client.Applications.Client.References.Queries.BrowseReferences;

public class BrowseReferenceQueryValidator : AbstractValidator<BrowseReferenceQuery>
{
    public BrowseReferenceQueryValidator()
    {
        RuleFor(query =>
            query.Client).NotNull().Must(s => !s.Session.Disposed);
        RuleFor(query => query.RefDesc).NotNull().Must(s => s.NodeId is not null);
    }
}
