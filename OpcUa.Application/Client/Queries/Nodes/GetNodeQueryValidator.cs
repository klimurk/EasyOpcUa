using FluentValidation;

namespace OpcUa.Application.Client.Queries.Nodes;

public class GetNodeQueryValidator : AbstractValidator<GetNodeQuery>
{
	public GetNodeQueryValidator()
	{
		RuleFor(query =>
			query.client).NotNull().Must(s => !s.Session.Disposed);
		RuleFor(query =>
			query.client).NotEmpty();
	}
}