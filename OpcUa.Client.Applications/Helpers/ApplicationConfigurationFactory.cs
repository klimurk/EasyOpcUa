using Opc.Ua;
using Opc.Ua.Security.Certificates;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Opc.Ua.Server;

namespace OpcUa.Client.Applications.Helpers;

internal class ApplicationConfigurationFactory
{
    string _AppName = "UA Client 1500";
    string _AppUri = "urn:MyClient";
    ushort _CertificateMonthLifetime = 24;
    string _ProductUri = "SiemensAG.IndustryOnlineSupport";
    int _OperationTimeout = 60000;
    int _SessionTimeout = 360000;
    int _MaxStringLength = 67108864;
    int _MaxByteStringLength = 16777216;
    CertificateTrustList TrustedIssuerCertificates = new()
    {
        StoreType = CertificateStoreType.X509Store,
        StorePath = "CurrentUser\\Root"
    };
    CertificateTrustList TrustedPeerCertificates = new()
    {
        StoreType = CertificateStoreType.X509Store,
        StorePath = "CurrentUser\\Root"
    };
    private CertificateIdentifier ApplicationCertificate = new()
    {
        StoreType = CertificateStoreType.X509Store,
        StorePath = "CurrentUser\\My",
        SubjectName = "UA Client 1500"
    };
    public ApplicationConfigurationFactory() { }
    public ApplicationConfigurationFactory WithTrustedPeerCertificates(
        string certificateStoreType = CertificateStoreType.X509Store,
        string storePath = "CurrentUser\\Root")
    {
        TrustedPeerCertificates = new()
        {
            StoreType = certificateStoreType,
            StorePath = storePath
        };
        return this;
    }
    public ApplicationConfigurationFactory WithTrustedIssuerCertificates(
        string certificateStoreType = CertificateStoreType.X509Store,
        string storePath = "CurrentUser\\Root")
    {
        TrustedIssuerCertificates = new()
        {
            StoreType = certificateStoreType,
            StorePath = storePath
        };
        return this;
    }
    public ApplicationConfigurationFactory WithApplicationCertificate(
      string certificateStoreType = CertificateStoreType.X509Store,
      string appCertificatePath = "CurrentUser\\My",
      string subjectName = "UA Client 1500")
    {
        ApplicationCertificate = new CertificateIdentifier()
        {
            StoreType = certificateStoreType,
            StorePath = appCertificatePath,
            SubjectName = subjectName
        };
        return this;
    }
    public ApplicationConfigurationFactory WithAppName(string appName)
    {
        _AppName = appName;
        return this;
    }
    public ApplicationConfigurationFactory WithAppUri(string appUri)
    {
        _AppUri = appUri;
        return this;
    }
    public ApplicationConfigurationFactory WithProductUri(string productUri)
    {
        _ProductUri = productUri;
        return this;
    }



    public ApplicationConfigurationFactory WithOperationTimeout(int operationTimeout)
    {
        _OperationTimeout = operationTimeout;
        return this;
    }
    public ApplicationConfigurationFactory WithMaxStringLength(int maxStringLength)
    {
        _MaxStringLength = maxStringLength;
        return this;
    }
    public ApplicationConfigurationFactory WithSessionTimeout(int sessionTimeout)
    {
        _SessionTimeout = sessionTimeout;
        return this;
    }
    public ApplicationConfigurationFactory WithCertificateLifetime(ushort certificateMonthLifetime)
    {
        _CertificateMonthLifetime = certificateMonthLifetime;
        return this;
    }
    public ApplicationConfigurationFactory WithMaxByteStringLength(int MaxByteStringLength)
    {
        _MaxByteStringLength = MaxByteStringLength;
        return this;
    }


    public async Task<ApplicationConfiguration> CreateAsync()
    {
        // The application configuration can be loaded from any file.
        // ApplicationConfiguration.Load() method loads configuration by looking up a file path in the App.config.
        // This approach allows applications to share configuration files and to update them.
        // This example creates a minimum ApplicationConfiguration using its default constructor.

        ApplicationConfiguration configuration = new()
        {
            // Step 1 - Specify the client identity.
            ApplicationName = _AppName,
            ApplicationType = ApplicationType.Client,
            ApplicationUri = _AppUri, //Kepp this syntax
            ProductUri = _ProductUri,

            // Step 2 - Specify the client's application instance certificate.
            // Application instance certificates must be placed in a windows certficate store because that is
            // the best way to protect the private key. Certificates in a store are identified with 4 parameters:
            // StoreLocation, StoreName, SubjectName and Thumbprint.
            // When using StoreType = Directory you need to have the opc.ua.certificategenerator.exe installed on your machine

            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = ApplicationCertificate,
                AutoAcceptUntrustedCertificates = true,
                RejectSHA1SignedCertificates = false,
                TrustedIssuerCertificates = TrustedIssuerCertificates,
                TrustedPeerCertificates = TrustedPeerCertificates
            }
        };

        // find the client certificate in the store.
        X509Certificate2 clientCertificate = await configuration.SecurityConfiguration.ApplicationCertificate.Find(needPrivateKey: true);

        // create a new self signed certificate if not found.
        if (clientCertificate == null)
        {
            CreateCertificate(configuration);
        }

        // Step 3 - Specify the supported transport quotas.
        // The transport quotas are used to set limits on the contents of messages and are
        // used to protect against DOS attacks and rogue clients. They should be set to
        // reasonable values.
        configuration.TransportQuotas = new TransportQuotas
        {
            OperationTimeout = _OperationTimeout,
            //SecurityTokenLifetime = 86400000,
            MaxStringLength = _MaxStringLength,
            MaxByteStringLength = _MaxByteStringLength //Needed, i.e. for large TypeDictionarys
        };

        // Step 4 - Specify the client specific configuration.
        configuration.ClientConfiguration = new ClientConfiguration
        {
            DefaultSessionTimeout = _SessionTimeout
        };

        // Step 5 - Validate the configuration.
        // This step checks if the configuration is consistent and assigns a few internal variables
        // that are used by the SDK. This is called automatically if the configuration is loaded from
        // a file using the ApplicationConfiguration.Load() method.
        await configuration.Validate(ApplicationType.Client);

        return configuration;
    }

    private void CreateCertificate(ApplicationConfiguration configuration)
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
            _CertificateMonthLifetime,
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
