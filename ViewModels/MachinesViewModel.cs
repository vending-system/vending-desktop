using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VendingSystemClient.Models;
using VendingSystemClient.Services;
using VendingSystemClient.Views;

namespace VendingSystemClient.ViewModels;

public partial class MachinesViewModel(ApiService api) : ObservableObject
{
  private readonly ApiService _api = api;

    private List<Machine> _allMachines = [];

    [ObservableProperty] private int _page = 1;
    [ObservableProperty] private int _limit = 20;
    [ObservableProperty] private int _total = 0;
    [ObservableProperty] private bool _isTableView = true;
    [ObservableProperty] private Machine? _selectedMachine;
    [ObservableProperty] private string _pageInfo = "";
    [ObservableProperty] private string _totalInfo = "";
    [ObservableProperty] private string _pageRangeInfo = "";
    [ObservableProperty] private int _selectedLimit = 20;
    [ObservableProperty] private bool _isExportPopupOpen = false;

    private string _searchText = "";
    public string SearchText
{
    get => _searchText;
    set
    {
        if (SetProperty(ref _searchText, value))
            ApplyFilter();
    }
}
    public string TableBtnColor => IsTableView ? "#2196F3" : "White";
    public string TableBtnForeground => IsTableView ? "White" : "#2196F3";
    public string TileBtnColor => !IsTableView ? "#2196F3" : "White";
    public string TileBtnForeground => !IsTableView ? "White" : "#2196F3";

    public List<int> LimitOptions { get; } = [10, 20, 50, 100];

    public ObservableCollection<Machine> Machines { get; } = [];

    public bool CanGoPrev => Page > 1;
    public bool CanGoNext => Page * Limit < Total;
    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allMachines
            : _allMachines.Where(m =>
                m.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

        Machines.Clear();
        foreach (var m in filtered)
            Machines.Add(m);

        TotalInfo = $"Всего найдено {filtered.Count} шт.";
        PageRangeInfo = $"Записей с {(Page - 1) * Limit + 1} до {Math.Min(Page * Limit, Total)} из {Total}";
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        var result = await _api.GetMachinesAsync(Page, Limit);
        if (result == null) return;

        _allMachines = result.Data;
        Total = result.Total;

        OnPropertyChanged(nameof(CanGoPrev));
        OnPropertyChanged(nameof(CanGoNext));

        TotalInfo = $"Всего найдено {Total} шт.";
        PageRangeInfo = $"Записей с {(Page - 1) * Limit + 1} до {Math.Min(Page * Limit, Total)} из {Total}";
        PageInfo = $"Страница {Page}";

        ApplyFilter();
    }
    [RelayCommand]
private async Task EditMachineAsync(Machine machine)
{
    var lifetime = (IClassicDesktopStyleApplicationLifetime)
        Application.Current!.ApplicationLifetime!;
    var mainWindow = lifetime.MainWindow!;

    var win = new EditMachineWindow(_api, machine);
    var changed = await win.ShowDialog<bool>(mainWindow);

    if (changed)
        await LoadAsync();
}
    partial void OnSelectedLimitChanged(int value)
    {
        Limit = value;
        Page = 1;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task PrevPageAsync()
    {
        if (!CanGoPrev) return;
        Page--;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task NextPageAsync()
    {
        if (!CanGoNext) return;
        Page++;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteMachineAsync(Machine machine)
    {
        var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        var mainWindow = lifetime.MainWindow!;

        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            "Предупреждение", "Вы точно хотите удалить автомат?",
            ButtonEnum.YesNo, Icon.Question);
        var result = await messageBox.ShowWindowDialogAsync(mainWindow);
        if (result != ButtonResult.Yes) return;

        await _api.DeleteMachineAsync(machine.Id);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task UnlinkModemAsync(Machine machine)
    {
        await _api.UnlinkModemAsync(machine.Id);
        await LoadAsync();
    }

    [RelayCommand]
    private void SwitchToTable()
    {
        IsTableView = true;
        OnPropertyChanged(nameof(TableBtnColor));
        OnPropertyChanged(nameof(TableBtnForeground));
        OnPropertyChanged(nameof(TileBtnColor));
        OnPropertyChanged(nameof(TileBtnForeground));
    }

    [RelayCommand]
    private void SwitchToTile()
    {
        IsTableView = false;
        OnPropertyChanged(nameof(TableBtnColor));
        OnPropertyChanged(nameof(TableBtnForeground));
        OnPropertyChanged(nameof(TileBtnColor));
        OnPropertyChanged(nameof(TileBtnForeground));
    }

    [RelayCommand]
    private void ToggleExportPopup() => IsExportPopupOpen = !IsExportPopupOpen;

    [RelayCommand]
    private void CloseExportPopup() => IsExportPopupOpen = false;

    [RelayCommand]
private async Task ExportToExcelAsync()
{
    IsExportPopupOpen = false;
    var path = await PickSaveFileAsync("Экспорт в Excel", "machines.xlsx",
        new FilePickerFileType("Excel") { Patterns = ["*.xlsx"] });
    if (path == null) return;

    using var workbook = new ClosedXML.Excel.XLWorkbook();
    var ws = workbook.Worksheets.Add("Автоматы");

    ws.Cell(1, 1).Value = "ID";
    ws.Cell(1, 2).Value = "Название";
    ws.Cell(1, 3).Value = "Модель";
    ws.Cell(1, 4).Value = "Компания";
    ws.Cell(1, 5).Value = "Модем";
    ws.Cell(1, 6).Value = "Адрес";
    ws.Cell(1, 7).Value = "В работе с";

    var header = ws.Range(1, 1, 1, 7);
    header.Style.Font.Bold = true;
    header.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#f8f9fa");

    int row = 2;
    foreach (var m in Machines)
    {
        ws.Cell(row, 1).Value = m.Id;
        ws.Cell(row, 2).Value = m.Name;
        ws.Cell(row, 3).Value = m.ModelName;
        ws.Cell(row, 4).Value = m.Company?.Name ?? "";
        ws.Cell(row, 5).Value = m.ModemId;
        ws.Cell(row, 6).Value = m.LocationAddress;
        ws.Cell(row, 7).Value = m.CommissioningDate;
        row++;
    }

    ws.Columns().AdjustToContents();
    workbook.SaveAs(path);
}

    [RelayCommand]
    private async Task ExportToHtmlAsync()
    {
        IsExportPopupOpen = false;
        var path = await PickSaveFileAsync("Экспорт в HTML", "machines.html",
            new FilePickerFileType("HTML") { Patterns = ["*.html"] });
        if (path == null) return;

        var sb = new StringBuilder();
        sb.AppendLine("""
            <!DOCTYPE html>
            <html lang="ru">
            <head>
            <meta charset="UTF-8"/>
            <title>Торговые автоматы</title>
            <style>
              body { font-family: Segoe UI, sans-serif; padding: 24px; background: #f4f6f9; }
              h2 { color: #222; }
              table { border-collapse: collapse; width: 100%; background: white;
                      border-radius: 6px; overflow: hidden; box-shadow: 0 1px 4px #0002; }
              th { background: #f8f9fa; color: #666; font-size: 12px;
                   text-align: left; padding: 10px 14px; border-bottom: 1px solid #e4e4e4; }
              td { padding: 10px 14px; font-size: 13px; border-bottom: 1px solid #f0f0f0; color: #333; }
              tr:last-child td { border-bottom: none; }
            </style>
            </head>
            <body>
            <h2>Торговые автоматы</h2>
            <table>
            <thead><tr>
              <th>ID</th><th>Название</th><th>Модель</th>
              <th>Компания</th><th>Модем</th><th>Адрес</th><th>В работе с</th>
            </tr></thead>
            <tbody>
            """);

        foreach (var m in Machines)
        {
            sb.AppendLine($"""
                <tr>
                  <td>{m.Id}</td>
                  <td>{m.Name}</td>
                  <td>{m.ModelName}</td>
                  <td>{m.Company?.Name}</td>
                  <td>{m.ModemId}</td>
                  <td>{m.LocationAddress}</td>
                  <td>{m.CommissioningDate}</td>
                </tr>
                """);
        }

        sb.AppendLine("</tbody></table></body></html>");
        await File.WriteAllTextAsync(path, sb.ToString(), Encoding.UTF8);
    }

    private static async Task<string?> PickSaveFileAsync(string title, string suggestedName,
        FilePickerFileType fileType)
    {
        var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        var window = lifetime.MainWindow!;

        var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = suggestedName,
            FileTypeChoices = [fileType]
        });

        return file?.Path.LocalPath;
    }
}