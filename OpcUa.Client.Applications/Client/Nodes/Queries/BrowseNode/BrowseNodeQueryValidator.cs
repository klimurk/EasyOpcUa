using FluentValidation;

namespace OpcUa.Client.Applications.Client.Nodes.Queries.BrowseNode;

public class BrowseNodeQueryValidator : AbstractValidator<BrowseNodeQuery>
{
    public BrowseNodeQueryValidator()
    {
        RuleFor(query =>
            query.Client).NotNull().Must(s => !s.Session.Disposed);
        RuleFor(query => query.node).NotNull().Must(s => s.NodeId is not null);
    }
}
