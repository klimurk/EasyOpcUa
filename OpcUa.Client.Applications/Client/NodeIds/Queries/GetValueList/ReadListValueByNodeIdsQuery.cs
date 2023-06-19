using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Client.Applications.Client.NodeIds.Queries.GetValueList;

/// <param name="NodeIds">The node Ids as strings</param>
public record ReadListValueByNodeIdsQuery(IOpcClient Client, IEnumerable<string> NodeIds, BuiltInType ExpectedType) : IRequest<Result<DataValueCollection>> { }
