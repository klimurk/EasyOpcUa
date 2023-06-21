using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Client.Applications.Client.Nodes.Queries.GetNode;

/// <param name="NodeId">The node Id as string</param>
/// <param name="Client">Client you want to read</param>
public record class GetNodeQuery(IOpcClient Client, string NodeId) : IRequest<Result<Node>>;
public record class GetNodeListQuery(IOpcClient Client, IEnumerable<string> NodeId) : IRequest<Result<IList<Node>>>;
