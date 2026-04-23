using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace VendingSystemClient;

public partial class App : Application
{
     public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
{
    LiveCharts.Configure(config =>
        config
            .AddDefaultMappers()
            .AddSkiaSharp()
            .AddLightTheme() 
    );

    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        desktop.MainWindow = new MainWindow();

    base.OnFrameworkInitializationCompleted();
}
}