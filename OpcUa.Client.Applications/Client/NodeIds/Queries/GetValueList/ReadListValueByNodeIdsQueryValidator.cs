using FluentValidation;
using Opc.Ua;

namespace OpcUa.Client.Applications.Client.NodeIds.Queries.GetValueList;

public class ReadListValueByNodeIdsQueryValidator : AbstractValidator<ReadListValueByNodeIdsQuery>
{
    public ReadListValueByNodeIdsQueryValidator()
    {
        RuleFor(query => query.Client).NotNull().Must(s => !s.Session.Disposed);
        RuleFor(query => query.NodeIds).NotEmpty();
        RuleForEach(query => query.NodeIds).NotEmpty();
        RuleFor(query => query.ExpectedType).Must(s => Enum.IsDefined(typeof(BuiltInType), s));
    }
}
