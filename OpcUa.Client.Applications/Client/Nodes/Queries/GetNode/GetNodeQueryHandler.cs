using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Applications.Errors;

namespace OpcUa.Client.Applications.Client.Nodes.Queries.GetNode;

/// <summary>Reads a node by node Id</summary>
/// <returns>The read node</returns>
/// <exception cref="ReadNodeException">Throws and forwards exception while have problem with connection.</exception>
public class GetNodeQueryHandler : IRequestHandler<GetNodeQuery, Result<Node>>
{
    public async Task<Result<Node>> Handle(GetNodeQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request.Client);
        string idString = request.NodeId.Trim();
        ArgumentException.ThrowIfNullOrEmpty(idString);
        Node result;
        try
        {
            result = request.Client.Session.ReadNode(new NodeId(idString));
        }
        catch (Exception e)
        {
            return Result.Fail(new ReadNodeError(request.Client, request.NodeId, e));
        }
        return result;
    }
}

