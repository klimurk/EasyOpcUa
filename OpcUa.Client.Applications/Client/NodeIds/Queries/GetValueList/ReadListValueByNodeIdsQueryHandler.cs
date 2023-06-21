using FluentResults;
using FluentValidation;
using MediatR;
using Opc.Ua;
using OpcUa.Domain.Errors;

namespace OpcUa.Client.Applications.Client.NodeIds.Queries.GetValueList;


/// <summary>Reads values from node Ids</summary>
/// <returns>The read values as strings</returns>
/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
public class ReadListValueByNodeIdsQueryHandler : IRequestHandler<ReadListValueByNodeIdsQuery, Result<DataValueCollection>>
{
    public async Task<Result<DataValueCollection>> Handle(ReadListValueByNodeIdsQuery request, CancellationToken cancellationToken)
    {
        List<NodeId> nodeIds = request.NodeIds.Select(s => new NodeId(s)).ToList();
        DataValueCollection values = new();
        IList<ServiceResult> serviceResults = new List<ServiceResult>();
        List<BuiltInType> types = Enumerable.Repeat(request.ExpectedType, nodeIds.Count).ToList();
        try
        {
            request.Client.Session.ReadValues(nodeIds, out values, out serviceResults);
        }
        catch (Exception e)
        {
            return Result.Fail(new ReadNodeError(request.Client, request.NodeIds.First(), e));
        }
        List<ReadNodeResultNotGoodError> errors = serviceResults.Where(svResult => svResult != ServiceResult.Good).Select(s => new ReadNodeResultNotGoodError()).ToList();


        return errors.Count > 0 ? Result.Fail(errors) : Result.Ok(values);

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

internal class ReadNodeResultNotGoodError : Error
{
}