using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Playground.WpfApp.Converters
{
    public class CursorConverter : MarkupExtension, IValueConverter
    {
        private static CursorConverter instance = new CursorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && ((bool)value))
                return Cursors.Wait;
            else
                return Cursors.Arrow;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return instance;
        }
    }
}