using System.Windows.Controls;

using OpcUa.Client.Wpf.ViewModels;

namespace OpcUa.Client.Wpf.Views;

public partial class ClientsPage : Page
{
    public ClientsPage(ClientsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
