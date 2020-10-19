using System.Collections.Generic;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Repositories
{
    public class Continent : PropertyChangedBase
    {
        private string _continentName;

        public string ContinentName
        {
            get => _continentName;
            set => SetPropertyValue(ref _continentName, value);
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetPropertyValue(ref _isSelected, value);
        }
    }

    public class Country : PropertyChangedBase
    {
        private string _continentName;

        public string ContinentName
        {
            get => _continentName;
            set => SetPropertyValue(ref _continentName, value);
        }

        private string _countryName;

        public string CountryName
        {
            get => _countryName;
            set => SetPropertyValue(ref _countryName, value);
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetPropertyValue(ref _isSelected, value);
        }
    }

    public class City : PropertyChangedBase
    {
        private string _countryName;

        public string CountryName
        {
            get => _countryName;
            set => SetPropertyValue(ref _countryName, value);
        }

        private string _cityName;

        public string CityName
        {
            get => _cityName;
            set => SetPropertyValue(ref _cityName, value);
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetPropertyValue(ref _isSelected, value);
        }
    }

    public static class WorldRepository
    {
        public static List<Continent> GetContinents()
        {
            var continents = new List<Continent>()
            {
                new Continent{ContinentName = "North America", IsSelected = false},
                new Continent{ ContinentName = "Asia", IsSelected = false},
                new Continent{ ContinentName = "Europe", IsSelected = false}
            };

            return continents;
        }

        public static List<Country> GetCountries(string continent)
        {
            var countries = new List<Country>();

            if (continent == "North America")
            {
                countries.Add(new Country() { ContinentName = continent, CountryName = "Canada" });
                countries.Add(new Country() { ContinentName = continent, CountryName = "Mexico" });
                countries.Add(new Country() { ContinentName = continent, CountryName = "USA" });
            }
            else if (continent == "Asia")
            {
                countries.Add(new Country() { ContinentName = continent, CountryName = "China" });
                countries.Add(new Country() { ContinentName = continent, CountryName = "Japan" });
                countries.Add(new Country() { ContinentName = continent, CountryName = "Pakistan" });
                countries.Add(new Country() { ContinentName = continent, CountryName = "Malaysia" });
            }
            else if (continent == "Europe")
            {
                countries.Add(new Country() { ContinentName = continent, CountryName = "France" });
                countries.Add(new Country() { ContinentName = continent, CountryName = "Germany" });
                countries.Add(new Country() { ContinentName = continent, CountryName = "Italy" });
            }

            return countries;
        }

        public static List<City> GetCities(string country)
        {
            var cities = new List<City>();

            if (country == "Canada")
            {
                cities.Add(new City { CountryName = country, CityName = "Vancouver" });
            }
            else if (country == "Mexico")
            {
                cities.Add(new City { CountryName = country, CityName = "Mexico City" });
                cities.Add(new City { CountryName = country, CityName = "Tepatitlan" });
            }
            else if (country == "USA")
            {
                cities.Add(new City { CountryName = country, CityName = "Dallas" });
                cities.Add(new City { CountryName = country, CityName = "Houston" });
            }
            else if (country == "China")
            {
                cities.Add(new City { CountryName = country, CityName = "Beijing" });
            }
            else if (country == "Japan")
            {
                //_cities.Add(new City { CountryName = country, CityName = "Tokyo" });
            }
            else if (country == "Pakistan")
            {
                cities.Add(new City { CountryName = country, CityName = "Faisalabad" });
                cities.Add(new City { CountryName = country, CityName = "Islamabad" });
                cities.Add(new City { CountryName = country, CityName = "Lahore" });
            }
            else if (country == "Malaysia")
            {
                cities.Add(new City { CountryName = country, CityName = "Kuala Lumpur" });
            }
            else if (country == "France")
            {
                cities.Add(new City { CountryName = country, CityName = "Paris" });
            }
            else if (country == "Germany")
            {
                cities.Add(new City { CountryName = country, CityName = "Frankfurt" });
            }
            else if (country == "Italy")
            {
                cities.Add(new City { CountryName = country, CityName = "Rome" });
            }

            return cities;
        }
    }
}
