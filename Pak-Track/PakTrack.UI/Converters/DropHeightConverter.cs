using System;
using System.Globalization;
using System.Windows.Data;

namespace PakTrack.UI.Converters
{
    public class DropHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var dropHeight = (double) values[0];
            var isInstantaneous = (bool) values[1];
            return isInstantaneous ? "Instantaneous Shock" : dropHeight.ToString("##0.0000");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}