using Microsoft.UI.Xaml.Data;

namespace SplitBill.Xaml;
internal class OptionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (parameter == null) throw new ArgumentException("Please provide the desired value", nameof(parameter));
        return (int)value == int.Parse((string)parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (parameter == null) throw new ArgumentException("Please provide the desired value", nameof(parameter));
        if (value is bool v && v) return int.Parse((string)parameter);
        else return null;
    }
}
