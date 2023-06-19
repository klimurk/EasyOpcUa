using System.Windows.Controls;

using OpcUa.Client.Wpf.ViewModels;

namespace OpcUa.Client.Wpf.Views;

public partial class SettingsPage : Page
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
