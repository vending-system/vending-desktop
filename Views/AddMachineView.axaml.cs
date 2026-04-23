using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using VendingSystemClient.Models;
using VendingSystemClient.Services;
using VendingSystemClient.Views;

namespace Views;

public partial class AddMachineView : UserControl
{
    private readonly ApiService _api;
    private readonly NavigationService _navigation;

    public AddMachineView(ApiService api, NavigationService navigation)
    {
        InitializeComponent();
        _api = api;
        _navigation = navigation;

        this.FindControl<Button>("BtnCancel")!.Click += (_, _) => navigation.GoBack();
        this.FindControl<Button>("BtnCreate")!.Click += async (_, _) => await CreateAsync();
    }

    private async Task CreateAsync()
    {
        var name = this.FindControl<TextBox>("TbName")!.Text ?? "";
        var serial = this.FindControl<TextBox>("TbSerial")!.Text ?? "";
        var inventory = this.FindControl<TextBox>("TbInventory")!.Text ?? "";
        var model = this.FindControl<TextBox>("TbModelName")!.Text ?? "";
        var cbManufacturer = this.FindControl<ComboBox>("CbManufacturer")!;
        var manufacturer = (cbManufacturer.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
        var address = this.FindControl<TextBox>("TbAddress")!.Text ?? "";
        var modemId = this.FindControl<TextBox>("TbModem")!.Text;

        var commDatePicker = this.FindControl<DatePicker>("DpCommissioning")!;
        var manufDatePicker = this.FindControl<DatePicker>("DpManufacture")!;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(serial) ||string.IsNullOrWhiteSpace(inventory) || string.IsNullOrWhiteSpace(address) ||
            commDatePicker.SelectedDate == null ||
            manufDatePicker.SelectedDate == null)
        {
            var box = MessageBoxManager.GetMessageBoxStandard( "Ошибка", "Заполните все обязательные поля (*).", ButtonEnum.Ok, Icon.Warning);
            await box.ShowAsync();
            return;
        }

        var dto = new CreateMachineDto
        {
            Name = name,
            SerialNumber = serial,
            InventoryNumber = inventory,
            ModelName = model,
            Manufacturer = manufacturer,
            Country = "RU",
            StatusId = 1,
            CompanyId = 1,
            LocationAddress = address,
            CommissioningDate = commDatePicker.SelectedDate!.Value.DateTime,
            ManufactureDate = manufDatePicker.SelectedDate!.Value.DateTime,
            ModemId = string.IsNullOrWhiteSpace(modemId) ? null : modemId,
        };

        var result = await _api.CreateMachineAsync(dto);

        if (result == null)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Не удалось создать автомат.", ButtonEnum.Ok, Icon.Error);
            await box.ShowAsync();
            return;
        }
         var box1 = MessageBoxManager.GetMessageBoxStandard("Успех", "Торговый аппарат создан", ButtonEnum.Ok, Icon.Info);
            await box1.ShowAsync();
         _navigation.GoBack();
    }
}