using FluentResults;
using Opc.Ua;
using System.Text;

namespace OpcUa.Applications.Errors;

public class ConnectServerError : Error
{
	public ConnectServerError(EndpointDescription endpointDescription, UserIdentity userIdentity, ApplicationConfiguration? clientAppConfig, Exception e)
		: base(BuildError(endpointDescription, userIdentity, clientAppConfig, e)) { Metadata.Add(nameof(ErrorCodes.ConnectServerErrorCode), ErrorCodes.ConnectServerErrorCode); }

	private static string BuildError(EndpointDescription endpointDescription, UserIdentity userIdentity, ApplicationConfiguration? clientAppConfig, Exception ex)
	{
		StringBuilder sb = new($"Error while create connection. Endpoint {endpointDescription.EndpointUrl}. ");
		if (userIdentity is not null) sb.Append($"User {userIdentity.DisplayName}");
		if (clientAppConfig is not null) sb.Append($"User {clientAppConfig.ProductUri}");
		if (ex is not null) sb.Append($"Exception {ex}");

		return sb.ToString();
	}
}