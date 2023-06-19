using System.Diagnostics;
using System.Reflection;

using OpcUa.Client.Wpf.Contracts.Services;

namespace OpcUa.Client.Wpf.Services;

public class ApplicationInfoService : IApplicationInfoService
{
    public ApplicationInfoService()
    {
    }

    public Version GetVersion()
    {
        // Set the app version in OpcUa.Client.Wpf > Properties > Package > PackageVersion
        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var version = FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;
        return new Version(version);
    }
}
