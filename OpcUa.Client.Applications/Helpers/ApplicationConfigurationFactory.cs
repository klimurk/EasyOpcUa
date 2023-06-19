using Opc.Ua;
using Opc.Ua.Security.Certificates;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Opc.Ua.Server;

namespace OpcUa.Client.Applications.Helpers;

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
        X509Certificate2 clientCertificate = new();
        // find the client certificate in the store.
        try
        {
            clientCertificate = await configuration.SecurityConfiguration.ApplicationCertificate.Find(true);
        }
        catch
        {
            var builder = CertificateBuilder.Create(string.Empty);
            clientCertificate = builder.CreateForRSA();
        }


        // This step checks if the configuration is consistent and assigns a few internal variables
        // that are used by the SDK. This is called automatically if the configuration is loaded from
        // a file using the ApplicationConfiguration.Load() method.
        await configuration.Validate(ApplicationType.Client);
        return configuration;
    }
    public static ApplicationConfiguration CreateClientConfiguration(string appName = "UA Client 1500", string appUri = "urn:MyClient",
        string productUri = "SiemensAG.IndustryOnlineSupport",
        string appCertificatePath = "CurrentUser\\My",
        ushort certificateMonthLifetime = 24,
        string certificateIdentifierStorePath = "CurrentUserStore\\Store",
        int operationTimeout = 60000, int maxStringLength = 67108864, int MaxByteStringLength = 16777216,
        string certificateTrustStorePath = "CurrentUser\\Root")
    {
        // The application configuration can be loaded from any file.
        // ApplicationConfiguration.Load() method loads configuration by looking up a file path in the App.config.
        // This approach allows applications to share configuration files and to update them.
        // This example creates a minimum ApplicationConfiguration using its default constructor.
        var trustList = new CertificateTrustList()
        {
            StoreType = CertificateStoreType.X509Store,
            StorePath = certificateTrustStorePath
        };
        ApplicationConfiguration configuration = new()
        {
            // Step 1 - Specify the client identity.
            ApplicationName = appName,
            ApplicationType = ApplicationType.Client,
            ApplicationUri = appUri, //Kepp this syntax
            ProductUri = productUri,

            // Step 2 - Specify the client's application instance certificate.
            // Application instance certificates must be placed in a windows certficate store because that is
            // the best way to protect the private key. Certificates in a store are identified with 4 parameters:
            // StoreLocation, StoreName, SubjectName and Thumbprint.
            // When using StoreType = Directory you need to have the opc.ua.certificategenerator.exe installed on your machine

            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier()
                {
                    StoreType = CertificateStoreType.X509Store,
                    StorePath = appCertificatePath,
                    SubjectName = appName
                },
                AutoAcceptUntrustedCertificates = true,
                RejectSHA1SignedCertificates = false,
                TrustedIssuerCertificates = trustList,
                TrustedPeerCertificates = trustList
            }
        };

        // Define trusted root store for server certificate checks
        //configuration.SecurityConfiguration.TrustedIssuerCertificates.StoreType = CertificateStoreType.X509Store;
        //configuration.SecurityConfiguration.TrustedIssuerCertificates.StorePath = "CurrentUser\\Root";
        //configuration.SecurityConfiguration.TrustedPeerCertificates.StoreType = CertificateStoreType.X509Store;
        //configuration.SecurityConfiguration.TrustedPeerCertificates.StorePath = "CurrentUser\\Root";

        // find the client certificate in the store.
        Task<X509Certificate2> clientCertificate = configuration.SecurityConfiguration.ApplicationCertificate.Find(needPrivateKey: true);

        // create a new self signed certificate if not found.
        if (clientCertificate.Result == null)
        {
            CreateCertificate(configuration, certificateMonthLifetime);
        }

        // Step 3 - Specify the supported transport quotas.
        // The transport quotas are used to set limits on the contents of messages and are
        // used to protect against DOS attacks and rogue clients. They should be set to
        // reasonable values.
        configuration.TransportQuotas = new TransportQuotas
        {
            OperationTimeout = operationTimeout,
            //SecurityTokenLifetime = 86400000,
            MaxStringLength = 67108864,
            MaxByteStringLength = 16777216 //Needed, i.e. for large TypeDictionarys
        };

        // Step 4 - Specify the client specific configuration.
        configuration.ClientConfiguration = new ClientConfiguration
        {
            DefaultSessionTimeout = 360000
        };

        // Step 5 - Validate the configuration.
        // This step checks if the configuration is consistent and assigns a few internal variables
        // that are used by the SDK. This is called automatically if the configuration is loaded from
        // a file using the ApplicationConfiguration.Load() method.
        configuration.Validate(ApplicationType.Client);

        return configuration;
    }

    private static void CreateCertificate(ApplicationConfiguration configuration, ushort certificateMonthLifetime = 24)
    {
        // Get local interface ip addresses and DNS name
        List<string> localIps = GetLocalIpAddressAndDns();

        const ushort keySize = 2048; //must be multiples of 1024

        const ushort algorithm = 256; //0 = SHA1; 1 = SHA256

        // this code would normally be called as part of the installer - called here to illustrate.
        // create a new certificate and place it in the current user certificate store.

        CertificateFactory.CreateCertificate(
            configuration.SecurityConfiguration.ApplicationCertificate.StoreType,
            configuration.SecurityConfiguration.ApplicationCertificate.StorePath,
            password: null,
            configuration.ApplicationUri,
            configuration.ApplicationName,
            subjectName: null,
            localIps,
            keySize,
            DateTime.UtcNow,
            certificateMonthLifetime,
            algorithm);
    }

    private static List<string> GetLocalIpAddressAndDns()
    {
        List<string> localIps = new();
        var hostname = Dns.GetHostName();
        var host = Dns.GetHostEntry(hostname);
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIps.Add(ip.ToString());
            }
        }
        if (localIps.Count == 0)
        {
            throw new Exception("Local IP Address Not Found!");
        }
        //localIps.Add(Dns.GetHostName());
        return localIps;
    }
}
