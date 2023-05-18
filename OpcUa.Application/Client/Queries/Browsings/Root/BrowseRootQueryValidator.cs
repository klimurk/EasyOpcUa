using FluentValidation;

namespace OpcUa.Application.Client.Queries.Browsings.Root;

public class BrowseRootQueryValidator : AbstractValidator<BrowseRootQuery>
{
	public BrowseRootQueryValidator()
	{
		RuleFor(query =>
			query.Client).NotNull().Must(s => !s.Session.Disposed);
	}
}


