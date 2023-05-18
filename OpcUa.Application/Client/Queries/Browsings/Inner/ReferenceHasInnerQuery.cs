using MediatR;
using Opc.Ua;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Application.Client.Queries.Browsings.Inner;

/// <param name="node">Node</param>
/// <param name="Client">Client you want to read</param>
public record ReferenceHasInnerQuery(IOpcClient Client, Node node) : IRequest<bool> { }


