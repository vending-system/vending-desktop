using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VendingSystemClient.Services;
using VendingSystemClient.ViewModels;

namespace VendingSystemClient.Views;

public partial class LoginView : UserControl
{
    public LoginView(ApiService api, NavigationService navigation)
    {
        InitializeComponent();
        var vm = new LoginViewModel(api);
        DataContext = vm;
        
        vm.LoginSuccess += (username, role) =>
    {
        navigation.Navigate(new DashboardView(api, navigation, username, role));
    };
       
    }
}