using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Repositories;

namespace Playground.WpfApp.Forms.TreeViewEx.TreeViewSelectItem
{
    public class CountryNode : TreeViewItemVm, IWorld
    {
        private readonly Country _country;

        public CountryNode(Country country, ContinentNode continent)
            : base(continent, true)
        {
            _country = country;
        }

        protected override void LoadChildren()
        {
            var cities = WorldRepository.GetCities(_country.CountryName);

            if (cities == null || cities.Count == 0) return;

            foreach (City city in cities)
            {
                Children.Add(new CityNode(city, this));
            }
        }

        /*
        protected override void LoadChildren()
        {
            var _countries = WorldRepository.GetCountries(_continent.ContinentName);

            if (_countries == null || _countries.Count == 0) return;

            foreach (Country _country in _countries)
            {
                base.Children.Add(new CountryNode(_country, this));
            }
        }
         */

        public string Type => "Country";

        public string Name => _country.CountryName;

        public string ToolTipText => $"{Type.ToString()} - ({Name})";
    }
}