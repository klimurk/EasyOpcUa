using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Client.Applications.Client.Nodes.Queries.GetNodeList;

public record class GetNodeListQuery(IOpcClient Client, IEnumerable<string> NodeId) : IRequest<Result<IList<Node>>>;
