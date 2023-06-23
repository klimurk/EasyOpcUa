using FluentResults;
using MediatR;
using Newtonsoft.Json.Linq;
using Opc.Ua;
using OpcUa.Domain.Contracts.Client;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpcUa.Client.Applications.Client.Nodes.Commands.WriteNodeValue
{
    public record class WriteNodeValueCommand(IOpcClient Client, Node node, object Value) : IRequest<Result>;
}
