using OpcUa.Persistance.Startup;

namespace OpcUa.Web;

public class AppConfig : IAppConfig
{
    public PlcConfig PlcData { get; set; }
    public InitData Initialization { get; set; }
}
