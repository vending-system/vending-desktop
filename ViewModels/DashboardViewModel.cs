using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VendingSystemClient.Models;
using VendingSystemClient.Services;

namespace VendingSystemClient.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly ApiService _api;

    public DashboardViewModel() : this(null!, "designer", "Роль") { }

    [ObservableProperty] private string _userName = "";
    [ObservableProperty] private string _userRole = "";
    [ObservableProperty] private string _efficiencyText = "—";
    [ObservableProperty] private int _workingMachines = 0;
    [ObservableProperty] private int _brokenMachines = 0;
    [ObservableProperty] private int _maintenanceMachines = 0;
    [ObservableProperty] private int _totalMachines = 0;
    [ObservableProperty] private string _totalCash = "—";
    [ObservableProperty] private string _todaySales = "—";
    [ObservableProperty] private string _yesterdaySales = "—";
    [ObservableProperty] private string _totalSales = "—";
    [ObservableProperty] private string _averageCheck = "—";
    [ObservableProperty] private string _serviceToday = "—";
    [ObservableProperty] private bool _showByAmount = true;
    [ObservableProperty] private string _salesDateRange = "";
    [ObservableProperty] private bool _adminMenuOpen = false;
    public ObservableCollection<SalesDynamic> SalesData { get; } = [];
    public ObservableCollection<NewsItem> News { get; } = [];

    public ISeries[] DonutSeries { get; set; } = [];
    public ISeries[] BarSeries { get; set; } = [];
    public Axis[] XAxes { get; set; } = [];
    [ObservableProperty] private bool _profileMenuOpen = false;

    [RelayCommand]
    private void ToggleProfileMenu() => ProfileMenuOpen = !ProfileMenuOpen;
    public event Action? Logount;
    [RelayCommand]
    private void Logout()
    {
        _api.CleanToken();
        ProfileMenuOpen = false;
        Logount?.Invoke();
    }
    
    public Axis[] YAxes { get; set; } =
    [
        new Axis
        {
            MinLimit = 0,
            LabelsPaint = new SolidColorPaint(SKColor.Parse("#888888")),
            TextSize = 10,
            SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#eeeeee")),
            TicksPaint = null
        }
    ];

    public DashboardViewModel(ApiService api, string username, string role)
    {
        _api = api;
        UserName = username;
        UserRole = role;
        
    }

    [RelayCommand]
    private async Task LoadDataAsync()
{
        if (_api == null) return;

        // Статистика сети
        var stats = await _api.GetMashineStatsAsync();
        if (stats != null)
        {
            TotalMachines = stats.TotalMachines;
            WorkingMachines = stats.WorkingMachines;
            BrokenMachines = stats.BrokenMachines;
            MaintenanceMachines = stats.MaintenanceMachines;
            EfficiencyText = $"{stats.EfficiencyPercent:F0}%";
            TotalCash = $"{stats.TotalCash:N0} р.";

            DonutSeries =
    [
        new PieSeries<double>
        {
            Values = [WorkingMachines > 0 ? (double)WorkingMachines : 0.001],
            Name = "Работает",
            Fill = new SolidColorPaint(SKColor.Parse("#4CAF50")),
            InnerRadius = 50,
            Stroke = null
        },
        new PieSeries<double>
        {
            Values = [BrokenMachines > 0 ? (double)BrokenMachines : 0.001],
            Name = "Сломано",
            Fill = new SolidColorPaint(SKColor.Parse("#f44336")),
            InnerRadius = 50,
            Stroke = null
        },
        new PieSeries<double>
        {
            Values = [MaintenanceMachines > 0 ? (double)MaintenanceMachines : 0.001],
            Name = "Обслуживание",
            Fill = new SolidColorPaint(SKColor.Parse("#2196F3")),
            InnerRadius = 50,
            Stroke = null
        }
    ];
            OnPropertyChanged(nameof(DonutSeries));
        }
    YAxes =
    [
        new Axis
        {
            LabelsPaint = new SolidColorPaint(SKColor.Parse("#555555")),
            TextSize = 10,
            SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#eeeeee")) { StrokeThickness = 1 },
            TicksPaint = null
        }
    ];
        var summary = await _api.GetSalesSummaryAsync();
        if (summary != null)
        {
            TodaySales = $"{summary.TodaySales:N0} р.";
            YesterdaySales = $"{summary.YesterdaySales:N0} р.";
            TotalSales = $"{summary.TotalSales:N0} р.";
            AverageCheck = $"{summary.AverageCheck:N0} р.";
            ServiceToday = summary.ServiceToday.ToString();
        }

        SalesDateRange = $"{DateTime.Now.AddDays(-10):dd.MM.yyyy} по {DateTime.Now:dd.MM.yyyy}";

        var dynamics = await _api.GetSalesDynamicsAsync(10);

        SalesData.Clear();
        var maxAmount = dynamics.Count != 0 ? dynamics.Max(d => d.TotalAmount) : 1;
        if (maxAmount == 0) maxAmount = 1;

        foreach (var d in dynamics)
        {
            d.BarHeight = (double)(d.TotalAmount / maxAmount) * 100;
            d.DateStr = d.Date.ToString("dd.MM");
            SalesData.Add(d);
        }

        var labels = dynamics.Select(d => $"{d.DayName}   {d.DateStr}").ToArray();
        var values = dynamics.Select(d => (double)d.TotalAmount).ToArray();

        BarSeries =
        [
            new ColumnSeries<double>
            {
                Values = values,
                Name = "Сумма",
                Fill = new SolidColorPaint(SKColor.Parse("#90CAF9")),
                Stroke = null,
                MaxBarWidth = 40,
                Rx = 4,
                Ry = 4
            }
        ];

    XAxes =
    [
        new Axis
        {
            Labels = labels,
            LabelsPaint = new SolidColorPaint(SKColor.Parse("#555555")),
            TextSize = 10,
            SeparatorsPaint = null,
            TicksPaint = null
        }
    ];

        OnPropertyChanged(nameof(BarSeries));
        OnPropertyChanged(nameof(XAxes));

        News.Clear();
        News.Add(new NewsItem { Date = "29.01.25", Title = "Терминалы KitPos получили лицензию от СФ ФСБ" });
        News.Add(new NewsItem { Date = "31.12.24", Title = "Новогоднее поздравление от KIT Vending / KIT Pro" });
        News.Add(new NewsItem { Date = "28.12.24", Title = "Ставки НДС 5% и 7% для УСН" });
        News.Add(new NewsItem { Date = "04.12.24", Title = "Релиз новой CRM-системы KIT Shop" });
        News.Add(new NewsItem { Date = "27.11.24", Title = "Новые модели снековых автоматов от KIT" });
        News.Add(new NewsItem { Date = "20.11.24", Title = "Получение сертификата PCI DSS 4.0.1" });

        _amountValues = dynamics.Select(d => (double)d.TotalAmount).ToArray();
        _quantityValues = dynamics.Select(d => (double)d.TotalQuantity).ToArray();

    BarSeries =
    [
        new ColumnSeries<double>
        {
            Values = _amountValues,
            Fill = new SolidColorPaint(SKColor.Parse("#90CAF9")),
            Stroke = null,
            MaxBarWidth = 40,
            Rx = 4, Ry = 4
        }
    ];
}


    [RelayCommand]
    private void ToggleAdminMenu() => AdminMenuOpen = !AdminMenuOpen;

    private double[] _amountValues = [];
    private double[] _quantityValues = [];

[RelayCommand]
private void SwitchToAmount()
{
    ShowByAmount = true;
    BarSeries =
    [
        new ColumnSeries<double>
        {
            Values = _amountValues,
            Fill = new SolidColorPaint(SKColor.Parse("#90CAF9")),
            Stroke = null,
            MaxBarWidth = 40,
            Rx = 4, Ry = 4
        }
    ];
    OnPropertyChanged(nameof(BarSeries));
}

[RelayCommand]
private void SwitchToQuantity()
{
    ShowByAmount = false;
    BarSeries =
    [
        new ColumnSeries<double>
        {
            Values = _quantityValues,
            Fill = new SolidColorPaint(SKColor.Parse("#FF9800")),
            Stroke = null,
            MaxBarWidth = 40,
            Rx = 4, Ry = 4
        }
    ];
    OnPropertyChanged(nameof(BarSeries));
}
}