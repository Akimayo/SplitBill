using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace SplitBill.Xaml;
internal class NegateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool v) return !v;
        else throw new ArgumentException("Convert only accepts values of type bool", nameof(value));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is bool v) return !v;
        else throw new ArgumentException("ConvertBack only accepts values of type bool", nameof(value));
    }
}
