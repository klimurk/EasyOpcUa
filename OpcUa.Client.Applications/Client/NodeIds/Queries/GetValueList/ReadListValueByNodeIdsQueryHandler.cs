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
            return Result.Fail(DomainErrors.Opc.Client.Reading.ReadNodeError.CausedBy(e));
        }
        List<Error> errors = serviceResults.Where(svResult => svResult != ServiceResult.Good).Select(s => DomainErrors.Opc.Client.Reading.NotGoodResultError.WithMetadata(nameof(ServiceResult), s)).ToList();


        return errors.Count > 0 ? Result.Fail(errors) : Result.Ok(values);


    }
}

internal class ReadNodeResultNotGoodError : Error
{
}