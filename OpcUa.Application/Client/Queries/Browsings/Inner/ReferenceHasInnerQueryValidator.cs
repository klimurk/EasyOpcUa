using FluentValidation;

namespace OpcUa.Application.Client.Queries.Browsings.Inner;

public class ReferenceHasInnerQueryValidator : AbstractValidator<ReferenceHasInnerQuery>
{
	public ReferenceHasInnerQueryValidator()
	{
		RuleFor(query =>
			query.Client).NotNull().Must(s=>!s.Session.Disposed);
		RuleFor(query => query.node).NotNull().Must(s => s.NodeId is not null);
	}
}


