using Avalonia.Controls;
using VendingSystemClient.Services;
using VendingSystemClient.ViewModels;

namespace VendingSystemClient.Views;

public partial class MonitorView : UserControl
{
    private MonitorViewModel? _vm;

    public MonitorView(ApiService api, NavigationService navigation)
    {
        InitializeComponent();

        _vm = new MonitorViewModel(api);
        DataContext = _vm;

        this.FindControl<Button>("BtnBack")!.Click += (_, _) => navigation.GoBack();

        this.FindControl<Button>("BtnStateAll")!.Click += (_, _) =>
        {
            _vm.FilterAll = true;
            _vm.FilterOk = false;
            _vm.FilterError = false;
            _vm.FilterMaintenance = false;
        };

        this.FindControl<Button>("BtnStateOk")!.Click += (_, _) =>
        {
            _vm.FilterOk = !_vm.FilterOk;
            if (_vm.FilterOk) _vm.FilterAll = false;
            if (!_vm.FilterOk && !_vm.FilterError && !_vm.FilterMaintenance) _vm.FilterAll = true;
        };

        this.FindControl<Button>("BtnStateError")!.Click += (_, _) =>
        {
            _vm.FilterError = !_vm.FilterError;
            if (_vm.FilterError) _vm.FilterAll = false;
            if (!_vm.FilterOk && !_vm.FilterError && !_vm.FilterMaintenance) _vm.FilterAll = true;
        };

        this.FindControl<Button>("BtnStateMaint")!.Click += (_, _) =>
        {
            _vm.FilterMaintenance = !_vm.FilterMaintenance;
            if (_vm.FilterMaintenance) _vm.FilterAll = false;
            if (!_vm.FilterOk && !_vm.FilterError && !_vm.FilterMaintenance) _vm.FilterAll = true;
        };

        this.FindControl<Button>("BtnMdb")!.Click += (_, _) => _vm.FilterMdb = !_vm.FilterMdb;
        this.FindControl<Button>("BtnExe")!.Click += (_, _) => _vm.FilterExe = !_vm.FilterExe;
        this.FindControl<Button>("BtnRh")!.Click  += (_, _) => _vm.FilterRh  = !_vm.FilterRh;
        this.FindControl<Button>("BtnSt")!.Click  += (_, _) => _vm.FilterSt  = !_vm.FilterSt;

        this.FindControl<Button>("BtnExMoney")!.Click   += (_, _) => _vm.FilterMoney    = !_vm.FilterMoney;
        this.FindControl<Button>("BtnExWarn")!.Click    += (_, _) => _vm.FilterWarning  = !_vm.FilterWarning;
        this.FindControl<Button>("BtnExService")!.Click += (_, _) => _vm.FilterService  = !_vm.FilterService;
        this.FindControl<Button>("BtnExDocs")!.Click    += (_, _) => _vm.FilterDocs     = !_vm.FilterDocs;
        this.FindControl<Button>("BtnExPower")!.Click   += (_, _) => _vm.FilterPower    = !_vm.FilterPower;
        this.FindControl<Button>("BtnExCrit")!.Click    += (_, _) => _vm.FilterCritical = !_vm.FilterCritical;
        this.FindControl<Button>("BtnExNoSig")!.Click   += (_, _) => _vm.FilterNoSignal = !_vm.FilterNoSignal;

        Loaded += async (_, _) => await _vm.LoadAsync();
    }
}
