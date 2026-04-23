using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using System;
using VendingSystemClient.Services;

namespace VendingSystemClient;

class Program
{
    public static IServiceProvider Services { get; private set; } = null!;

    [STAThread]
    public static void Main(string[] args)
    {
        var collection = new ServiceCollection();
        collection.AddHttpClient<ApiService>();
        collection.AddSingleton<ApiService>();
        Services = collection.BuildServiceProvider();

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
