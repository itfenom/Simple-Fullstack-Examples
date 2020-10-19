using System;
using System.Windows.Data;

namespace Playground.WpfApp.Converters
{
    public class ColumnWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // ReSharper disable once PossibleNullReferenceException
            var isVisible = (bool)value;
            // ReSharper disable once AssignNullToNotNullAttribute
            var width = double.Parse(parameter as string);
            return isVisible ? 0.0 : width;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}