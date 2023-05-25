using MediatR;
using Opc.Ua;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Application.Client.Queries.Values.List.NodeIds;

/// <param name="NodeIds">The node Ids as strings</param>
public record ReadListValueQuery(IOpcClient Client, IEnumerable<string> NodeIds, BuiltInType ExpectedType ) : IRequest<OpcReadListResult> { }
