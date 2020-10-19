using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Repositories;

namespace Playground.WpfApp.Forms.TreeViewEx.TreeViewSelectItem
{
    public class CityNode : TreeViewItemVm, IWorld
    {
        private readonly City _city;

        public CityNode(City city, CountryNode country)
            : base(country, true)
        {
            _city = city;
        }

        public string Type => "City";

        public string Name => _city.CityName;

        public string ToolTipText => $"{Type} - ({Name})";
    }
}