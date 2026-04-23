using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using VendingSystemClient.Models;
using VendingSystemClient.Models.DTO;
using VendingSystemClient.Services;

namespace VendingSystemClient.Views;

public partial class EditMachineWindow : Window
{
    private readonly ApiService _api;
    private readonly Machine _machine;

    public EditMachineWindow(ApiService api, Machine machine)
    {
        InitializeComponent();
        _api = api;
        _machine = machine;

        this.FindControl<TextBlock>("TbTitle")!.Text =$"Редактирование: {machine.Name}";
        this.FindControl<TextBox>("TbName")!.Text  = machine.Name;
        this.FindControl<TextBox>("TbManufacturer")!.Text = machine.Manufacturer;
        this.FindControl<TextBox>("TbModelName")!.Text= machine.ModelName;
        this.FindControl<TextBox>("TbSerial")!.Text = machine.SerialNumber;
        this.FindControl<TextBox>("TbInventory")!.Text = machine.InventoryNumber;
        this.FindControl<TextBox>("TbCountry")!.Text = machine.Country;
        this.FindControl<TextBox>("TbAddress")!.Text = machine.LocationAddress;
        this.FindControl<TextBox>("TbModem")!.Text = machine.ModemId ?? "";

        this.FindControl<TextBox>("TbCalibrationInterval")!.Text =  machine.CalibrationIntervalMonths?.ToString() ?? "";
        this.FindControl<TextBox>("TbResourceHours")!.Text = machine.ResourceHoursTotal?.ToString() ?? "";
        this.FindControl<TextBox>("TbServiceHours")!.Text =machine.ServiceTimeHours?.ToString() ?? "";

        if (DateTime.TryParse(machine.ManufactureDate, out var md))this.FindControl<DatePicker>("DpManufacture")!.SelectedDate = md;
        if (DateTime.TryParse(machine.CommissioningDate, out var cd)) this.FindControl<DatePicker>("DpCommissioning")!.SelectedDate = cd;
        if (DateTime.TryParse(machine.LastCalibrationDate, out var ld)) this.FindControl<DatePicker>("DpCalibration")!.SelectedDate = ld;

        this.FindControl<Button>("BtnCancel")!.Click += (_, _) => Close(false);
        this.FindControl<Button>("BtnSave")!.Click   += async (_, _) => await SaveAsync();

        Loaded += async (_, _) => await LoadCompaniesAsync();
    }

    private async Task LoadCompaniesAsync()
    {
        var result = await _api.GetCompaniesAsync(1, 100);
        if (result == null) return;

        var cb = this.FindControl<ComboBox>("CbCompany")!;
        cb.ItemsSource = result.Data;

        var current = result.Data.FirstOrDefault(c => c.Id == _machine.Company?.Id);
        if (current != null)
            cb.SelectedItem = current;
    }

    private async Task SaveAsync()
    {
        var name = this.FindControl<TextBox>("TbName")!.Text ?? "";
        var serial = this.FindControl<TextBox>("TbSerial")!.Text ?? "";
        var inventory = this.FindControl<TextBox>("TbInventory")!.Text ?? "";
        var modelName = this.FindControl<TextBox>("TbModelName")!.Text ?? "";
        var mfr = this.FindControl<TextBox>("TbManufacturer")!.Text ?? "";
        var country = this.FindControl<TextBox>("TbCountry")!.Text ?? "";
        var address = this.FindControl<TextBox>("TbAddress")!.Text ?? "";
        var modem = this.FindControl<TextBox>("TbModem")!.Text;

        var dpMfr = this.FindControl<DatePicker>("DpManufacture")!;
        var dpComm  = this.FindControl<DatePicker>("DpCommissioning")!;
        var dpCalib = this.FindControl<DatePicker>("DpCalibration")!;

        var cb = this.FindControl<ComboBox>("CbCompany")!;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(serial) ||string.IsNullOrWhiteSpace(inventory) || string.IsNullOrWhiteSpace(address) ||
            string.IsNullOrWhiteSpace(modelName) ||
            string.IsNullOrWhiteSpace(mfr) ||
            dpMfr.SelectedDate == null ||
            dpComm.SelectedDate == null ||
            cb.SelectedItem is not CompanyDto selectedCompany)
        {
            var warn = MessageBoxManager.GetMessageBoxStandard( "Ошибка", "Заполните все обязательные поля (*).", ButtonEnum.Ok);
            await warn.ShowAsync();
            return;
        }

        var dto = new CreateMachineDto
        {
            Name = name,
            SerialNumber = serial,
            InventoryNumber = inventory,
            ModelName = modelName,
            Manufacturer= mfr,
            Country = string.IsNullOrWhiteSpace(country) ? "RU" : country,
            StatusId = _machine.Status?.Id ?? 1,
            CompanyId = selectedCompany.Id,
            LocationAddress = address,
            ManufactureDate = dpMfr.SelectedDate!.Value.DateTime,
            CommissioningDate = dpComm.SelectedDate!.Value.DateTime,
            ModemId  = string.IsNullOrWhiteSpace(modem) ? null : modem,
            LastCalibrationDate = dpCalib.SelectedDate.HasValue? dpCalib.SelectedDate.Value.DateTime : null,
            CalibrationIntervalMonths = int.TryParse(this.FindControl<TextBox>("TbCalibrationInterval")!.Text, out var ci) ? ci : null,
            ResourceHoursTotal = int.TryParse(this.FindControl<TextBox>("TbResourceHours")!.Text,out var rh) ? rh : null,
            ServiceTimeHours =int.TryParse(this.FindControl<TextBox>("TbServiceHours")!.Text, out var sh) ? sh : null,
        };

        var success = await _api.UpdateMachineAsync(_machine.Id, dto);

        if (!success)
        {
            var err = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Не удалось сохранить. Проверьте данные.", ButtonEnum.Ok);
            await err.ShowAsync();
            return;
        }

        Close(true);
    }
}