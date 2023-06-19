using System.Windows.Controls;

namespace OpcUa.Client.Wpf.Contracts.Views;

public interface IShellWindow
{
    Frame GetNavigationFrame();

    void ShowWindow();

    void CloseWindow();
}
