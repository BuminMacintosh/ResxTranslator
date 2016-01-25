using System;
using System.Globalization;
using System.Windows.Data;

namespace ResxTranslator.Converter
{
	public class TranslatedValueConverter : IMultiValueConverter
	{
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == values[1] || string.Empty == values[1].ToString() || values[0].ToString() == values[1].ToString())
            {
                return "-";
            }

            return "○";
        }

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
    }
}
