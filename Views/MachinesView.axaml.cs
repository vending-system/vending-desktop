using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using VendingSystemClient.Services;
using VendingSystemClient.ViewModels;
using Views;

namespace VendingSystemClient.Views;

public partial class MachinesView : UserControl
{
    private readonly ApiService _api;
    private readonly NavigationService _navigation;
    public MachinesView(ApiService api, NavigationService navigation)
    {
        
            InitializeComponent();
            _api = api;
            _navigation = navigation;
        
        
            var vm = new MachinesViewModel(api);
            DataContext = vm;      
            this.FindControl<Button>("BtnBack")!.Click += (_, _) => navigation.GoBack();
        
        Loaded += async (_, _) => await vm.LoadAsync();
        
        this.FindControl<Button>("BtnAdd")!.Click += (_, _) =>
        _navigation.Navigate(new AddMachineView(_api, _navigation));
    }
    
}