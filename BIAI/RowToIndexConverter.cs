using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace BIAI
{
    public class RowToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var row = value as DataGridRow;
            if (row != null) return row.GetIndex();
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}