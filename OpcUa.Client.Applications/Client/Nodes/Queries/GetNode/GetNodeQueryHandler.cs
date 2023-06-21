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
            Node result= request.Client.Session.ReadNode(new NodeId(idString));
            return result;
        }
        catch (Exception e)
        {
            return Result.Fail(new ReadNodeError(request.Client, request.NodeId, e));
        }
    }
}
public class GetNodeListQueryHandler : IRequestHandler<GetNodeListQuery, Result<IList<Node>>>
{
    public async Task<Result<IList<Node>>> Handle(GetNodeListQuery request, CancellationToken cancellationToken)
    {
        IList<NodeId> nodeIds = request.NodeId.Select(s => new NodeId(s.Trim())).ToList();
        try
        {
            request.Client.Session.ReadNodes(nodeIds, out IList<Node> result, out IList<ServiceResult> serviceResult);
            var errors = serviceResult.Where(s => s.StatusCode != StatusCodes.Good).Select(s => new ReadNodeError(request.Client, s.AdditionalInfo));
            if(errors.Any())  return Result.Fail(errors); 
            return Result.Ok(result);
        }
        catch (Exception e)
        {
            return Result.Fail(new ReadNodeError(request.Client, request.NodeId.First(), e));
        }
    }
}
