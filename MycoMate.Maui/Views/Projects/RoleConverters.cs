using System.Globalization;
using MycoMate.Maui.Models;



namespace MycoMate.Maui.Views.Projects;

public class RoleToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is ProjectRole role ? role switch
        {
            ProjectRole.Owner  => Color.FromArgb("#F9A825"),
            ProjectRole.Editor => Color.FromArgb("#2196F3"),
            _                  => Color.FromArgb("#9E9E9E")
        } : Color.FromArgb("#9E9E9E");

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class RoleToSymbolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is ProjectRole role ? role switch
        {
            ProjectRole.Owner  => "\u2605", // ★
            ProjectRole.Editor => "\u270e", // ✎
            _                  => "\u25ce"  // ◎
        } : "\u25ce";

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class SelectedBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? Color.FromArgb("#E3F2FD") : Colors.Transparent;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
