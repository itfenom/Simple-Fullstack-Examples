using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Repositories;

namespace Playground.WpfApp.Forms.TreeViewEx.TreeViewSelectItem
{
    public class ContinentNode : TreeViewItemVm, IWorld
    {
        private readonly Continent _continent;

        public ContinentNode(Continent continent)
            : base(null, true)
        {
            _continent = continent;
        }

        protected override void LoadChildren()
        {
            var countries = WorldRepository.GetCountries(_continent.ContinentName);

            if (countries == null || countries.Count == 0) return;

            foreach (Country country in countries)
            {
                Children.Add(new CountryNode(country, this));
            }
        }

        public string Type => "Continent";

        public string Name => _continent.ContinentName;

        public string ToolTipText => $"{Type} - ({Name})";
    }
}