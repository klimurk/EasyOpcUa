using MediatR;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Application.Client.Queries.Values.List.NodeIds;

/// <param name="nodeIdStrings">The node Ids as strings</param>
public record ReadListValueQuery(IOpcClient client, IEnumerable<string> nodeIdStrings, Type ExpectedType = null) : IRequest<OpcReadListResult> { }
