using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Domain.Errors;

namespace OpcUa.Client.Applications.Client.Nodes.Queries.BrowseNode;



/// <summary>Browses a node ID provided by a Node</summary>
/// <returns>ReferenceDescriptionCollection of found nodes</returns>
public class BrowseNodeQueryHandler : IRequestHandler<BrowseNodeQuery, Result<ReferenceDescriptionCollection>>
{
    public async Task<Result<ReferenceDescriptionCollection>> Handle(BrowseNodeQuery request, CancellationToken cancellationToken)
    {
        //Create a NodeId using the selected ReferenceDescription as browsing starting point
        NodeId nodeId = ExpandedNodeId.ToNodeId(request.node.NodeId, null);
        ReferenceDescriptionCollection referenceDescriptionCollection;
        try
        {
            //Browse from starting point for all object types
            request.Client.Session.Browse(null, view: null, nodeId, 0u, BrowseDirection.Forward, ReferenceTypeIds.HierarchicalReferences, true, 0, out byte[] continuationPoint, out referenceDescriptionCollection);
            // todo: check arra not empty
            while (continuationPoint != null)
            {
                request.Client.Session.BrowseNext(null, false, continuationPoint, out byte[] revisedContinuationPoint, out ReferenceDescriptionCollection nextreferenceDescriptionCollection);
                referenceDescriptionCollection.AddRange(nextreferenceDescriptionCollection);
                continuationPoint = revisedContinuationPoint;
            }

        }
        catch (Exception e)
        {
            //handle Exception here
            return Result.Fail( DomainErrors.Opc.Client.Browsing.BrowsingNodeError.CausedBy(e));

        }
        return referenceDescriptionCollection;
    }
}

