using MediatR;
using OpcUa.Application.Client.Exceptions;

namespace OpcUa.Application.Client.Queries.Values.List.Nodes;

public class ReadNodeValueQueryHandler : IRequestHandler<ReadNodeValueQuery, object>
{
    public async Task<object> Handle(ReadNodeValueQuery request, CancellationToken cancellationToken)
    {
        object result;
        try
        {
			result = request.client.Session.ReadValue(request.nodes.NodeId, request.ExpectedType);
        }
        catch (Exception e)
        {
            throw new BrowsingException("Error connection while read values", e.InnerException);
        }
        return result;
    }
}
