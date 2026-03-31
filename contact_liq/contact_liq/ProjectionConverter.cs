using System;
using System.Globalization;
using System.Windows.Data;

namespace contact_liq
{
    /// <summary>
    /// Projects a value using a format string passed as the converter parameter.
    /// Example usage: ConverterParameter='Name: {0}' will format the bound value into that template.
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class ProjectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string format && !string.IsNullOrEmpty(format))
                return string.Format(culture, format, value);
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ProjectionConverter does not support ConvertBack.");
        }
    }
}
