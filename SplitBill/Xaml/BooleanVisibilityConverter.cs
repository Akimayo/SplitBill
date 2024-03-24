using Microsoft.UI.Xaml.Data;

namespace SplitBill.Xaml;
internal class BooleanVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool b) return b ? Visibility.Visible : Visibility.Collapsed;
        else throw new ArgumentException("Convert only accepts values of type bool", nameof(value));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is Visibility v) return v == Visibility.Visible;
        else throw new ArgumentException("ConvertBack only accepts values of type Visibility", nameof(value));
    }
}
