using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace PakTrack.UI.Converters
{
    public class ShockOrientationConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var faces = value as IEnumerable<int>;
            // faces of the package -> the axis
            var orientation = new StringBuilder("[ ");
            if (faces != null)
                foreach (var face in faces)
                {
                    orientation.Append(face);
                    orientation.Append(" ");
                }
            orientation.Append("]");
            return orientation.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}