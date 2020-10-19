using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Playground.WpfApp.Converters
{
    public class BackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.ToString().ToUpper().Trim() == "MALE")
                {
                    // ReSharper disable once PossibleNullReferenceException
                    var blueColor = (Color)ColorConverter.ConvertFromString("Blue");
                    return new SolidColorBrush((Color)blueColor);
                }

                if (value.ToString().ToUpper().Trim() == "FEMALE")
                {
                    // ReSharper disable once PossibleNullReferenceException
                    var pinkColor = (Color)ColorConverter.ConvertFromString("Pink");
                    return new SolidColorBrush((Color)pinkColor);
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}