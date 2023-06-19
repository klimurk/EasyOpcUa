using FluentResults;
using Opc.Ua;
using System.Text;

namespace OpcUa.Applications.Errors;

public class ClientCertificateValidationError : Error
{
	public ClientCertificateValidationError(ApplicationConfiguration applicationConfiguration, Exception ex)
		: base(BuildError(applicationConfiguration, ex)) =>
		Metadata.Add(nameof(ErrorCodes.CertificateValidationErrorCode), ErrorCodes.CertificateValidationErrorCode);

	private static string BuildError(ApplicationConfiguration applicationConfiguration, Exception ex)
	{
		StringBuilder sb = new("Certificate validation error. ");
		if (applicationConfiguration is not null) sb.Append($"Configuration {applicationConfiguration}. ");
		if (ex is not null) sb.Append($"Exception {ex}");
		return sb.ToString();
	}
}