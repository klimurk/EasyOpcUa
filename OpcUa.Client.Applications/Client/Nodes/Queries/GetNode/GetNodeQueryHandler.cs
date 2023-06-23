using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Domain.Errors;

namespace OpcUa.Client.Applications.Client.Nodes.Queries.GetNode;

/// <summary>Reads a node by node Id</summary>
/// <returns>The read node</returns>
/// <exception cref="ReadNodeException">Throws and forwards exception while have problem with connection.</exception>
public class GetNodeQueryHandler : IRequestHandler<GetNodeQuery, Result<Node>>
{
    public async Task<Result<Node>> Handle(GetNodeQuery request, CancellationToken cancellationToken)
    {
        string idString = request.NodeId.Trim();
        try
        {
            Node result = request.Client.Session.ReadNode(new NodeId(idString));
            return result;
        }
        catch (Exception e)
        {
            return Result.Fail(DomainErrors.Opc.Client.Reading.ReadNodeError.CausedBy(e));
        }
    }
}
