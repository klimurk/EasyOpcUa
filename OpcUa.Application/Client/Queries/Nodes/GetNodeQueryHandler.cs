using MediatR;
using Opc.Ua;
using OpcUa.Application.Client.Exceptions;

namespace OpcUa.Application.Client.Queries.Nodes;

/// <summary>Reads a node by node Id</summary>
/// <returns>The read node</returns>
/// <exception cref="ReadNodeException">Throws and forwards exception while have problem with connection.</exception>
public class GetNodeQueryHandler : IRequestHandler<GetNodeQuery, Node?>
{
	public async Task<Node?> Handle(GetNodeQuery request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request.client);
		string idString = request.nodeIdString.Trim();
		ArgumentException.ThrowIfNullOrEmpty(idString);
		Node result;
		try
		{
			result = request.client.Session.ReadNode(new NodeId(idString));
		}
		catch (Exception e)
		{
			throw new ReadNodeException($"Cannot read node '{idString}' from session", e.InnerException);
		}
		return result;
	}
}

