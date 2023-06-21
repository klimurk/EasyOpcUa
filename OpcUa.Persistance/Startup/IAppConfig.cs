namespace OpcUa.Persistance.Startup;

public interface IAppConfig
{
    PlcConfig PlcData { get; set; }

    InitData Initialization { get; set; }
}

public class InitData
{
    public IEnumerable<OpcInit> Opc { get; set; }
}

public class OpcInit
{
    public string Name { get; set; }
    public string OpcAddress { get; set; }
    public string VisuAddress { get; set; }
    public List<OpcSignalInit> Signals { get; set; }
}

public class OpcSignalInit
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string Description { get; set; }
    public string Device { get; set; }
}