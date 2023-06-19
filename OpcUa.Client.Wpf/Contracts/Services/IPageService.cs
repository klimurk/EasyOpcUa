using System.Windows.Controls;

namespace OpcUa.Client.Wpf.Contracts.Services;

public interface IPageService
{
    Type GetPageType(string key);

    Page GetPage(string key);
}
