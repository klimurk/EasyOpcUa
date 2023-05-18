using MediatR;
using Opc.Ua;
using OpcUa.Application.Client.Queries.Browsings.Nodes;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Application.Client.Queries.Browsings.References;

/// <param name="RefDesc">The ReferenceDescription</param>
/// <param name="Client">Client you want to read</param>
public record BrowseReferenceQuery(IOpcClient Client, ReferenceDescription RefDesc) : IRequest<ReferenceDescriptionCollection?> { }
