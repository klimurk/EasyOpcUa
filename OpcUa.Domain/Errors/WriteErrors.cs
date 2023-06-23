using FluentResults;
using Opc.Ua;
using OpcUa.Domain.Contracts.Client;
using System.Text;

namespace OpcUa.Domain.Errors;

//public class WriteNodeError : Error
//{

//    public WriteNodeError(IOpcClient client, Node node, object value, Exception ex = default) : base(BuildError(client, node, value, ex))
//    {

//    }

//    private static string BuildError(IOpcClient client, Node node, object value, Exception ex)
//    {
//        StringBuilder sb =
//            new($"Error while write value {value} on node {node.NodeId}. Client {client.Name}");
//        if (ex != null) { sb.AppendLine($"Exception {ex}"); }
//        return sb.ToString();


//    }
//}
//public class WrongWriteValueTypeError : Error
//{
//    public WrongWriteValueTypeError(Node node, Type valueType, Type expectedType)
//        : base(BuildError(node, valueType, expectedType))
//    {
//    }

//    private static string BuildError(Node node, Type valueType, Type expectedType)
//    {
//        return $"Type mismatch {valueType} on writing {node.NodeId}. Expected type {expectedType}.";
//    }
//}
