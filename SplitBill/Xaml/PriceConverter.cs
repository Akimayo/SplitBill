using Microsoft.UI.Xaml.Data;

namespace SplitBill.Xaml;

class PriceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int price) return ((float)price / 100).ToString("0.00");
        else if (value is null) return 0.ToString("0.00"); // For Skia+GTK, which sometimes just gives a null here
        else throw new ArgumentException("Convert only accepts values of type int", nameof(value));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is string price)
        {
            if (float.TryParse(price, out float newPrice)) return (int)(newPrice * 100);
            else if (parameter is int fallbackPrice) return fallbackPrice;
            else return 0;
        }
        else throw new ArgumentException("ConvertBack only accepts values of type string", nameof(value));
    }
}
