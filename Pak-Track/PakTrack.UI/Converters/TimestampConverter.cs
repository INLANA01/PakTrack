using System;
using System.Globalization;
using System.Windows.Data;


namespace PakTrack.UI.Converters
{
    public class TimestampConverter:IValueConverter
    {
        /// <summary>
        /// Convert EPOCH timestamp value to regular date
        /// </summary>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timestamp = System.Convert.ToInt64(value);
            return new DateTime(timestamp).ToString("MMM dd, yyyy hh:mm:ss tt");
        }

        /// <summary>
        /// Convert regular date to EPOCH timestamp
        /// </summary>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateValue = (DateTime?) value;
            return dateValue?.Ticks;
        }
    }
}