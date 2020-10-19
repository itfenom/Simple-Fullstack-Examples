using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Repositories;

namespace Playground.WpfApp.Forms.DataGridsEx.MultipleDataGrids
{
    public class MultipleDataGridsViewModel : ObservableObject
    {
        public string Title => "CollectionView with Multiple DataGrids";

        private readonly IDialogCoordinator _dialogCoordinator;

        private readonly List<Predicate<DataGridCity>> _cityFilterCriteria;

        private readonly List<DataGridCity> _deletedCity;

        private readonly List<Predicate<DataGridState>> _stateFilterCriteria;

        private readonly List<DataGridState> _deletedState;

        private readonly ObservableCollection<States> _comboBoxStates;

        public ObservableCollection<States> ComboBoxStates => _comboBoxStates;

        private readonly ObservableCollection<Cities> _comboBoxCities;

        public ObservableCollection<Cities> ComboBoxCities => _comboBoxCities;

        public ICommand DisplayChangesCommand
        {
            get { return new DelegateCommand(() => DisplayChanges()); }
        }

        private void DisplayChanges()
        {
            try
            {
                var sb = new StringBuilder();

                if (HasUnsavedChangesInStates() || HasUnsavedChangesInCities())
                {
                    if (DataGridCities != null && DataGridCities.Count > 0)
                    {
                        foreach (var city in DataGridCities)
                        {
                            if (city.IsNew)
                            {
                                sb.AppendLine("New Item:");
                                sb.AppendLine("StateId: " + city.StateId);
                                sb.AppendLine("CityId: " + city.CityId);
                                sb.AppendLine("City: " + city.CityName);
                            }
                            else if (city.IsDirty)
                            {
                                sb.AppendLine("\nUpdated Item:");
                                sb.AppendLine("StateId: " + city.StateId);
                                sb.AppendLine("CityId: " + city.CityId);
                                sb.AppendLine("City: " + city.CityName + "\n");
                            }

                            city.IsNew = false;
                            city.IsDirty = false;
                            city.IsDeleted = false;
                        }
                    }

                    if (_deletedCity.Count > 0)
                    {
                        foreach (var item in _deletedCity)
                        {
                            sb.AppendLine("\nDeleted Item:");
                            sb.AppendLine("StateId: " + item.StateId);
                            sb.AppendLine("CityId: " + item.CityId);
                            sb.AppendLine("City: " + item.CityName + "\n");
                        }
                    }
                }

                if (sb.Length > 0)
                {
                    _dialogCoordinator.ShowMessageAsync(this, "Display Changes", sb.ToString());
                }
                else
                {
                    _dialogCoordinator.ShowMessageAsync(this, "Display Changes", "No Change(s) to display!");
                }
            }
            catch (Exception oEx)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Display Changes", oEx.Message);
            }
        }

        public MultipleDataGridsViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
            
            _cityNameFilter = string.Empty;
            _stateNameFilter = string.Empty;

            _cityFilterCriteria = new List<Predicate<DataGridCity>>();
            _deletedCity = new List<DataGridCity>();

            _stateFilterCriteria = new List<Predicate<DataGridState>>();
            _deletedState = new List<DataGridState>();

            //Load data for comboBoxes
            _comboBoxStates = new ObservableCollection<States>(StateRepository.GetAllStates());
            _comboBoxCities = new ObservableCollection<Cities>(StateRepository.GetAllCities());

            //Initialize Commands
            ViewStateRowDetailCommand = new DelegateCommand(() => ViewStateRowDetail());
            _addNewStateCommand = new DelegateCommand(() => AddNewState(), () => CanAddNewState);
            _deleteStateCommand = new DelegateCommand(() => DeleteState(), () => CanDeleteState);

            _addNewCityCommand = new DelegateCommand(() => AddNewCity(), () => CanAddNewCity);
            _deleteCityCommand = new DelegateCommand(() => DeleteCity(), () => CanDeleteCity);

            //load initial data for States DataGrid
            var statesForDataGrid = StateRepository.GetStatesForDataGrid();
            var dataGridStateList = new List<DataGridState>();

            foreach (var item in statesForDataGrid)
            {
                // ReSharper disable once InconsistentNaming
                var _item = new DataGridState();
                _item.StateId = item.StateId;
                _item.StateName = item.Name;
                _item.Description = item.Description;

                _item.PropertyChanged += DataGridState_PropertyChanged;

                dataGridStateList.Add(_item);
            }

            _dataGridStates = new ObservableCollection<DataGridState>(dataGridStateList);
            DataGridStatesView = (CollectionView)new CollectionViewSource { Source = _dataGridStates }.View;
            NotifyPropertyChanged("DataGridStatesView");

            DataGridStatesView.CurrentChanging += DataGridStatesView_CurrentChanging;
            DataGridStatesView.CurrentChanged += DataGridStatesView_CurrentChanged;

            DataGridStates.CollectionChanged += DataGridStates_CollectionChanged;

            //Set first item manually!
            var firstItem = _dataGridStates[0];
            DataGridStatesView.MoveCurrentTo(firstItem);

            //set selected Item to firstItem
            SelectedDataGridState = firstItem;
        }

        //---------------------------States

        #region DataGridState: Properties

        private ObservableCollection<DataGridState> _dataGridStates;

        public ObservableCollection<DataGridState> DataGridStates
        {
            get => _dataGridStates;
            set => SetPropertyValue(ref _dataGridStates, value);
        }

        private DataGridState _selectedDataGridState;

        public DataGridState SelectedDataGridState
        {
            get => _selectedDataGridState;
            set
            {
                if (_selectedDataGridState == value)
                {
                    return;
                }

                SetPropertyValue(ref _selectedDataGridState, value);

                if (value == null)
                {
                    LoadCities(0);
                }
                else
                {
                    LoadCities(value.StateId);
                }

                _addNewCityCommand.RaiseCanExecuteChanged();
            }
        }

        public CollectionView DataGridStatesView { get; set; }

        private string _stateNameFilter;

        public string StateNameFilter
        {
            get => _stateNameFilter;
            set
            {
                if (_stateNameFilter == value)
                {
                    return;
                }

                SetPropertyValue(ref _stateNameFilter, value);

                ApplyStateFilters();
            }
        }

        #endregion DataGridState: Properties

        #region Event-Handlers: DataGridStatesView

        private void DataGridStatesView_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            //Check if there were any unsaved changes in Cities that must be saved before loading/selecting another state
            if (HasUnsavedChangesInCities())
            {
                if (DataGridStates.Count == 0) return;

                var result = MessageBox.Show("There are unsaved changes in Cities for the selected State.\nDiscard changes and continue?", "Select State", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void DataGridStatesView_CurrentChanged(object sender, EventArgs e)
        {
            SelectedDataGridState = (DataGridState)DataGridStatesView.CurrentItem;
        }

        #endregion Event-Handlers: DataGridStatesView

        #region Event-Handlers: DataGridState

        private void DataGridState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsDirty")
            {
                _addNewStateCommand.RaiseCanExecuteChanged();
                _deleteStateCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == "StateId")
            {
                var state = sender as DataGridState;

                // ReSharper disable once PossibleNullReferenceException
                if (state.StateId != 0)
                {
                    state.StateName = (from s in ComboBoxStates where s.StateId == state.StateId select s.Name).FirstOrDefault();
                }

                if (DataGridCities != null && DataGridCities.Count > 0)
                {
                    foreach (var item in DataGridCities)
                    {
                        item.StateId = state.StateId;
                        item.IsDirty = true;
                    }

                    _addNewCityCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private void DataGridStates_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
            }

            RefreshStatesView();
        }

        #endregion Event-Handlers: DataGridState

        #region DataGridStates: Methods/Filtering

        private void ApplyStateFilters()
        {
            if (DataGridStatesView == null) return;

            if (string.IsNullOrEmpty(StateNameFilter))
            {
                RefreshStatesView();
                return;
            }

            try
            {
                _stateFilterCriteria.Clear();

                _stateFilterCriteria.Add(x => x.StateName != null && x.StateName.ToLower().Contains(StateNameFilter.ToLower()));

                DataGridStatesView.Filter = State_Filter;
                NotifyPropertyChanged("DataGridStatesView");

                RefreshCitiesView();
            }
            catch (Exception oEx)
            {
                if (DataGridStatesView is IEditableCollectionView editableCollectionView)
                {
                    if (editableCollectionView.IsAddingNew)
                    {
                        editableCollectionView.CommitNew();
                    }

                    if (editableCollectionView.IsEditingItem)
                    {
                        editableCollectionView.CommitEdit();
                    }

                    DataGridStatesView.Filter = State_Filter;
                    NotifyPropertyChanged("DataGridStatesView");
                }
                else
                {
                    _dialogCoordinator.ShowMessageAsync(this, "Filter: State", oEx.Message);
                }
            }
        }

        private bool State_Filter(object item)
        {
            if (_stateFilterCriteria.Count == 0)
                return true;

            var state = item as DataGridState;
            return _stateFilterCriteria.TrueForAll(x => x(state));
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private object RefreshStatesView()
        {
            try
            {
                DataGridStatesView.Refresh();
                NotifyPropertyChanged("DataGridStatesView");
            }
            catch (Exception oEx)
            {
                if (DataGridStatesView is IEditableCollectionView editableCollectionView)
                {
                    if (editableCollectionView.IsAddingNew)
                    {
                        editableCollectionView.CommitNew();
                    }

                    if (editableCollectionView.IsEditingItem)
                    {
                        editableCollectionView.CommitEdit();
                    }

                    DataGridStatesView.Filter = State_Filter;
                    NotifyPropertyChanged("DataGridStatesView");
                }
                else
                {
                    _dialogCoordinator.ShowMessageAsync(this, "Filter: State", oEx.Message);
                }
            }
            return null;
        }

        private bool HasUnsavedChangesInStates()
        {
            var isDirtyObjects = (from s in DataGridStates where s.IsDirty select s).ToList();

            if (isDirtyObjects.Any()) return true;

            if (_deletedState.Count > 0) return true;

            return false;
        }

        #endregion DataGridStates: Methods/Filtering

        #region DataGridStates: ICommands/Button Clicks

        private readonly DelegateCommand _addNewStateCommand;

        public DelegateCommand AddNewStateCommand => _addNewStateCommand;

        private bool CanAddNewState => true;

        private void AddNewState()
        {
            //Prevent adding new rows in States if there were Changes in Cities that have not been saved.
            if (DataGridCities != null)
            {
                bool citiesHasChanges = false;

                foreach (var item in DataGridCities)
                {
                    if (item.IsDeleted || item.IsDirty || item.IsNew)
                    {
                        citiesHasChanges = true;
                        break;
                    }
                }

                if (citiesHasChanges)
                {
                    _dialogCoordinator.ShowMessageAsync(this, "Add new State", "There are unsaved changes in Cities for the current Selected State.\n\nSave changes first before adding new State!");
                    return;
                }
            }

            var newState = new DataGridState();
            newState.StateId = 0;
            newState.StateName = string.Empty;

            newState.IsDirty = true;
            newState.IsNew = true;
            newState.PropertyChanged += DataGridState_PropertyChanged;

            SelectedDataGridState = newState;

            if (DataGridStates == null || DataGridStates.Count == 0)
            {
                var dataGridStateList = new List<DataGridState>();
                dataGridStateList.Add(newState);

                _dataGridStates = new ObservableCollection<DataGridState>(dataGridStateList);
                DataGridStatesView = (CollectionView)new CollectionViewSource { Source = _dataGridStates }.View;
                NotifyPropertyChanged("DataGridStatesView");

                DataGridStatesView.CurrentChanging += DataGridStatesView_CurrentChanging;
                DataGridStatesView.CurrentChanged += DataGridStatesView_CurrentChanged;

                DataGridStates.CollectionChanged += DataGridStates_CollectionChanged;
            }
            else
            {
                DataGridStates.Add(newState);
            }

            //Set selected State to new state!
            SelectedDataGridState = newState;

            //Add new City
            //AddNewCity();

            DataGridStatesView.MoveCurrentToLast();
        }

        private readonly DelegateCommand _deleteStateCommand;

        public DelegateCommand DeleteStateCommand => _deleteStateCommand;

        private bool CanDeleteState => (DataGridStates != null && SelectedDataGridState != null);

        private void DeleteState()
        {
            //Delete all Cities associated with State being deleted.
            if (DataGridCities != null)
            {
                var citiesToDelete = (from c in DataGridCities where c.StateId == SelectedDataGridState.StateId select c).ToList();

                foreach (var city in citiesToDelete)
                {
                    if (!_deletedCity.Contains(city))
                    {
                        _deletedCity.Add(city);
                    }

                    city.IsDeleted = true;
                    city.IsDirty = true;
                    DataGridCities.Remove(city);
                    NotifyPropertyChanged("DataGridCities");
                    NotifyPropertyChanged("DataGridCitiesView");
                }
            }

            if (!_deletedState.Contains(SelectedDataGridState))
            {
                _deletedState.Add(SelectedDataGridState);
            }

            SelectedDataGridState.IsDeleted = true;
            SelectedDataGridState.IsDirty = true;
            DataGridStates.Remove(SelectedDataGridState);
            NotifyPropertyChanged("DataGridStates");
            NotifyPropertyChanged("DataGridStatesView");
        }

        public void DoubleClickStateDataGrid()
        {
            if (SelectedDataGridState == null) return;
            _dialogCoordinator.ShowMessageAsync(this, "Double-click on State DataGrid", "Selected State: " + SelectedDataGridState.StateName);
        }

        public ICommand ViewStateRowDetailCommand { get; }

        private void ViewStateRowDetail()
        {
            if (SelectedDataGridState == null) return;

            _dialogCoordinator.ShowMessageAsync(this, "View Detail", "Selected State: " + SelectedDataGridState.StateName);
        }

        #endregion DataGridStates: ICommands/Button Clicks

        //---------------------------Cities

        #region DataGridCities: Properties

        private ObservableCollection<DataGridCity> _dataGridCities;

        public ObservableCollection<DataGridCity> DataGridCities
        {
            get => _dataGridCities;
            set => SetPropertyValue(ref _dataGridCities, value);
        }

        private DataGridCity _selectedDataGridCity;

        public DataGridCity SelectedDataGridCity
        {
            get => _selectedDataGridCity;
            set
            {
                if (_selectedDataGridCity == value)
                {
                    return;
                }

                SetPropertyValue(ref _selectedDataGridCity, value);
            }
        }

        public CollectionView DataGridCitiesView { get; set; }

        private string _cityNameFilter;

        public string CityNameFilter
        {
            get => _cityNameFilter;
            set
            {
                if (_cityNameFilter == value)
                {
                    return;
                }

                SetPropertyValue(ref _cityNameFilter, value);

                ApplyCityFilters();
            }
        }

        #endregion DataGridCities: Properties

        #region DataGridCities: Methods/Filtering

        private void LoadCities(int stateId)
        {
            var cities = StateRepository.GetCitiesForState(stateId);

            if (cities.Count == 0)
            {
                //ClearCityFilters();
                _dataGridCities = null;
                DataGridCitiesView = null;
                SelectedDataGridCity = null;
                return;
            }

            DataGridCitiesView = null;
            var citiesColl = new List<DataGridCity>();
            foreach (var item in cities)
            {
                // ReSharper disable once InconsistentNaming
                var _item = new DataGridCity();
                _item.CityId = item.CityId;
                _item.CityName = item.Name;
                _item.StateId = item.StateId;

                _item.PropertyChanged += DataGridCity_PropertyChanged;

                citiesColl.Add(_item);
            }

            _dataGridCities = new ObservableCollection<DataGridCity>(citiesColl);
            DataGridCitiesView = (CollectionView)new CollectionViewSource { Source = _dataGridCities }.View;
            NotifyPropertyChanged("DataGridCitiesView");

            DataGridCities.CollectionChanged += DataGridCities_CollectionChanged;
        }

        private void DataGridCities_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshCitiesView();
        }

        private void DataGridCity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsDirty")
            {
                _addNewCityCommand.RaiseCanExecuteChanged();
                _deleteCityCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == "PropertyId")
            {
                if (SelectedDataGridState != null)
                {
                    //Set it's State's IsDirty to true
                    SelectedDataGridState.IsDirty = true;
                }
            }
        }

        private void ApplyCityFilters()
        {
            if (DataGridCitiesView == null) return;

            if (string.IsNullOrEmpty(CityNameFilter))
            {
                RefreshCitiesView();
                return;
            }

            try
            {
                _cityFilterCriteria.Clear();

                _cityFilterCriteria.Add(x => x.CityName != null && x.CityName.ToLower().Contains(CityNameFilter.ToLower()));

                DataGridCitiesView.Filter = City_Filter;
                NotifyPropertyChanged("DataGridCitiesView");

                RefreshCitiesView();
            }
            catch (Exception oEx)
            {
                if (DataGridCitiesView is IEditableCollectionView editableCollectionView)
                {
                    if (editableCollectionView.IsAddingNew)
                    {
                        editableCollectionView.CommitNew();
                    }

                    if (editableCollectionView.IsEditingItem)
                    {
                        editableCollectionView.CommitEdit();
                    }

                    DataGridCitiesView.Filter = City_Filter;
                    NotifyPropertyChanged("DataGridCitiesView");
                }
                else
                {
                    MessageBox.Show(oEx.Message, "Filter: City", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private bool City_Filter(object item)
        {
            var city = item as DataGridCity;

            if (_cityFilterCriteria.Count == 0)
            {
                return true;
            }

            return _cityFilterCriteria.TrueForAll(x => x(city));

        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private object RefreshCitiesView()
        {
            if (DataGridCitiesView == null) return null;

            try
            {
                DataGridCitiesView.Refresh();
                NotifyPropertyChanged("DataGridCitiesView");
            }
            catch (Exception oEx)
            {
                if (DataGridCitiesView is IEditableCollectionView editableCollectionView)
                {
                    if (editableCollectionView.IsAddingNew)
                    {
                        editableCollectionView.CommitNew();
                    }

                    if (editableCollectionView.IsEditingItem)
                    {
                        editableCollectionView.CommitEdit();
                    }

                    DataGridCitiesView.Filter = City_Filter;
                    NotifyPropertyChanged("DataGridCitiesView");
                }
                else
                {
                    MessageBox.Show(oEx.Message, "Filter: City", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            return null;
        }

        private bool HasUnsavedChangesInCities()
        {
            if (_deletedCity.Count > 0) return true;

            if (DataGridCities == null) return false;

            var isDirtyObjects = (from c in DataGridCities where c.IsDirty select c).ToList();

            if (isDirtyObjects.Any()) return true;

            return false;
        }

        #endregion DataGridCities: Methods/Filtering

        #region DataGridCities: ICommands/Button Clicks

        private readonly DelegateCommand _addNewCityCommand;

        public DelegateCommand AddNewCityCommand => _addNewCityCommand;

        private bool CanAddNewCity => (SelectedDataGridState != null);

        private void AddNewCity()
        {
            var newCity = new DataGridCity();
            newCity.CityId = 0;
            newCity.CityName = string.Empty;
            newCity.StateId = SelectedDataGridState.StateId;

            newCity.IsNew = true;
            newCity.IsDirty = true;
            newCity.PropertyChanged += DataGridCity_PropertyChanged;

            if (DataGridCities == null)
            {
                var citiesColl = new List<DataGridCity>();
                citiesColl.Add(newCity);

                _dataGridCities = new ObservableCollection<DataGridCity>(citiesColl);
                DataGridCitiesView = (CollectionView)new CollectionViewSource { Source = _dataGridCities }.View;
                NotifyPropertyChanged("DataGridCitiesView");

                DataGridCities.CollectionChanged += DataGridCities_CollectionChanged;
            }
            else
            {
                DataGridCities.Add(newCity);
            }

            SelectedDataGridCity = newCity;

            //Set its' State IsDirty to true
            SelectedDataGridState.IsDirty = true;
        }

        private readonly DelegateCommand _deleteCityCommand;

        public DelegateCommand DeleteCityCommand => _deleteCityCommand;

        private bool CanDeleteCity => (SelectedDataGridState != null && SelectedDataGridCity != null);

        private void DeleteCity()
        {
            if (!_deletedCity.Contains(SelectedDataGridCity))
            {
                _deletedCity.Remove(SelectedDataGridCity);
            }

            SelectedDataGridCity.IsDeleted = true;
            SelectedDataGridCity.IsDirty = true;
            DataGridCities.Remove(SelectedDataGridCity);
            NotifyPropertyChanged("DataGridCitiesView");

            //Set its' State IsDirty to true
            SelectedDataGridState.IsDirty = true;
        }

        #endregion DataGridCities: ICommands/Button Clicks
    }
}