using FluentResults;
using Opc.Ua;
using System.Text;

namespace OpcUa.Domain.Errors;

//public class EndpointsNotFoundedError : Error
//{
//    public EndpointsNotFoundedError(string uri, Exception e = default) 
//        : base(BuildError(uri, e)) => 
//        Metadata.Add(nameof(ErrorCodes.EndpointsNotFoundCode), ErrorCodes.EndpointsNotFoundCode);
//    private static string BuildError(string uri, Exception e )
//    {
//        StringBuilder sb = new($"Endpoints on {uri} not found");
//        if (e != default) sb.Append($"Exception {e}");
//        return sb.ToString();
//    }
//}
//public class EndpointWithSequrityCodeError : Error
//{
//    public EndpointWithSequrityCodeError(string uri, MessageSecurityMode messageSecurityMode)
//        : base(BuildError(uri, messageSecurityMode)) =>
//        Metadata.Add(nameof(ErrorCodes.EndpointSecurityCodeNotFoundCode), ErrorCodes.EndpointSecurityCodeNotFoundCode);

//    private static string BuildError(string uri, MessageSecurityMode messageSecurityMode)
//    {
//        return $"Endpoints on {uri} with sequrity {messageSecurityMode} not found.";
//    }
//}