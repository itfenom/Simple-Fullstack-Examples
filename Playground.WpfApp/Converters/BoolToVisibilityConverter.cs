using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Playground.WpfApp.Converters
{
    /// <summary>
    /// An implementation of <see cref="IValueConverter" /> that converts boolean values to
    /// <see cref="Visibility" /> values.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : ConverterMarkupExtension
    {
        public bool IsReversed { get; set; }

        public bool UseHidden { get; set; }

        public BoolToVisibilityConverter()
        {
        }

        public BoolToVisibilityConverter(bool isReversed, bool useHidden)
        {
            IsReversed = isReversed;
            UseHidden = useHidden;
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = System.Convert.ToBoolean(value, CultureInfo.InvariantCulture);

            if (IsReversed)
            {
                val = !val;
            }

            if (val)
            {
                return Visibility.Visible;
            }

            return UseHidden ? Visibility.Hidden : Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility))
            {
                return DependencyProperty.UnsetValue;
            }

            var visibility = (Visibility)value;
            var result = visibility == Visibility.Visible;

            if (IsReversed)
            {
                result = !result;
            }

            return result;
        }
    }
}