using FluentResults;
using Opc.Ua;
using OpcUa.Domain.Contracts.Client;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUa.Domain.Errors;

public class ReadNodeError : Error
{
    public ReadNodeError(IOpcClient client, NodeId node, Exception e = default)
        : base(BuildError(client, node, e)) =>
        Metadata.Add(nameof(ErrorCodes.ReadNodeCode), ErrorCodes.ReadNodeCode);

    private static string BuildError(IOpcClient client, NodeId node, Exception e)
    {
        StringBuilder sb = new StringBuilder($"Cannot read node {node} references in client with url {client.Session.Endpoint.EndpointUrl}.");
        if (e != default) sb.AppendLine($"Exception {e}");
        return sb.ToString();
    }


}
