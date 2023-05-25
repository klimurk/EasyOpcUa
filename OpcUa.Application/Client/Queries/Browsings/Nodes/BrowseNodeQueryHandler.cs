using MediatR;
using Opc.Ua;
using OpcUa.Application.Client.Exceptions;

namespace OpcUa.Application.Client.Queries.Browsings.Nodes;



/// <summary>Browses a node ID provided by a Node</summary>
/// <returns>ReferenceDescriptionCollection of found nodes</returns>
/// <exception cref="OpcConnectionException">Throws and forwards exception while have error while browsing.</exception>
public class BrowseNodeQueryHandler : IRequestHandler<BrowseNodeQuery, ReferenceDescriptionCollection?>
{
    public async Task<ReferenceDescriptionCollection?> Handle(BrowseNodeQuery request, CancellationToken cancellationToken)
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
            throw new OpcConnectionException("Error while browsing", e.InnerException);
        }
        return referenceDescriptionCollection;
    }
}


