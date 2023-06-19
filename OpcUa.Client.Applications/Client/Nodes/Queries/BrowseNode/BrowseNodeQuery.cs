using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Client.Applications.Queries.Browsings.Inner;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Client.Applications.Client.Nodes.Queries.BrowseNode;

/// <param name="node">Node</param>
/// <param name="Client">Client you want to read</param>
public record BrowseNodeQuery(IOpcClient Client, Node node) : IRequest<Result<ReferenceDescriptionCollection>> { }
