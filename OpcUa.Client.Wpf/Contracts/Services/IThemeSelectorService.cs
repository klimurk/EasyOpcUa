using OpcUa.Client.Wpf.Models;

namespace OpcUa.Client.Wpf.Contracts.Services;

public interface IThemeSelectorService
{
    void InitializeTheme();

    void SetTheme(AppTheme theme);

    AppTheme GetCurrentTheme();
}
