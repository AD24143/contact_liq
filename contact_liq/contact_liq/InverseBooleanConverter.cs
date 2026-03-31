using System;
using System.Globalization;
using System.Windows.Data;

namespace contact_liq
{
    /// <summary>
    /// Converts a boolean value to its inverse.
    /// Used in XAML bindings to invert boolean properties (e.g., IsEnabled when IsReadOnly is true).
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;
            return value;
        }
    }
}
