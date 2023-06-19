using OpcUa.Client.Wpf.Core.Models;

namespace OpcUa.Client.Wpf.Core.Contracts.Services;

public interface ISampleDataService
{
    Task<IEnumerable<SampleOrder>> GetGridDataAsync();
}
