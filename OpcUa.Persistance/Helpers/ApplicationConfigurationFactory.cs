using Opc.Ua;
using Opc.Ua.Security.Certificates;
using System.Security.Cryptography.X509Certificates;

namespace OpcUa.Helpers;

internal static class ApplicationConfigurationFactory
{
	public async static Task<ApplicationConfiguration> Create(
		string appName = "UA Client 1500", string appUri = "urn:MyClient",
		string certificateIdentifierStorePath = "CurrentUserStore\\Store", string certificateStoreType = CertificateStoreType.X509Store,
		int operationTimeout = 60000, int maxStringLength = 67108864, int MaxByteStringLength = 16777216,
		string certificateTrustStorePath = "CurrentUser\\Root"
		)
	{
		// The application configuration can be loaded from any file.
		// ApplicationConfiguration.Load() method loads configuration by looking up a file path in the App.config.
		// This approach allows applications to share configuration files and to update them.
		// This example creates a minimum ApplicationConfiguration using its default constructor.
		var certificate = new CertificateIdentifier()
		{
			StoreType = certificateStoreType,
			StorePath = certificateIdentifierStorePath,
			SubjectName = appName
		};
		ApplicationConfiguration configuration = new()
		{
			// Step 1 - Specify the client identity.
			ApplicationName = appName,
			ApplicationType = ApplicationType.Client,
			ApplicationUri = appUri, //Kepp this syntax

			// Step 2 - Specify the client's application instance certificate.
			// Application instance certificates must be placed in a windows certficate store because that is
			// the best way to protect the private key. Certificates in a store are identified with 4 parameters:
			// StoreLocation, StoreName, SubjectName and Thumbprint.
			// When using StoreType = Directory you need to have the opc.ua.certificategenerator.exe installed on your machine

			SecurityConfiguration = new SecurityConfiguration()
			{
				ApplicationCertificate = certificate,
				AutoAcceptUntrustedCertificates = true,
				RejectSHA1SignedCertificates = false,
			},
			TransportQuotas = new TransportQuotas
			{
				OperationTimeout = operationTimeout,
				//SecurityTokenLifetime = 86400000,
				MaxStringLength = maxStringLength,
				MaxByteStringLength = MaxByteStringLength //Needed, i.e. for large TypeDictionarys
			},
			//ClientConfiguration = new ClientConfiguration
			//{
			//	DefaultSessionTimeout = 60000
			//}
		};

		var trustlist = new CertificateTrustList()
		{
			StoreType = certificateStoreType,
			StorePath = certificateTrustStorePath
		};
		// Define trusted root store for server certificate checks
		configuration.SecurityConfiguration.TrustedIssuerCertificates = trustlist;
		configuration.SecurityConfiguration.TrustedPeerCertificates = trustlist;

		// find the client certificate in the store.
		X509Certificate2 clientCertificate = await configuration.SecurityConfiguration.ApplicationCertificate.Find(true);

		// create a new self signed certificate if not found.
		if (clientCertificate == null)
		{
			var builder = CertificateBuilder.Create(string.Empty);
			builder.CreateForRSA();
		}
		// This step checks if the configuration is consistent and assigns a few internal variables
		// that are used by the SDK. This is called automatically if the configuration is loaded from
		// a file using the ApplicationConfiguration.Load() method.
		await configuration.Validate(ApplicationType.Client);
		return configuration;
	}
}
