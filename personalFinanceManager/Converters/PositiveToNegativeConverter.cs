using System;
using System.Globalization;
using Xamarin.Forms;

namespace personalFinanceManager.Converters
{
    public class PositiveToNegativeConverter : IValueConverter
    {
        public PositiveToNegativeConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringAmount = value.ToString();

            bool temp = double.TryParse(stringAmount, out double amountAsDouble);

            if (temp)
            {
                string amountAsString = amountAsDouble.ToString("C2", culture);

                return ("-" + $"{amountAsString}");
            }

            return "-$0.00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
