using System.Windows.Controls;

using MahApps.Metro.Controls;

using OpcUa.Client.Wpf.Contracts.Views;
using OpcUa.Client.Wpf.ViewModels;

namespace OpcUa.Client.Wpf.Views;

public partial class ShellWindow : MetroWindow, IShellWindow
{
    public ShellWindow(ShellViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    public Frame GetNavigationFrame()
        => shellFrame;

    public void ShowWindow()
        => Show();

    public void CloseWindow()
        => Close();
}
