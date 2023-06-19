using System.Windows.Controls;

using OpcUa.Client.Wpf.ViewModels;

namespace OpcUa.Client.Wpf.Views;

public partial class MainPage : Page
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
