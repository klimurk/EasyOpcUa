using FluentResults;
using Opc.Ua;
using Opc.Ua.Client;
using OpcUa.Domain.Contracts.Client;
using System.Text;

namespace OpcUa.Domain.Errors;

//public class CreateSessionSubscriptionError : Error
//{
//    public CreateSessionSubscriptionError(IOpcClient client, Exception ex = default) : base(BuildError(client, ex)) =>
//        Metadata.Add(nameof(ErrorCodes.CreateSessionSubscriptionErrorCode), ErrorCodes.CreateSessionSubscriptionErrorCode);

//    private static string BuildError(IOpcClient client, Exception ex)
//    {
//        StringBuilder stringBuilder = new($"Error while create subscription on client {client.Session.Endpoint.EndpointUrl}.");
//        if (ex != default) stringBuilder.AppendLine($"Exception {ex}");
//        return stringBuilder.ToString();
//    }


//}
//public class RemoveSubscriptionError : Error
//{
//    public RemoveSubscriptionError(IOpcClient client, Subscription sub, Exception e = default) : base(BuildError(client, sub, e)) => 
//        Metadata.Add(nameof(ErrorCodes.RemoveSubscriptionErrorCode), ErrorCodes.RemoveSubscriptionErrorCode);

//    private static string BuildError(IOpcClient client, Subscription sub, Exception e)
//    {
//        StringBuilder sb = new($"Cannot remove subscription {sub.DisplayName} on for client {client.Session.Endpoint.EndpointUrl}.");
//        if (e != default) sb.AppendLine($"Exception {e}");
//        return sb.ToString();
//    }
//}
//public class SubscriptionAlreadyExist : Error
//{
//    public SubscriptionAlreadyExist(IOpcClient Client, string name) : base(BuildError(Client, name)) => 
//        Metadata.Add(nameof(ErrorCodes.SubscriptionExist), ErrorCodes.SubscriptionExist);

//    private static string BuildError(IOpcClient client, string name)
//    {
//        return $"On client {client.Session.Endpoint.EndpointUrl} subscription with name {name} already exist.";
//    }
//}
//public class SubscribingNodeError : Error
//{
//    public SubscribingNodeError(IOpcClient client, NodeId nodeId, Exception e = default) : base(BuildError(client, nodeId, e))
//    {
//        Metadata.Add(nameof(ErrorCodes.SubscribeError), ErrorCodes.SubscribeError);
//    }

//    private static string BuildError(IOpcClient client, NodeId nodeId, Exception e)
//    {
//        StringBuilder sb = new($"Cannot subscribe node {nodeId} in client {client.Session.Endpoint.EndpointUrl}.");
//        if (e != default) sb.AppendLine($"Exception {e}");
//        return sb.ToString();
//    }
//}