using System;
using System.Windows;
using System.Windows.Data;

namespace Playground.WpfApp.Converters
{
    public class BoolToVisibleOrHidden : IValueConverter
    {

        // ReSharper disable once EmptyConstructor
        public BoolToVisibleOrHidden() { }

        public bool Collapse { get; set; }
        public bool Reverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // ReSharper disable once PossibleNullReferenceException
            bool bValue = (bool)value;

            if (bValue != Reverse)
            {
                return Visibility.Visible;
            }

            if (Collapse)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // ReSharper disable once PossibleNullReferenceException
            Visibility visibility = (Visibility)value;

            if (visibility == Visibility.Visible)
            {
                return !Reverse;
            }

            return Reverse;
        }
    }
}