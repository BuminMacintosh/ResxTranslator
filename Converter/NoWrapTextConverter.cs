using System;
using System.Globalization;
using System.Windows.Data;

namespace ResxTranslator.Converter
{
	public class NoWrapTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == value || string.IsNullOrEmpty(value.ToString()))
            {
                return string.Empty;
            }

            return value.ToString().Replace(Environment.NewLine, "¶ ");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
