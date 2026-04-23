using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace VendingSystemClient.ViewModels;

public class StatusColorConverter : IValueConverter
{
    public static readonly StatusColorConverter Instance = new();
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int id)
            return id switch
            {
                1 => new SolidColorBrush(Color.Parse("#4CAF50")),
                2 => new SolidColorBrush(Color.Parse("#f44336")),
                3 => new SolidColorBrush(Color.Parse("#2196F3")),
                _ => new SolidColorBrush(Color.Parse("#9E9E9E"))
            };
        return new SolidColorBrush(Color.Parse("#9E9E9E"));
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class PercentToWidthConverter : IValueConverter
{
    public static readonly PercentToWidthConverter Instance = new();
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int percent)
            return (double)Math.Clamp(percent, 0, 100) / 100.0 * 90.0;
        return 0.0;
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class LoadColorConverter : IValueConverter
{
    public static readonly LoadColorConverter Instance = new();
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int percent)
            return percent switch
            {
                >= 60 => new SolidColorBrush(Color.Parse("#4CAF50")),
                >= 30 => new SolidColorBrush(Color.Parse("#FF9800")),
                _     => new SolidColorBrush(Color.Parse("#f44336"))
            };
        return new SolidColorBrush(Color.Parse("#4CAF50"));
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class BoolToBgConverter : IValueConverter
{
    public static readonly BoolToBgConverter Instance = new();
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? new SolidColorBrush(Color.Parse("#E8F5E9"))
                     : new SolidColorBrush(Color.Parse("#F5F5F5"));
        return new SolidColorBrush(Color.Parse("#F5F5F5"));
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class BoolToBorderConverter : IValueConverter
{
    public static readonly BoolToBorderConverter Instance = new();
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? new SolidColorBrush(Color.Parse("#A5D6A7"))
                     : new SolidColorBrush(Color.Parse("#E0E0E0"));
        return new SolidColorBrush(Color.Parse("#E0E0E0"));
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class BoolToTextConverter : IValueConverter
{
    public static readonly BoolToTextConverter Instance = new();
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? new SolidColorBrush(Color.Parse("#2E7D32"))
                     : new SolidColorBrush(Color.Parse("#9E9E9E"));
        return new SolidColorBrush(Color.Parse("#9E9E9E"));
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
public class ConnectionIconConverter : IValueConverter
{
    public static readonly ConnectionIconConverter Instance = new();
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s)
            return s == "online" ? "📶" : "📵";
        return "📵";
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
public class SignalBarConverter : IValueConverter
{
    public static readonly SignalBarConverter Instance = new();
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        int signal = value is int s ? s : 0;
        int threshold = parameter is string p && int.TryParse(p, out var t) ? t : 100;
        bool active = signal >= threshold;
        return active
            ? new SolidColorBrush(Color.Parse("#4CAF50"))
            : new SolidColorBrush(Color.Parse("#D0D0D0"));
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
