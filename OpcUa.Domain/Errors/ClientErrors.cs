using FluentResults;
using Opc.Ua;
using System.Text;

namespace OpcUa.Domain.Errors;

//public class ClientCertificateValidationError : Error
//{
//    public ClientCertificateValidationError(ApplicationConfiguration configuration)
//        : base(BuildError(configuration)) =>
//        Metadata.Add(nameof(ErrorCodes.CertificateValidationErrorCode), ErrorCodes.CertificateValidationErrorCode);

//    private static string BuildError(ApplicationConfiguration configuration)
//    {
//        StringBuilder sb = new StringBuilder($"Cannot validate client certification with configuration{configuration.ApplicationUri}.");
//        return sb.ToString();
//    }
//}
