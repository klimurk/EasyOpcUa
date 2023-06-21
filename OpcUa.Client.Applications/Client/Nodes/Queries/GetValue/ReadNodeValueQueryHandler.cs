using FluentResults;
using MediatR;
using OpcUa.Domain.Errors;

namespace OpcUa.Client.Applications.Client.Nodes.Queries.GetValue;

public class ReadNodeValueQueryHandler : IRequestHandler<ReadNodeValueQuery, Result<object>>
{
    public async Task<Result<object>> Handle(ReadNodeValueQuery request, CancellationToken cancellationToken)
    {
        object result;
        try
        {
            result = request.client.Session.ReadValue(request.nodes.NodeId, request.ExpectedType);
        }
        catch (Exception e)
        {
            return Result.Fail(new ReadNodeError(request.client, request.nodes.NodeId, e));
        }
        return result;
    }
}
