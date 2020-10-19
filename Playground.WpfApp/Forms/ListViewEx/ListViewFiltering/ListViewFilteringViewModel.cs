using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Repositories;

namespace Playground.WpfApp.Forms.ListViewEx.ListViewFiltering
{
    public class ListViewFilteringViewModel : PropertyChangedBase
    {
        public override string Title => "Filtering and caching with ListView";

        private readonly IDemoEmpRepository _repository;

        public ListViewFilteringViewModel()
        {
            _repository = new DemoEmpRepository();
            _selectedItems = new ObservableCollection<string>();
            SelectedItems = new ObservableCollection<string>(_selectedItems);
            NotifyPropertyChanged("SelectedItems");

            RefreshDataCommand = new DelegateCommand(() => RefreshData());
            AddSelectedItemCommand = new DelegateCommand<object>(param => AddSelectedItem(param));
            RemoveSelectedItemCommand = new DelegateCommand<object>(param => RemoveSelectedItem(param));
            ClearSelectionCommand = new DelegateCommand(() => ClearSelection());

            RefreshData();
        }

        #region Buttons/Commands

        public ICommand RefreshDataCommand { get; }

        private void RefreshData()
        {
            if (IsCacheValid)
                ResetFiltersToDefaults();
            else
                LoadData();
        }

        private void LoadData()
        {
            //Clear ListBox
            _personCatalog = new ObservableCollection<DemoEmployeeModel>();
            NotifyPropertyChanged("PersonCatalog");

            var catalogTask = GetData();
            catalogTask.ContinueWith(
                task =>
                {
                    switch (task.Status)
                    {
                        case TaskStatus.RanToCompletion:
                            _fullList = catalogTask.Result;
                            PersonCatalog = new ObservableCollection<DemoEmployeeModel>(_fullList);
                            NotifyPropertyChanged("PersonCatalog");
                            LastUpdateTime = DateTime.Now;
                            break;

                        case TaskStatus.Canceled:
                            MessageBox.Show("Cancellation Requested.", "CANCELLED!");
                            break;

                        case TaskStatus.Faulted:
                            // ReSharper disable once PossibleNullReferenceException
                            foreach (var ex in task.Exception.Flatten().InnerExceptions)
                            {
                                MessageBox.Show(ex.Message, "ERROR!");
                            }
                            break;
                    }
                });
        }

        private async Task<List<DemoEmployeeModel>> GetData()
        {
            await Task.Delay(1000); //wait for 1 second(s)

            return _repository.GetAllEmployees();
        }

        public ICommand AddSelectedItemCommand { get; }

        private void AddSelectedItem(object sender)
        {
            if (sender == null) return;

            var itemToAdd = (DemoEmployeeModel)sender;
            var formattedItem = itemToAdd.FirstName + ", " + itemToAdd.LastName;

            if (!_selectedItems.Contains(formattedItem))
            {
                _selectedItems.Add(formattedItem);
                NotifyPropertyChanged("SelectedItemsList");
            }
        }

        public ICommand RemoveSelectedItemCommand { get; }

        private void RemoveSelectedItem(object sender)
        {
            if (sender == null) return;

            var itemToRemove = sender.ToString();
            if (_selectedItems.Contains(itemToRemove))
            {
                _selectedItems.Remove(itemToRemove);
                NotifyPropertyChanged("SelectedItemsList");
            }
        }

        public ICommand ClearSelectionCommand { get; }

        private void ClearSelection()
        {
            _selectedItems.Clear();
            NotifyPropertyChanged("SelectedItemsList");
        }

        #endregion Buttons/Commands

        #region Properties

        private List<DemoEmployeeModel> _fullList;
        private ObservableCollection<DemoEmployeeModel> _personCatalog;

        public ObservableCollection<DemoEmployeeModel> PersonCatalog
        {
            get => _personCatalog;
            set => SetPropertyValue(ref _personCatalog, value);
        }

        private ObservableCollection<string> _selectedItems;

        public ObservableCollection<string> SelectedItems
        {
            get => _selectedItems;
            set => SetPropertyValue(ref _selectedItems, value);
        }

        private DateTime _lastUpdateTime;

        public DateTime LastUpdateTime
        {
            get => _lastUpdateTime;
            set
            {
                if (_lastUpdateTime == value) return;
                SetPropertyValue(ref _lastUpdateTime, value);
            }
        }

        public bool IsCacheValid => DateTime.Now - LastUpdateTime < TimeSpan.FromSeconds(10);

        #endregion Properties

        #region Filtering

        private bool _include70S;

        public bool Include70S
        {
            get => _include70S;
            set
            {
                if (_include70S == value) return;
                SetPropertyValue(ref _include70S, value);
                RefreshFilter();
            }
        }

        private bool _include80S;

        public bool Include80S
        {
            get => _include80S;
            set
            {
                if (_include80S == value) return;
                SetPropertyValue(ref _include80S, value);
                RefreshFilter();
            }
        }

        private bool _include90S;

        public bool Include90S
        {
            get => _include90S;
            set
            {
                if (_include90S == value) return;
                SetPropertyValue(ref _include90S, value);
                RefreshFilter();
            }
        }

        private bool _include00S;

        public bool Include00S
        {
            get => _include00S;
            set
            {
                if (_include00S == value) return;
                SetPropertyValue(ref _include00S, value);
                RefreshFilter();
            }
        }

        private void RefreshFilter()
        {
            NotifyPropertyChanged("Include70s");
            NotifyPropertyChanged("Include80s");
            NotifyPropertyChanged("Include90s");
            NotifyPropertyChanged("Include00s");

            IEnumerable<DemoEmployeeModel> people = _fullList;
            if (!Include70S)
                people = people.Where(p => p.StartDecade != 1970);
            if (!Include80S)
                people = people.Where(p => p.StartDecade != 1980);
            if (!Include90S)
                people = people.Where(p => p.StartDecade != 1990);
            if (!Include00S)
                people = people.Where(p => p.StartDecade != 2000);

            PersonCatalog = new ObservableCollection<DemoEmployeeModel>(people);
            NotifyPropertyChanged("PersonCatalog");
        }

        private void ResetFiltersToDefaults()
        {
            _include70S = true;
            _include80S = true;
            _include90S = true;
            _include00S = true;
            RefreshFilter();
        }

        #endregion Filtering

        protected override void DisposeManagedResources()
        {
            _fullList = null;
            _personCatalog = null;
            _selectedItems = null;
        }
    }
}