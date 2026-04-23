using Avalonia.Controls;
using Avalonia.Threading;
using VendingSystemClient.Services;
using VendingSystemClient.ViewModels;
using Views;

namespace VendingSystemClient.Views;

public partial class DashboardView : UserControl
{
    private readonly ApiService _api;
    private readonly NavigationService _navigation;

    public DashboardView(ApiService api, NavigationService navigation, string username, string role)
{
    InitializeComponent();
    _api = api;
    _navigation = navigation;

    var vm = new DashboardViewModel(api, username, role);
    DataContext = vm;

    this.FindControl<Button>("BtnHome")!.Click += (_, _) => { };

    this.FindControl<Button>("BtnMachines")!.Click += (_, _) =>
        _navigation.Navigate(new MachinesView(_api, _navigation));

    this.FindControl<Button>("BtnMonitor")!.Click += (_, _) =>
    _navigation.Navigate(new MonitorView(_api, _navigation));
    Loaded += async (_,_) =>
    {
      await vm.LoadDataCommand.ExecuteAsync(null);  
    };

    vm.Logount += () =>
    {
           while(_navigation.CanGoBack)
           _navigation.GoBack();
           _navigation.Navigate(new LoginView(_api,_navigation));
    };
    
    
}
}