using FluentResults;
using Opc.Ua;
using System.Text;

namespace OpcUa.Domain.Errors;

public class ClientCertificateValidationError : Error
{
    public ClientCertificateValidationError(ApplicationConfiguration configuration, Exception exception = default)
        : base(BuildError(configuration, exception)) =>
        Metadata.Add(nameof(ErrorCodes.CertificateValidationErrorCode), ErrorCodes.CertificateValidationErrorCode);

    private static string BuildError(ApplicationConfiguration configuration, Exception exception)
    {
        StringBuilder sb = new StringBuilder($"Cannot validate client certification with configuration{configuration.ApplicationUri}.");
        if (exception != default) sb.Append(exception);
        return sb.ToString();
    }
}
