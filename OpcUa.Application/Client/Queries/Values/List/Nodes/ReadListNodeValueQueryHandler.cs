using FluentValidation;
using MediatR;
using Opc.Ua;
using OpcUa.Application.Client.Exceptions;

namespace OpcUa.Application.Client.Queries.Values.List.Nodes;

public class ReadListNodeValueQueryHandler : IRequestHandler<ReadListNodeValueQuery, OpcReadListResult>
{
    public async Task<OpcReadListResult> Handle(ReadListNodeValueQuery request, CancellationToken cancellationToken)
    {
        List<NodeId> nodeIds = request.nodes.Select(s => s.NodeId).ToList();
        DataValueCollection values = new();
        IList<ServiceResult> serviceResults = new List<ServiceResult>();
        List<Type> types = Enumerable.Repeat(request.ExpectedType, nodeIds.Count).ToList();
        try
        {
            request.client.Session.ReadValues(nodeIds, out values, out serviceResults);
        }
        catch (Exception e)
        {
            throw new OpcConnectionException("Error connection while read values", e.InnerException);
        }
        foreach (var svResult in serviceResults.Where(svResult => svResult != ServiceResult.Good))
        {
            throw new ReadNodeException($"Error {svResult.Code} while reading in {svResult.NamespaceUri} namespace with result{svResult}.");
        }
        IEnumerable<object> result = values.Select(s => s.Value);
        // exclude bytes with symbol '-'
        //// check byte[]
        //if (result.First() is byte[])
        //{
        //	List<byte> byteArr = result.OfType<byte>().ToList();
        //	byte exclude = 45; // symbol '-'
        //	if (byteArr.Any()) byteArr.RemoveAll(s => s == exclude);
        //	result = byteArr.ToArray();
        //}
        return new OpcReadListResult(result);
    }
}
