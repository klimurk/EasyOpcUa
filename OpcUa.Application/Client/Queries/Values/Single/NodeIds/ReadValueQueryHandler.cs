using MediatR;
using Opc.Ua;
using OpcUa.Application.Client.Exceptions;

namespace OpcUa.Application.Client.Queries.Values.Single.NodeIds;

public class ReadValueQueryHandler : IRequestHandler<ReadValueQuery, object>
{
	public async Task<object> Handle(ReadValueQuery request, CancellationToken cancellationToken)
	{
		object result;
		try
		{
			result = request.client.Session.ReadValue(new NodeId(request.nodeId), request.ExpectedType);
		}
		catch (Exception e)
		{
			throw new OpcConnectionException("Error connection while read values", e.InnerException);
		}
		return result;
	}
}
