using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Playground.WpfApp.Repositories;

namespace Playground.WpfApp.Converters
{
    public class CountryCollectionToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<Country> countries = values[0] as IEnumerable<Country>;
            string continentName = values[1] as string;
            if (countries != null && continentName != null)
            {
                IEnumerable<Country> countriesOnTheCurrentContinent
                    = countries.Where(c => c.ContinentName.Equals(continentName));

                // ReSharper disable once ReplaceWithSingleCallToCount
                // ReSharper disable once PossibleMultipleEnumeration
                int selectedCountriesCount = countriesOnTheCurrentContinent
                    .Where(c => c.IsSelected)
                    .Count();

                // ReSharper disable once PossibleMultipleEnumeration
                if (selectedCountriesCount.Equals(countriesOnTheCurrentContinent.Count()))
                    return true;

                if (selectedCountriesCount > 0)
                    return null;
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}