using System.Windows.Controls;

using OpcUa.Client.Wpf.ViewModels;

namespace OpcUa.Client.Wpf.Views;

public partial class SignalsPage : Page
{
    public SignalsPage(SignalsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
