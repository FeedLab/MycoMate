using System.Globalization;

namespace MycoMate.Maui.Views.Projects;

public class IsTypeConverter : IValueConverter
{
    public Type? TargetType { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value?.GetType() == TargetType;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
