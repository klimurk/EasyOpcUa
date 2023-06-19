namespace OpcUa.Persistance.Startup;

public interface IAppConfig
{
    PlcConfig PlcData { get; set; }
}