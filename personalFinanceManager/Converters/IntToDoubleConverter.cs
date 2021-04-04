using System;
using System.Globalization;
using Xamarin.Forms;

namespace personalFinanceManager.Converters
{
    public class IntToDoubleConverter : IValueConverter
    {
        public IntToDoubleConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double.TryParse(value.ToString(), out double result);

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
