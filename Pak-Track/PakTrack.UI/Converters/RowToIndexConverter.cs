using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace PakTrack.UI.Converters
{
    public class RowToIndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Object item = values[0];
            DataGrid grdPassedGrid = values[1] as DataGrid;
            int rowNumber = grdPassedGrid.Items.IndexOf(item) + 1;
            return rowNumber.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
           // throw new NotImplementedException();
           return null;
        }
    }
}