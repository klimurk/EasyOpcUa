using FluentResults;
using Opc.Ua;
using System.Text;

namespace OpcUa.Domain.Errors;

public class ServerNotFoundedError : Error
{
    public ServerNotFoundedError(string uri, Exception ex = default)
        : base(BuildError(uri, ex)) =>
        Metadata.Add(nameof(ErrorCodes.ServersNotFoundCode), ErrorCodes.ServersNotFoundCode);

    private static string BuildError(string uri, Exception ex)
    {
        StringBuilder sb = new($"Servers not found on uri {uri}");
        if (ex != default) sb.AppendLine($"Exception {ex}");
        return sb.ToString();
    }
}
public class ServerConnectionError : Error
{
    public ServerConnectionError(EndpointDescription endpointDescription, UserIdentity userIdentity, ApplicationConfiguration? clientAppConfig, Exception e = default)
        : base(BuildError(endpointDescription, userIdentity, clientAppConfig, e)) => Metadata.Add(nameof(ErrorCodes.ConnectServerErrorCode), ErrorCodes.ConnectServerErrorCode);

    private static string BuildError(EndpointDescription endpointDescription, UserIdentity userIdentity, ApplicationConfiguration? clientAppConfig, Exception ex)
    {
        StringBuilder sb = new($"Error while create connection. Endpoint {endpointDescription.EndpointUrl}. ");
        if (userIdentity is not null) sb.AppendLine($"User {userIdentity.DisplayName}");
        if (clientAppConfig is not null) sb.AppendLine($"User {clientAppConfig.ProductUri}");
        if (ex != default) sb.AppendLine($"Exception {ex}");

        return sb.ToString();
    }
}


