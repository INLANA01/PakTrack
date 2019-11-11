using System;
using System.Globalization;
using System.Windows.Data;


namespace PakTrack.UI.Converters
{
    public class TemperatureUnitConverter : IValueConverter
    {
        /// <summary>
        /// Convert EPOCH timestamp value to regular date
        /// </summary>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double fahrenheit = System.Convert.ToInt64(value);
            double celsius = Math.Round((fahrenheit - 32) * 5 / 9, 4);
            return celsius.ToString();
        }

        /// <summary>
        /// Convert regular date to EPOCH timestamp
        /// </summary>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double celsius = System.Convert.ToInt64(value);
            double fahrenheit = Math.Round((celsius * 9) / 5 + 32, 4);
            return fahrenheit.ToString();
        }
    }
}