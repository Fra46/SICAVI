using Microsoft.UI.Xaml.Data;
using System;

namespace SICAVI.WinUI.Converters
{
    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (decimal.TryParse(value?.ToString(), out decimal result))
                return result;

            return 0m;
        }
    }
}