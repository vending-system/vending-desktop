using Avalonia.Controls;
using VendingSystemClient.Services;
using VendingSystemClient.ViewModels;
using VendingSystemClient.Views;

namespace VendingSystemClient;

public partial class MainWindow : Window
{
    private readonly NavigationService _navigation;
    private readonly ApiService _api;

    public MainWindow()
    {
        InitializeComponent();

        _api = (Program.Services.GetService(typeof(ApiService)) as ApiService)!;
        _navigation = new NavigationService(FrameMain);

        _navigation.Navigate(new LoginView(_api, _navigation));
        
        
    }

}