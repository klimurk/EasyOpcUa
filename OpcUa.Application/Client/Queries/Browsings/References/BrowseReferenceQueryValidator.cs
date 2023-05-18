using FluentValidation;

namespace OpcUa.Application.Client.Queries.Browsings.References;

public class BrowseReferenceQueryValidator : AbstractValidator<BrowseReferenceQuery>
{
	public BrowseReferenceQueryValidator()
	{
		RuleFor(query =>
			query.Client).NotNull().Must(s => !s.Session.Disposed);
		RuleFor(query => query.RefDesc).NotNull().Must(s => s.NodeId is not null);
	}
}
