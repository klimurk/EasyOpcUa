using FluentResults;
using FluentValidation;
using MediatR;
using Opc.Ua;
using OpcUa.Domain.Errors;
using OpcUa.Client.Applications.Client.NodeIds.Queries.GetValueList;

namespace OpcUa.Client.Applications.Client.Nodes.Queries.GetValueList;

public class ReadListNodeValueQueryHandler : IRequestHandler<ReadListNodeValueQuery, Result<DataValueCollection>>
{
    public async Task<Result<DataValueCollection>> Handle(ReadListNodeValueQuery request, CancellationToken cancellationToken)
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
            return Result.Fail(new ReadNodeError(request.client, request.nodes.First().NodeId, e));
        }
        List<ReadNodeResultNotGoodError> errors = serviceResults.Where(svResult => svResult != ServiceResult.Good).Select(s => new ReadNodeResultNotGoodError()).ToList();


        return errors.Count > 0 ? Result.Fail(errors) : Result.Ok(values);
        //IEnumerable<object> result = values.Select(s => s.Value);
        // exclude bytes with symbol '-'
        //// check byte[]
        //if (result.First() is byte[])
        //{
        //	List<byte> byteArr = result.OfType<byte>().ToList();
        //	byte exclude = 45; // symbol '-'
        //	if (byteArr.Any()) byteArr.RemoveAll(s => s == exclude);
        //	result = byteArr.ToArray();
        //}
    }
}
