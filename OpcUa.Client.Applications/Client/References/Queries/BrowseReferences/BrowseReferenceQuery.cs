using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Client.Applications.Client.References.Queries.BrowseReferences;

/// <param name="RefDesc">The ReferenceDescription</param>
/// <param name="Client">Client you want to read</param>
public record BrowseReferenceQuery(IOpcClient Client, ReferenceDescription RefDesc) : IRequest<Result<ReferenceDescriptionCollection>?> { }
