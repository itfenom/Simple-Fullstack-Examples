using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Repositories;


namespace Playground.WpfApp.Forms.OtherEx.ThreeStateCheckBox
{
    public class ThreeStateCheckBoxViewModel : PropertyChangedBase
    {
        private const string Asia = "Asia";
        private const string Europe = "Europe";
        private const string NorthAmerica = "North America";

        public override string Title => "Three-State checkBox in MVVM";

        public ObservableCollection<Country> Countries { get; set; }

        public ICommand SelectCountriesCommand { get; }

        public ICommand DeSelectCountriesCommand { get; }

        public ThreeStateCheckBoxViewModel()
        {
            SelectCountriesCommand = new DelegateCommand<string>((continentName) =>
             {
                 SetIsSelectedProperty(continentName, true);
             });

            DeSelectCountriesCommand = new DelegateCommand<string>((continentName) =>
            {
                SetIsSelectedProperty(continentName, false);
            });

            Countries = new ObservableCollection<Country>();

            //subscribe to event handler to trigger property changed whenever an item is added/removed to this collection
            Countries.CollectionChanged += Countries_CollectionChanged;

            foreach (var item in WorldRepository.GetCountries(Asia))
            {
                Countries.Add(item);
            }

            foreach (var item in WorldRepository.GetCountries(Europe))
            {
                Countries.Add(item);
            }

            foreach (var item in WorldRepository.GetCountries(NorthAmerica))
            {
                Countries.Add(item);
            }
        }

        private void Countries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (object country in e.NewItems)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    (country as INotifyPropertyChanged).PropertyChanged += item_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (object country in e.OldItems)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    (country as INotifyPropertyChanged).PropertyChanged -= item_PropertyChanged;
                }
            }
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("Countries");
        }

        private void SetIsSelectedProperty(string continentName, bool isSelected)
        {
            IEnumerable<Country> countriesOnTheCurrentContinent =
                    Countries.Where(c => c.ContinentName.Equals(continentName));

            foreach (Country country in countriesOnTheCurrentContinent)
            {
                // ReSharper disable once RedundantCast
                INotifyPropertyChanged c = country as INotifyPropertyChanged;
                c.PropertyChanged -= item_PropertyChanged;
                country.IsSelected = isSelected;
                c.PropertyChanged += item_PropertyChanged;
            }
        }
    }
}