using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace SplitBill.Xaml;

class ByteCheckboxConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is byte amt) return amt > 0;
        else if (value is null) return false; // For Skia+GTK, which sometimes gives null here for some reason
        else throw new ArgumentException("Convert only accepts values of type byte", nameof(value));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is bool b) return b ? (byte)1 : (byte)0;
        else if (value is null) return (byte)0; // For Skia+GTK, which sometimes gives null here for some reason
        else throw new ArgumentException("ConvertBack only accepts values of type bool", nameof(value));
    }
}
