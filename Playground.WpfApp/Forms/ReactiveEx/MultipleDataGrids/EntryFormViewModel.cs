using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Playground.WpfApp.Forms.DataGridsEx.CheckBoxDataGrid;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Mvvm.AttributedValidation;
using ReactiveUI;

namespace Playground.WpfApp.Forms.ReactiveEx.MultipleDataGrids
{
    public class EntryFormViewModel : ValidatableBindableBase
    {
        public override string Title => "Simple Entry-Form with validation using ReactiveUI";
        private readonly bool _isDataLoading;

        [ValidateObject]
        public EntryFormModel Model { get; }

        public List<VisitedCityModel> AllCities { get; private set; }

        [Required(ErrorMessage = "Current city is required!")]
        private VisitedCityModel _selectedCurrentCity;

        public VisitedCityModel SelectedCurrentCity
        {
            get => _selectedCurrentCity;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedCurrentCity, value);
                if (value != null)
                {
                    Model.CurrentCity = value.CityName;
                }
            }
        }

        #region Foods Properties
        private List<FavoriteFoodModel> _foods = new List<FavoriteFoodModel>();

        private FavoriteFoodModel _selectedFood;

        public FavoriteFoodModel SelectedFood
        {
            get => _selectedFood;
            set => this.RaiseAndSetIfChanged(ref _selectedFood, value);
        }

        private ICollectionView _foodCollectionView;

        public ICollectionView FoodCollectionView
        {
            get
            {
                if (_foodCollectionView == null)
                {
                    _foodCollectionView = CollectionViewSource.GetDefaultView(_foods);
                }

                return _foodCollectionView;
            }
        }

        private string _foodCountLabel;

        public string FoodCountLabel
        {
            get => _foodCountLabel;
            set => this.RaiseAndSetIfChanged(ref _foodCountLabel, value);
        }

        private void SetFoodCountLabel()
        {
            if (_foods == null || !_foods.Any())
            {
                _foodCountLabel = "0 Selected";
            }
            else
            {
                if (_foods[0].FoodName == "*")
                {
                    _foodCountLabel = "*";
                }
                else
                {
                    _foodCountLabel = $"{_foods.Count} Selected";
                }
            }

            this.RaisePropertyChanged(nameof(FoodCountLabel));
        }


        #endregion

        #region Cities Properties
        private List<VisitedCityModel> _cities = new List<VisitedCityModel>();

        private VisitedCityModel _selectedCity;

        public VisitedCityModel SelectedCity
        {
            get => _selectedCity;
            set => this.RaiseAndSetIfChanged(ref _selectedCity, value);
        }

        public ICollectionView CityCollectionView { get; set; }

        private bool Filter_City(VisitedCityModel cityModel)
        {
            if (cityModel == null)
            {
                return true;
            }

            return cityModel.CityName.IndexOf(CityFilter ?? "", StringComparison.OrdinalIgnoreCase) >= 0 &&
                   (!_showCheckedCities || cityModel.IsChecked);
        }

        private string _cityFilter;

        public string CityFilter
        {
            get => _cityFilter;
            set => this.RaiseAndSetIfChanged(ref _cityFilter, value);
        }

        private bool _showCheckedCities;

        public bool ShowCheckedCities
        {
            get => _showCheckedCities;
            set => this.RaiseAndSetIfChanged(ref _showCheckedCities, value);
        }

        private string _cityCountLabel;

        public string CityCountLabel
        {
            get => _cityCountLabel;
            set => this.RaiseAndSetIfChanged(ref _cityCountLabel, value);
        }

        public List<ActionCommand> CityActionCommands { get; private set; }

        public ICommand CityActionCommand
        {
            get { return new DelegateCommand<object>(obj => CityActions_Click(obj), o => true); }
        }

        private void CityActions_Click(object obj)
        {
            if (obj.ToString() == "SelectAllCities")
            {
                if (CityCollectionView != null)
                {
                    foreach (var item in _cities)
                    {
                        item.IsChecked = true;
                    }

                    Model.EditState = EditState.Changed;

                    //select the first Product as selected
                    SelectedCity = null;
                    SelectedCity = _cities[0];
                }
            }
            else if (obj.ToString() == "SelectFilteredCities")
            {
                if (CityCollectionView != null && !string.IsNullOrEmpty(CityFilter))
                {
                    VisitedCityModel filteredModel = null;

                    foreach (var item in CityCollectionView)
                    {
                        var cityModel = (VisitedCityModel)item;
                        if (cityModel != null)
                        {
                            cityModel.IsChecked = true;

                            if (filteredModel == null)
                            {
                                filteredModel = cityModel;
                            }
                        }
                    }

                    Model.EditState = EditState.Changed;

                    //select the first filtered Product as selected
                    SelectedCity = null;
                    SelectedCity = filteredModel;
                }
            }
            else if (obj.ToString() == "ClearCitySelection")
            {
                if (CityCollectionView != null)
                {
                    foreach (var item in _cities)
                    {
                        item.IsChecked = false;
                    }

                    Model.EditState = EditState.Changed;

                    //Clear selected Product
                    SelectedCity = null;
                }
            }
            else if (obj.ToString() == "ClearCityFilters")
            {
                CityFilter = string.Empty;
                this.RaisePropertyChanged(nameof(CityFilter));
            }

            CityCollectionView.Refresh();
            this.RaisePropertyChanged(nameof(CityCollectionView));
            this.RaisePropertyChanged(nameof(SelectedCity));
            SetCityCountLabel();

            ValidateAllErrors();
        }

        private void SetCityCountLabel()
        {
            if (_cities == null || !_cities.Any())
            {
                _cityCountLabel = "0 Selected";
            }
            else
            {
                var checkedCities = _cities.Where(p => p.IsChecked).ToList().Count;

                if (checkedCities == _cities.Count)
                {
                    _cityCountLabel = "*";
                }
                else
                {
                    _cityCountLabel = $"{checkedCities} Selected";
                }
            }

            this.RaisePropertyChanged(nameof(CityCountLabel));
        }
        #endregion

        #region Countries Properties
        private List<VisitedCountryModel> _countries = new List<VisitedCountryModel>();

        private VisitedCountryModel _selectedCountry;

        public VisitedCountryModel SelectedCountry
        {
            get => _selectedCountry;
            set => this.RaiseAndSetIfChanged(ref _selectedCountry, value);
        }

        public ICollectionView CountryCollectionView { get; set; }

        private bool Filter_Country(VisitedCountryModel countryModel)
        {
            if (countryModel == null)
            {
                return true;
            }

            return countryModel.CountryName.IndexOf(CountryFilter ?? "", StringComparison.OrdinalIgnoreCase) >= 0 &&
                   (!ShowCheckedCountries || countryModel.IsChecked);
        }

        private string _countryFilter;

        public string CountryFilter
        {
            get => _countryFilter;
            set => this.RaiseAndSetIfChanged(ref _countryFilter, value);
        }

        private bool _showCheckedCountries;

        public bool ShowCheckedCountries
        {
            get => _showCheckedCountries;
            set => this.RaiseAndSetIfChanged(ref _showCheckedCountries, value);
        }

        private string _countryCountLabel;

        public string CountryCountLabel
        {
            get => _countryCountLabel;
            set => this.RaiseAndSetIfChanged(ref _countryCountLabel, value);
        }

        public List<ActionCommand> CountryActionCommands { get; private set; }

        public ICommand CountryActionCommand
        {
            get { return new DelegateCommand<object>(obj => CountryActions_Click(obj), o => true); }
        }

        private void CountryActions_Click(object obj)
        {
            if (obj.ToString() == "SelectAllCountries")
            {
                if (CountryCollectionView != null)
                {
                    foreach (var item in _countries)
                    {
                        item.IsChecked = true;
                    }

                    Model.EditState = EditState.Changed;

                    //select the first Product as selected
                    SelectedCountry = null;
                    SelectedCountry = _countries[0];
                }
            }
            else if (obj.ToString() == "SelectFilteredCountries")
            {
                if (CountryCollectionView != null && !string.IsNullOrEmpty(CountryFilter))
                {
                    VisitedCountryModel filteredModel = null;

                    foreach (var item in CountryCollectionView)
                    {
                        var countryModel = (VisitedCountryModel)item;
                        if (countryModel != null)
                        {
                            countryModel.IsChecked = true;

                            if (filteredModel == null)
                            {
                                filteredModel = countryModel;
                            }
                        }
                    }

                    Model.EditState = EditState.Changed;

                    //select the first filtered Product as selected
                    SelectedCountry = null;
                    SelectedCountry = filteredModel;
                }
            }
            else if (obj.ToString() == "ClearCountrySelection")
            {
                if (CountryCollectionView != null)
                {
                    foreach (var item in _countries)
                    {
                        item.IsChecked = false;
                    }

                    Model.EditState = EditState.Changed;

                    //Clear selected Product
                    SelectedCountry = null;
                }
            }
            else if (obj.ToString() == "ClearCountryFilters")
            {
                CountryFilter = string.Empty;
                this.RaisePropertyChanged(nameof(CountryFilter));
            }

            CountryCollectionView.Refresh();
            this.RaisePropertyChanged(nameof(CountryCollectionView));
            this.RaisePropertyChanged(nameof(SelectedCountry));
            SetCountryCountLabel();

            ValidateAllErrors();
        }

        private void SetCountryCountLabel()
        {
            if (_countries == null || !_countries.Any())
            {
                _countryCountLabel = "0 Selected";
            }
            else
            {
                var checkedCountries = _countries.Where(p => p.IsChecked).ToList().Count;

                if (checkedCountries == _countries.Count)
                {
                    _countryCountLabel = "*";
                }
                else
                {
                    _countryCountLabel = $"{checkedCountries} Selected";
                }
            }

            this.RaisePropertyChanged(nameof(CountryCountLabel));
        }

        #endregion

        public EntryFormViewModel()
        {
            _isDataLoading = true;
            AllCities = new List<VisitedCityModel>(GetCities());

            LoadCommands();

            Model = new EntryFormModel
            {
                Name = string.Empty,
                CurrentCity = string.Empty,
                EditState = EditState.NotChanged
            };

            #region Favorites Food binding
            _foods.AddRange(GetFoods());
            _foods.ForEach(x => x.WhenAnyValue(l => l.FoodName).Subscribe(_ =>
            {
                Model.Foods = _foods.Select(ll => ll.FoodName).ToList();
                SetFoodCountLabel();

            }).DisposeWith(Disposables.Value));

            this.WhenAnyValue(x => x.SelectedFood).Subscribe(_ =>
            {
                Model.Foods = _foods.Select(l => l.FoodName).ToList();
            })
                .DisposeWith(Disposables.Value);

            if (_foods.Count > 0)
            {
                SelectedFood = _foods[0];
                this.RaisePropertyChanged(nameof(SelectedFood));
            }
            #endregion

            #region Visited Cities binding
            _cities.AddRange(GetCities());
            _cities.ForEach(x => x.WhenAnyValue(p => p.IsChecked).Subscribe(_ =>
            {
                Model.Cities = _cities.Where(pp => pp.IsChecked).Select(pp => pp.CityName).ToList();

                // notify that properties associated with the City have changed
                this.RaisePropertyChanged(nameof(CityFilter));
                this.RaisePropertyChanged(nameof(CityCollectionView));

                SetModelEditState("CITY", x.CityName);
                SetCityCountLabel();

            }).DisposeWith(Disposables.Value));

            CityCollectionView = CollectionViewSource.GetDefaultView(_cities);
            CityCollectionView.Filter += s => Filter_City(s as VisitedCityModel);
            this.RaisePropertyChanged(nameof(CityCollectionView));

            //Wire up City filter
            this.WhenAnyValue(
                    x => x.CityFilter,
                    x => x.ShowCheckedCities)
                .ObserveOnDispatcher()
                .Subscribe(_ => CityCollectionView.Refresh());
            #endregion

            #region Visited Countries binding
            _countries.AddRange(GetCountries());
            _countries.ForEach(x => x.WhenAnyValue(s => s.IsChecked).Subscribe(_ =>
            {
                Model.Countries = _countries.Where(ss => ss.IsChecked).Select(ss => ss.CountryName).ToList();

                // notify that properties associated with the Country have changed
                this.RaisePropertyChanged(nameof(CountryFilter));
                this.RaisePropertyChanged(nameof(CountryCollectionView));
                SetModelEditState("COUNTRY", x.CountryName);
                SetCountryCountLabel();

            }).DisposeWith(Disposables.Value));

            CountryCollectionView = CollectionViewSource.GetDefaultView(_countries);
            CountryCollectionView.Filter += s => Filter_Country(s as VisitedCountryModel);
            this.RaisePropertyChanged(nameof(CountryCollectionView));

            //Wire up Country filter
            this.WhenAnyValue(
                    x => x.CountryFilter,
                    x => x.ShowCheckedCountries)
                .ObserveOnDispatcher()
                .Subscribe(_ => CountryCollectionView.Refresh());
            #endregion

            //Add Food
            AddNewFoodCommand = ReactiveCommand.Create(() =>
            {
                var view = new AddFoodDialogView(_foods);
                view.ShowDialog();

                var viewModel = (AddFoodDialogViewModel)view.DataContext;
                if (viewModel.CloseWindowFlag == true && viewModel.OkToAddFood)
                {
                    var newFood = new FavoriteFoodModel(viewModel.FoodText.Trim()) { CanEdit = true, IsChecked = true };
                    _foods.Add(newFood);
                    Model.EditState = EditState.Changed;
                    Model.Foods = _foods.Select(f => f.FoodName).ToList();
                    SelectedFood = newFood;

                    this.RaisePropertyChanged(nameof(SelectedFood));
                    SetFoodCountLabel();
                    FoodCollectionView.Refresh();
                    this.RaisePropertyChanged(nameof(FoodCollectionView));
                    ValidateAllErrors();
                }


            }).DisposeWith(Disposables.Value);

            //Delete Food
            var canDeleteFood = this.WhenAnyValue(x => x.SelectedFood, (FavoriteFoodModel f) => f != null);
            DeleteFoodCommand = ReactiveCommand.Create(() =>
            {
                _foods.Remove(SelectedFood);

                Model.Foods = _foods.Select(f => f.FoodName).ToList();
                Model.EditState = EditState.Changed;

                SelectedFood = null;
                this.RaisePropertyChanged(nameof(SelectedFood));
                SetFoodCountLabel();
                FoodCollectionView.Refresh();
                this.RaisePropertyChanged(nameof(FoodCollectionView));
                ValidateAllErrors();

            }, canDeleteFood).DisposeWith(Disposables.Value);

            //Ok
            var canExecuteOk = this.WhenAnyValue(
                x => x.HasErrors,
                x => x.SelectedCurrentCity,
                x => x.SelectedFood,
                x => x.SelectedCity,
                x => x.SelectedCountry,
                (hasErr, currCity, food, city, country)
                    =>
                {
                    return !hasErr &&
                           currCity != null &&
                           Model.Foods.Count() > 0 &&
                           Model.Cities.Count() > 0 &&
                           Model.Countries.Count() > 0 &&
                           HasUnsavedChanges();
                });
            OkCommand = ReactiveCommand.Create(() =>
            {
                var msg = new StringBuilder();
                msg.AppendLine($"Name: {Model.Name}");
                msg.AppendLine($"Current City: {Model.CurrentCity}");
                msg.AppendLine($"Fav Foods:\n {string.Join(",", Model.Foods)}");
                msg.AppendLine($"\nVisited Cities:\n {string.Join(",", Model.Cities)}");
                msg.AppendLine($"\nVisited Countries:\n {string.Join(",", Model.Countries)}");
                msg.AppendLine($"\n\nClose window?");

                var result = MessageBox.Show(msg.ToString(), "Entry Form", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    CloseWindowFlag = true;
                }

            }, canExecuteOk).DisposeWith(Disposables.Value);

            //Close/Exit
            CancelCommand = ReactiveCommand.Create(() =>
            {
                CloseWindowFlag = true;
                return Unit.Default;
            });

            Model.BeginEdit();
            Model.ErrorsChanged += Model_ErrorsChanged;
            ValidateAllErrors();
            _isDataLoading = false;
        }

        private List<FavoriteFoodModel> GetFoods()
        {
            return new List<FavoriteFoodModel> { new FavoriteFoodModel("*") { IsChecked = true, CanEdit = true } };
        }

        private List<VisitedCityModel> GetCities()
        {
            var retVal = new List<VisitedCityModel>();
            retVal.Add(new VisitedCityModel("Faisalabad") { CanEdit = true, IsChecked = false });
            retVal.Add(new VisitedCityModel("Lahore") { CanEdit = true, IsChecked = false });
            retVal.Add(new VisitedCityModel("Islamabad") { CanEdit = true, IsChecked = false });
            retVal.Add(new VisitedCityModel("Dallas") { CanEdit = true, IsChecked = false });
            retVal.Add(new VisitedCityModel("Houston") { CanEdit = true, IsChecked = false });
            retVal.Add(new VisitedCityModel("New York") { CanEdit = true, IsChecked = false });
            retVal.Add(new VisitedCityModel("Chicago") { CanEdit = true, IsChecked = false });
            retVal.Add(new VisitedCityModel("Riyadh") { CanEdit = true, IsChecked = false });
            retVal.Add(new VisitedCityModel("Jeddah") { CanEdit = true, IsChecked = false });
            retVal.Add(new VisitedCityModel("Medinah") { CanEdit = true, IsChecked = false });
            retVal.Add(new VisitedCityModel("Makkah") { CanEdit = true, IsChecked = false });
            retVal.Add(new VisitedCityModel("Dubai") { CanEdit = true, IsChecked = false });

            return retVal;
        }

        private List<VisitedCountryModel> GetCountries()
        {
            var retVal = new List<VisitedCountryModel>
            {
                new VisitedCountryModel("Pakistan") {CanEdit = true, IsChecked = false},
                new VisitedCountryModel("USA") {CanEdit = true, IsChecked = false},
                new VisitedCountryModel("KSA") {CanEdit = true, IsChecked = false},
                new VisitedCountryModel("Canada") {CanEdit = true, IsChecked = false},
                new VisitedCountryModel("Dubai") {CanEdit = true, IsChecked = false}
            };

            return retVal;
        }

        private void LoadCommands()
        {
            //City Commands
            var cityCommands = new List<ActionCommand>();
            cityCommands.Add(new ActionCommand { Title = "Select All (*)", Command = CityActionCommand, ParameterText = "SelectAllCities" });
            cityCommands.Add(new ActionCommand { Title = "Select Filtered", Command = CityActionCommand, ParameterText = "SelectFilteredCities" });
            cityCommands.Add(new ActionCommand { Title = "Clear Selection", Command = CityActionCommand, ParameterText = "ClearCitySelection" });
            cityCommands.Add(new ActionCommand { Title = "Clear Filters", Command = CityActionCommand, ParameterText = "ClearCityFilters" });
            CityActionCommands = cityCommands;

            //Country Commands
            var countryCommands = new List<ActionCommand>();
            countryCommands.Add(new ActionCommand { Title = "Select All (*)", Command = CountryActionCommand, ParameterText = "SelectAllCountries" });
            countryCommands.Add(new ActionCommand { Title = "Select Filtered", Command = CountryActionCommand, ParameterText = "SelectFilteredCountries" });
            countryCommands.Add(new ActionCommand { Title = "Clear Selection", Command = CountryActionCommand, ParameterText = "ClearCountrySelection" });
            countryCommands.Add(new ActionCommand { Title = "Clear Filters", Command = CountryActionCommand, ParameterText = "ClearCountryFilters" });
            CountryActionCommands = countryCommands;
        }

        private void Model_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            ValidateProperty(nameof(Model));
        }

        private void ValidateAllErrors()
        {
            ValidateProperty(nameof(Model));
            ValidateProperty(nameof(AllErrors));
            ValidateProperty(nameof(HasErrors));
        }

        private void SetModelEditState(string attributeType, string name)
        {
            if (_isDataLoading)
            {
                return;
            }

            Model.EditState = EditState.Changed;

            if (attributeType == "CITY")
            {
                SelectedCity = null;
                SelectedCity = _cities.FirstOrDefault(s => s.CityName == name);
                this.RaisePropertyChanged(nameof(SelectedCity));
            }
            else if (attributeType == "COUNTRY")
            {
                SelectedCountry = null;
                SelectedCountry = _countries.FirstOrDefault(r => r.CountryName == name);
                this.RaisePropertyChanged(nameof(SelectedCountry));
            }
        }

        #region Commands
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public ReactiveCommand<Unit, Unit> AddNewFoodCommand { get; }

        public ReactiveCommand<Unit, Unit> DeleteFoodCommand { get; }

        public ReactiveCommand<Unit, Unit> OkCommand { get; }

        #endregion

        #region Closing
        private bool? _closeWindowFlag;

        public bool? CloseWindowFlag
        {
            get => _closeWindowFlag;
            set => this.RaiseAndSetIfChanged(ref _closeWindowFlag, value);
        }

        public bool HasUnsavedChanges()
        {
            if (CloseWindowFlag == true) return false;

            //if (_editorType == UdvAssignmentEditorType.Copy || _editorType == UdvAssignmentEditorType.New) return true;

            if (Model.EditState == EditState.Changed) return true;

            if (Model.IsChanged)
            {
                return true;
            }

            return false;
        }

        protected override void DisposeManagedResources()
        {
            Model.ErrorsChanged -= Model_ErrorsChanged;

            base.DisposeManagedResources();
        }

        #endregion
    }

    public sealed class EntryFormModel : EditableBindableBase, IEquatable<EntryFormModel>
    {
        private string _name;
        private string _currentCity;
        private IEnumerable<string> _foods = Enumerable.Empty<string>();
        private IEnumerable<string> _cities = Enumerable.Empty<string>();
        private IEnumerable<string> _countries = Enumerable.Empty<string>();

        [DisplayName("Name")]
        [Required(ErrorMessage = "Name is required!", AllowEmptyStrings = false)]
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        [DisplayName("Current City")]
        [Required(ErrorMessage = "Current City is required!", AllowEmptyStrings = false)]
        public string CurrentCity
        {
            get => _currentCity;
            set => this.RaiseAndSetIfChanged(ref _currentCity, value);
        }

        [Required(ErrorMessage = "Favorite food is required!")]
        [CollectionMinimumLength(length: 1, ErrorMessage = "You must enter at least one favorite food.")]
        public IEnumerable<string> Foods
        {
            get => _foods;
            set => this.RaiseAndSetIfChanged(ref _foods, value);
        }

        [Required(ErrorMessage = "Visited city is required!")]
        [CollectionMinimumLength(length: 1, ErrorMessage = "You must choose at least one visited city.")]
        public IEnumerable<string> Cities
        {
            get => _cities;
            set => this.RaiseAndSetIfChanged(ref _cities, value);
        }

        [Required(ErrorMessage = "Visited country is required!")]
        [CollectionMinimumLength(length: 1, ErrorMessage = "You must choose at least one visited country.")]
        public IEnumerable<string> Countries
        {
            get => _countries;
            set => this.RaiseAndSetIfChanged(ref _countries, value);
        }

        // Gets a value indicating whether or not this instance has changes.
        public override bool IsChanged =>
            base.IsChanged
            && (Memento?.State == null || !Equals(Memento.State));

        /// <summary>
        /// Create the memento representing the objects state.
        /// </summary>
        /// <returns>The memento representing the objects state.</returns>
        protected override Memento CreateMemento()
        {
            return new Memento(new EntryFormModel
            {
                Name = Name,
                CurrentCity = CurrentCity,
                Foods = Foods,
                Cities = Cities,
                Countries = Countries
            });
        }

        /// <summary>
        /// Restore the state of the object from the memento.
        /// </summary>
        /// <param name="memento">The memento to restore state from.</param>
        protected override void RestoreMemento(Memento memento)
        {
            var oldState = memento?.State as EntryFormModel;
            if (oldState == null)
            {
                return;
            }

            Name = oldState.Name;
            CurrentCity = oldState.CurrentCity;
            Foods = oldState.Foods;
            Cities = oldState.Cities;
            Countries = oldState.Countries;
        }

        #region Equality

        public static bool operator !=(EntryFormModel model1, EntryFormModel model2)
        {
            return !(model1 == model2);
        }

        public static bool operator ==(EntryFormModel model1, EntryFormModel model2)
        {
            if (ReferenceEquals(model1, model2))
            {
                return true;
            }

            if (ReferenceEquals(null, model1))
            {
                return false;
            }

            return model1.Equals(model2);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EntryFormModel);
        }

        public bool Equals(EntryFormModel other)
        {
            // is the other item null?
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            // is the other item the same object as this instance?
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // check if all properties are equal
            return
                string.Equals(Name, other.Name)
                && string.Equals(CurrentCity, other.CurrentCity)
                && (Foods == null && other.Foods == null || Foods != null && other.Foods != null && Foods.SequenceEqual(other.Foods))
                && (Cities == null && other.Cities == null || Cities != null && other.Cities != null && Cities.SequenceEqual(other.Cities))
                && (Countries == null && other.Countries == null || Countries != null && other.Countries != null && Countries.SequenceEqual(other.Countries));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Choose large primes to avoid hashing collisions
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, Name) ? Name.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, CurrentCity) ? CurrentCity.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, Foods) ? Foods.Distinct().Aggregate(HashingBase, (x, y) => (x * HashingMultiplier) ^ y.GetHashCode()) : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, Cities) ? Cities.Distinct().Aggregate(HashingBase, (x, y) => (x * HashingMultiplier) ^ y.GetHashCode()) : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, Countries) ? Countries.Distinct().Aggregate(HashingBase, (x, y) => (x * HashingMultiplier) ^ y.GetHashCode()) : 0);
                return hash;
            }
        }

        #endregion
    }

    public class FavoriteFoodModel : BindableBase
    {
        private bool _isChecked;
        private bool _canEdit;

        public FavoriteFoodModel(string foodName)
        {
            FoodName = foodName;
        }

        public string FoodName { get; }

        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }

        public bool CanEdit
        {
            get => _canEdit;
            set => this.RaiseAndSetIfChanged(ref _canEdit, value);
        }
    }

    public class VisitedCityModel : BindableBase
    {
        private bool _isChecked;
        private bool _canEdit;

        public VisitedCityModel(string cityName)
        {
            CityName = cityName;
        }

        public string CityName { get; }

        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }

        public bool CanEdit
        {
            get => _canEdit;
            set => this.RaiseAndSetIfChanged(ref _canEdit, value);
        }

        public override string ToString()
        {
            return CityName;
        }
    }

    public class VisitedCountryModel : BindableBase
    {
        private bool _isChecked;
        private bool _canEdit;

        public VisitedCountryModel(string countryName)
        {
            CountryName = countryName;
        }

        public string CountryName { get; }

        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }

        public bool CanEdit
        {
            get => _canEdit;
            set => this.RaiseAndSetIfChanged(ref _canEdit, value);
        }
    }
}
