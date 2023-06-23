using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Domain.Errors;

namespace OpcUa.Client.Applications.Client.Nodes.Queries.GetNodeList;

public class GetNodeListQueryHandler : IRequestHandler<GetNodeListQuery, Result<IList<Node>>>
{
    public async Task<Result<IList<Node>>> Handle(GetNodeListQuery request, CancellationToken cancellationToken)
    {
        IList<NodeId> nodeIds = request.NodeId.Select(s => new NodeId(s.Trim())).ToList();
        try
        {
            request.Client.Session.ReadNodes(nodeIds, out IList<Node> result, out IList<ServiceResult> serviceResult);
            var errors = serviceResult.Where(s => s.StatusCode != StatusCodes.Good).Select(s => DomainErrors.Opc.Client.Reading.NotGoodResultError.WithMetadata(nameof(StatusCode), s)).ToList();

            if (errors.Any()) return Result.Fail(errors);
            return Result.Ok(result);
        }
        catch (Exception e)
        {
            return Result.Fail(DomainErrors.Opc.Client.Reading.ReadNodeError.CausedBy(e));
        }
    }
}
