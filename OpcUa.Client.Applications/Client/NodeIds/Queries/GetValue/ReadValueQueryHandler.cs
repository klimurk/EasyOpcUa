using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Applications.Errors;

namespace OpcUa.Client.Applications.Client.NodeIds.Queries.GetValue;

public class ReadValueQueryHandler : IRequestHandler<ReadValueQuery, Result<object>>
{
    public async Task<Result<object>> Handle(ReadValueQuery request, CancellationToken cancellationToken)
    {
        object result;
        try
        {
            result = request.client.Session.ReadValue(new NodeId(request.nodeId), request.ExpectedType);
        }
        catch (Exception e)
        {
            return Result.Fail(new ReadNodeError(request.client, request.nodeId, e));
        }
        return result;
    }
}
