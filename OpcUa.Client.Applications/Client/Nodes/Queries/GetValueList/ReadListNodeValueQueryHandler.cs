using FluentResults;
using FluentValidation;
using MediatR;
using Opc.Ua;
using OpcUa.Domain.Errors;
using OpcUa.Client.Applications.Client.NodeIds.Queries.GetValueList;

namespace OpcUa.Client.Applications.Client.Nodes.Queries.GetValueList;

public class ReadListNodeValueQueryHandler : IRequestHandler<ReadListNodeValueQuery, Result<DataValueCollection>>
{
    public async Task<Result<DataValueCollection>> Handle(ReadListNodeValueQuery request, CancellationToken cancellationToken)
    {
        List<NodeId> nodeIds = request.nodes.Select(s => s.NodeId).ToList();
        DataValueCollection values = new();
        IList<ServiceResult> serviceResults = new List<ServiceResult>();
        List<Type> types = Enumerable.Repeat(request.ExpectedType, nodeIds.Count).ToList();
        try
        {
            request.client.Session.ReadValues(nodeIds, out values, out serviceResults);
        }
        catch (Exception e)
        {
            return Result.Fail(DomainErrors.Opc.Client.Reading.ReadNodeError.CausedBy(e));
        }
        List<Error> errors = serviceResults.Where(svResult => svResult != ServiceResult.Good).Select(s => DomainErrors.Opc.Client.Reading.NotGoodResultError.WithMetadata(nameof(ServiceResult), s)).ToList();


        return errors.Count > 0 ? Result.Fail(errors) : Result.Ok(values);
 
    }
}
