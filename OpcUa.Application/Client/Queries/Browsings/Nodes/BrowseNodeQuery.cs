using MediatR;
using Opc.Ua;
using OpcUa.Application.Client.Queries.Browsings.Inner;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Application.Client.Queries.Browsings.Nodes;

/// <param name="node">Node</param>
/// <param name="Client">Client you want to read</param>
public record BrowseNodeQuery(IOpcClient Client, Node node) : IRequest<ReferenceDescriptionCollection?> { }
