using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VendingSystemClient.Models;
using VendingSystemClient.Models.DTO;
using VendingSystemClient.Services;

namespace VendingSystemClient.ViewModels;

public partial class MonitorMachineRow : ObservableObject
{
    [ObservableProperty] private int _id;
    [ObservableProperty] private string _name = "";
    [ObservableProperty] private string _modelName = "";
    [ObservableProperty] private string _address = "";
    [ObservableProperty] private string _companyName = "";
    [ObservableProperty] private string _connectionStatus = "";
    [ObservableProperty] private string _connectionLabel = "";
    [ObservableProperty] private string _modemId = "";
    [ObservableProperty] private string _lastSeenText = "";
    [ObservableProperty] private int _signalStrength = 0; 
    [ObservableProperty] private int _loadPercent = 0;
    [ObservableProperty] private string _loadLabel = "";
    [ObservableProperty] private string _loadTime = "";
    [ObservableProperty] private int _loadCoffee = 0;
    [ObservableProperty] private int _loadSugar = 0;
    [ObservableProperty] private int _loadMilk = 0;
    [ObservableProperty] private int _loadCups = 0;
    [ObservableProperty] private decimal _fullMoney = 0;
    [ObservableProperty] private string _cashText = "";
    [ObservableProperty] private string _cashChangeText = "";
    [ObservableProperty] private string _coinsText = "";
    [ObservableProperty] private bool _changeAvailable = false;
    [ObservableProperty] private string _event1 = "";
    [ObservableProperty] private string _event1Time = "";
    [ObservableProperty] private string _event2 = "";
    [ObservableProperty] private string _event2Time = "";
    [ObservableProperty] private bool _hasMdb = false;
    [ObservableProperty] private bool _hasExe = false;
    [ObservableProperty] private bool _hasSt = false;
    [ObservableProperty] private bool _hasRh = false;
    [ObservableProperty] private bool _hasPower = true;
    [ObservableProperty] private bool _hasTerminal = false;
    [ObservableProperty] private bool _hasCashAcceptor = false;
    [ObservableProperty] private bool _equipmentError = false;
    [ObservableProperty] private bool _statusHasMoney = false;      
    [ObservableProperty] private bool _statusHasWarning = false;   
    [ObservableProperty] private bool _statusNeedService = false;   
    [ObservableProperty] private bool _statusHasDocs = false;       
    [ObservableProperty] private bool _statusPowerIssue = false;    
    [ObservableProperty] private bool _statusHasError = false;      
    [ObservableProperty] private bool _statusOffline = false;       
    [ObservableProperty] private string _infoText = "";
    [ObservableProperty] private List<RealtimeStatus> _statusList = [];
    [ObservableProperty] private string _countersText = "";
    [ObservableProperty] private string _statusColor = "#FFFFFF";
    [ObservableProperty] private string _statusName = "";
    [ObservableProperty] private int _statusId = 1;
    [ObservableProperty] private string _connectionType = ""; 
}

public partial class MonitorViewModel : ObservableObject
{
    private readonly ApiService _api;
    private readonly List<MonitorMachineRow> _allRows = [];

    [ObservableProperty] private bool _filterAll = true;
    [ObservableProperty] private bool _filterOk = false;
    [ObservableProperty] private bool _filterError = false;
    [ObservableProperty] private bool _filterMaintenance = false;

    [ObservableProperty] private bool _filterMdb = false;
    [ObservableProperty] private bool _filterExe = false;
    [ObservableProperty] private bool _filterRh = false;
    [ObservableProperty] private bool _filterSt = false;

    [ObservableProperty] private bool _filterMoney = false;
    [ObservableProperty] private bool _filterWarning = false;
    [ObservableProperty] private bool _filterService = false;
    [ObservableProperty] private bool _filterDocs = false;
    [ObservableProperty] private bool _filterPower = false;
    [ObservableProperty] private bool _filterCritical = false;
    [ObservableProperty] private bool _filterNoSignal = false;
    [ObservableProperty] private string _selectedSort = "По состоянию ТА";
    public ObservableCollection<string> SortOptions { get; } = [
        "По состоянию ТА",
        "По названию",
        "По компании",
        "По адресу",
        "По сумме денег"
    ];
    public ObservableCollection<MonitorMachineRow> Rows { get; } = [];

    [ObservableProperty] private bool _isLoading = false;
    [ObservableProperty] private string _statusBarText = "";
    [ObservableProperty] private string _totalSummary = "";
    [ObservableProperty] private bool _isEmpty = false;

    public MonitorViewModel(ApiService api)
    {
        _api = api;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        _allRows.Clear();

        var result = await _api.GetMachinesAsync(1, 100);

        if (result != null)
        {
            foreach (var m in result.Data)
            {
                var rt = await _api.GetMachineRealtimeAsync(m.Id);
                var row = BuildRow(m, rt);
                _allRows.Add(row);
            }
        }

        ApplyFiltersAndSort();
        StatusBarText = $"Обновлено: {DateTime.Now:HH:mm:ss}";
        IsLoading = false;
    }

    private static MonitorMachineRow BuildRow(Machine m, RealtimeData? rt)
    {
        var row = new MonitorMachineRow
        {
            Id = m.Id,
            Name = m.Name,
            ModelName = m.ModelName,
            Address = m.LocationAddress,
            CompanyName = m.Company?.Name ?? "",
            StatusName = m.Status?.Name ?? "",
            StatusId = m.Status?.Id ?? 1,
            ModemId = m.ModemId ?? "",
        };

        if (rt != null)
        {
            bool isOnline = rt.Connection?.Status == "online";
            row.ConnectionStatus = rt.Connection?.Status ?? "offline";
            row.ConnectionLabel = rt.Connection?.Provider ?? "Нет данных";
            row.SignalStrength = rt.Connection?.Signal ?? 0;
            row.LastSeenText = isOnline ? "онлайн" : (rt.Connection?.LastSeen?.ToString("HH:mm") ?? "—");
            row.StatusOffline = !isOnline;

            row.LoadPercent = rt.Load?.Overall ?? 0;
            row.LoadCoffee = rt.Load?.Coffee ?? 0;
            row.LoadSugar = rt.Load?.Sugar ?? 0;
            row.LoadMilk = rt.Load?.Milk ?? 0;
            row.LoadCups = rt.Load?.Cups ?? 0;
            row.LoadLabel = "общая";
            row.LoadTime = DateTime.Now.ToString("HH:mm");

            row.FullMoney = rt.Cash?.FullMoney ?? 0;
            row.CashText = $"{rt.Cash?.FullMoney ?? 0:F0} р.";
            row.CoinsText = $"{rt.Cash?.Coins ?? 0} монет";
            row.ChangeAvailable = rt.Cash?.ChangeAvailable ?? false;
            row.CashChangeText = row.ChangeAvailable ? "Сдача есть" : "Нет сдачи";

            var statuses = rt.Statuses ?? [];
            row.StatusList = statuses;
            row.EquipmentError = statuses.Any(s => s.Type == "error");
            row.StatusHasWarning = statuses.Any(s => s.Type == "warning");
            row.StatusHasError = statuses.Any(s => s.Type == "error");
            row.StatusNeedService = statuses.Any(s =>
                s.Code.Contains("service", StringComparison.OrdinalIgnoreCase) ||
                s.Message.Contains("обслужи", StringComparison.OrdinalIgnoreCase));
            row.StatusHasMoney = statuses.Any(s =>
                s.Code.Contains("cash", StringComparison.OrdinalIgnoreCase) ||
                s.Code.Contains("money", StringComparison.OrdinalIgnoreCase) ||
                s.Message.Contains("деньг", StringComparison.OrdinalIgnoreCase) ||
                s.Message.Contains("сдача", StringComparison.OrdinalIgnoreCase));
            row.StatusPowerIssue = statuses.Any(s =>
                s.Code.Contains("power", StringComparison.OrdinalIgnoreCase) ||
                s.Message.Contains("питани", StringComparison.OrdinalIgnoreCase));

            row.HasMdb = statuses.Any(s => s.Code.Contains("MDB", StringComparison.OrdinalIgnoreCase))
                         || (rt.Connection?.Provider != null); 
            row.HasExe = statuses.Any(s => s.Code.Contains("EXE", StringComparison.OrdinalIgnoreCase));
            row.HasSt = statuses.Any(s => s.Code.Contains("ST", StringComparison.OrdinalIgnoreCase));
            row.HasRh = statuses.Any(s => s.Code.Contains("РН", StringComparison.OrdinalIgnoreCase)
                                       || s.Code.Contains("RH", StringComparison.OrdinalIgnoreCase));

            if (row.HasMdb) row.ConnectionType = "MDB";
            else if (row.HasExe) row.ConnectionType = "EXE";
            else if (row.HasRh) row.ConnectionType = "РН";
            else if (row.HasSt) row.ConnectionType = "ST";

            var infoStatus = statuses.FirstOrDefault();
            row.InfoText = infoStatus?.Message ?? "Работает в штатном режиме";

            if (rt.Events?.Count > 0)
            {
                row.Event1Time = rt.Events[0].Time;
                row.Event1 = rt.Events[0].Product != null
                    ? $"{rt.Events[0].Product} — {rt.Events[0].Amount} р."
                    : $"{rt.Events[0].Amount} р.";
            }
            if (rt.Events?.Count > 1)
            {
                row.Event2Time = rt.Events[1].Time;
                row.Event2 = rt.Events[1].Product != null
                    ? $"{rt.Events[1].Product} — {rt.Events[1].Amount} р."
                    : $"{rt.Events[1].Amount} р.";
            }

            row.HasPower = true;
        }
        else
        {
            row.ConnectionStatus = "offline";
            row.ConnectionLabel = "Нет данных";
            row.StatusOffline = true;
            row.CashText = "—";
            row.InfoText = "Нет данных";
        }

        row.StatusColor = m.Status?.Id switch
        {
            1 => "#FFFFFF",
            2 => "#FFF3F3",
            3 => "#FFFBF0",
            _ => "#FFFFFF"
        };

        return row;
    }

    private void ApplyFiltersAndSort()
    {
        var filtered = _allRows.AsEnumerable();

        if (!FilterAll)
        {
            filtered = filtered.Where(r =>
                (FilterOk && r.StatusId == 1) ||
                (FilterError && r.StatusId == 2) ||
                (FilterMaintenance && r.StatusId == 3));
        }

        bool anyConn = FilterMdb || FilterExe || FilterRh || FilterSt;
        if (anyConn)
        {
            filtered = filtered.Where(r =>
                (FilterMdb && r.HasMdb) ||
                (FilterExe && r.HasExe) ||
                (FilterRh && r.HasRh) ||
                (FilterSt && r.HasSt));
        }

        bool anyExtra = FilterMoney || FilterWarning || FilterService ||
                        FilterDocs || FilterPower || FilterCritical || FilterNoSignal;
        if (anyExtra)
        {
            filtered = filtered.Where(r =>
                (FilterMoney && r.StatusHasMoney) ||
                (FilterWarning && r.StatusHasWarning) ||
                (FilterService && r.StatusNeedService) ||
                (FilterDocs && r.StatusHasDocs) ||
                (FilterPower && r.StatusPowerIssue) ||
                (FilterCritical && r.StatusHasError) ||
                (FilterNoSignal && r.StatusOffline));
        }

        var sorted = SelectedSort switch
        {
            "По названию" => filtered.OrderBy(r => r.Name),
            "По компании" => filtered.OrderBy(r => r.CompanyName),
            "По адресу" => filtered.OrderBy(r => r.Address),
            "По сумме денег" => filtered.OrderByDescending(r => r.FullMoney),
            _ => filtered.OrderBy(r => r.StatusId)
        };

        var list = sorted.ToList();

        Rows.Clear();
        foreach (var r in list)
            Rows.Add(r);

        int total = _allRows.Count;
        int shown = list.Count;
        int working = list.Count(r => r.StatusId == 1);
        int errors = list.Count(r => r.StatusId == 2);
        int maintenance = list.Count(r => r.StatusId == 3);
        decimal totalMoney = list.Sum(r => r.FullMoney);

        TotalSummary = $"Итого автоматов: {shown} ({working} / {errors} / {maintenance})   " +
                       $"Денег в автоматах: {totalMoney:F2} р.";

        IsEmpty = list.Count == 0;
    }

    [RelayCommand]
    public void ApplyFilters()
    {
        ApplyFiltersAndSort();
    }

    [RelayCommand]
    public void ClearFilters()
    {
        FilterAll = true;
        FilterOk = false;
        FilterError = false;
        FilterMaintenance = false;
        FilterMdb = false;
        FilterExe = false;
        FilterRh = false;
        FilterSt = false;
        FilterMoney = false;
        FilterWarning = false;
        FilterService = false;
        FilterDocs = false;
        FilterPower = false;
        FilterCritical = false;
        FilterNoSignal = false;
        SelectedSort = "По состоянию ТА";
        ApplyFiltersAndSort();
    }

    partial void OnFilterAllChanged(bool value)
    {
        if (value)
        {
            FilterOk = false;
            FilterError = false;
            FilterMaintenance = false;
        }
    }

    partial void OnFilterOkChanged(bool value) { if (value) FilterAll = false; }
    partial void OnFilterErrorChanged(bool value) { if (value) FilterAll = false; }
    partial void OnFilterMaintenanceChanged(bool value) { if (value) FilterAll = false; }

    partial void OnSelectedSortChanged(string value)
    {
        ApplyFiltersAndSort();
    }
}
