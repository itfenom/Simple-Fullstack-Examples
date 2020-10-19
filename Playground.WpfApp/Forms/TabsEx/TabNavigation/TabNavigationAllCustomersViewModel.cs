using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.TabsEx.TabNavigation
{
    public class TabNavigationAllCustomersViewModel : Tab
    {
        private readonly TabNavigationCustomerRepository _repository;

        private ICollection<ITab> _tabs;

        ReadOnlyCollection<CommandViewModel> _commands;
        /// <summary>
        /// Returns a read-only list of commands 
        /// that the UI can display and execute.
        /// </summary>
        public ReadOnlyCollection<CommandViewModel> Commands
        {
            get
            {
                if (_commands == null)
                {
                    var cmds = CreateCommands();
                    _commands = new ReadOnlyCollection<CommandViewModel>(cmds);
                }
                return _commands;
            }
        }

        List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel>
            {
                new CommandViewModel("Delete", new DelegateCommand(() => OnDeleteCustomer())),
                new CommandViewModel("Edit", new DelegateCommand(() => OnEditCustomer()))
            };
        }

        private void OnDeleteCustomer()
        {
            var customer = AllCustomers.FirstOrDefault(c => c.IsSelected);
            if (customer != null)
            {
                _repository.DeleteCustomer(customer.Id);

                AllCustomers.Remove(customer);
                NotifyPropertyChanged("AllCustomers");
                NotifyPropertyChanged("CustomerCount");
            }

        }

        private void OnEditCustomer()
        {
            var customerToEdit = AllCustomers.FirstOrDefault(c => c.IsSelected);
            if (customerToEdit != null)
            {

                var customer = new TabNavigationCustomer
                {
                    IsNew = false,
                    Id = customerToEdit.Id,
                    FirstName = customerToEdit.FirstName,
                    LastName = customerToEdit.LastName,
                    Age = customerToEdit.Age,
                    Email = customerToEdit.Email
                };

                var addNewCustomerVm = new TabNavigationCustomerViewModel(customer, _repository);
                var tabAlreadyOpenedForEdit = false;
                foreach (var item in _tabs)
                {
                    if(item is TabNavigationCustomerViewModel)
                    {
                       if(item.Name.Contains(customer.Id.ToString()))
                        {
                            tabAlreadyOpenedForEdit = true;
                            break;
                        }
                    }
                }

                if(tabAlreadyOpenedForEdit)
                {
                    MessageBox.Show($"Customer '{customer.FirstName}, {customer.LastName}' already opened for edit!", "Edit", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                
                addNewCustomerVm.CloseTabOnSave += CloseTabOnEdit;
                _tabs.Add(addNewCustomerVm);
            }
        }

        private void CloseTabOnEdit(object sender, EventArgs e)
        {
            var tabToBeRemoved = ((ITab)sender);
            _tabs.Remove(tabToBeRemoved);
            var itemToDispose = (TabNavigationCustomerViewModel)tabToBeRemoved;
            itemToDispose.Dispose();
        }

        public TabNavigationAllCustomersViewModel(TabNavigationCustomerRepository repository, ICollection<ITab> tabs)
        {
            _tabs = tabs;
            Name = "All Customers";
            _repository = repository;

            // Subscribe for notifications of when a new customer is saved.
            _repository.CustomerAdded += OnCustomerAddedToRepository;

            //Subscribe for notification of when a customer is edited
            _repository.CustomerEdited += OnCustomerEditedToRepository;

            // Populate the AllCustomers collection with CustomerViewModels.
            CreateAllCustomers();
        }

        private void CreateAllCustomers()
        {
            var allCustomers = _repository.GetCustomers();
            var all = (from cust in allCustomers select new TabNavigationCustomerViewModel(cust, _repository)).ToList();

            foreach (TabNavigationCustomerViewModel cvm in all)
                cvm.PropertyChanged += OnCustomerViewModelPropertyChanged;

            AllCustomers = new ObservableCollection<TabNavigationCustomerViewModel>(all);
            AllCustomers.CollectionChanged += OnCollectionChanged;
        }

        public ObservableCollection<TabNavigationCustomerViewModel> AllCustomers { get; private set; }

        public int CustomerCount => AllCustomers.Count;

        #region Event-Handlers

        private void OnCustomerAddedToRepository(object sender, CustomerAddedEventArgs e)
        {
            var viewModel = new TabNavigationCustomerViewModel(e.NewCustomer, _repository);
            AllCustomers.Add(viewModel);
        }

        private void OnCustomerEditedToRepository(object sender, CustomerAddedEventArgs e)
        {
            var viewModel = new TabNavigationCustomerViewModel(e.NewCustomer, _repository);
            var customerToEdit = AllCustomers.FirstOrDefault(c => c.Id == viewModel.Id);
            if (customerToEdit != null)
            {
                customerToEdit.FirstName = viewModel.FirstName;
                customerToEdit.LastName = viewModel.LastName;
                customerToEdit.Age = viewModel.Age;
                customerToEdit.Email = viewModel.Email;
            }
        }

        private void OnCustomerViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is TabNavigationCustomerViewModel customer)
            {
                customer.ValidateProperty(customer.IsSelected, $"IsSelected");
            }

            if (e.PropertyName == "IsSelected")
            {
                NotifyPropertyChanged("CustomerCount");
            }

            /*
            string IsSelected = "IsSelected";

            // Make sure that the property name we're referencing is valid.
            // This is a debugging technique, and does not execute in a Release build.
            // ReSharper disable once PossibleNullReferenceException
            (sender as TabNavigationCustomerViewModel).ValidateProperty("", IsSelected);

            // When a customer is selected or unselected, we must let the
            // world know that the CustomerCount property has changed,
            // so that it will be queried again for a new value.
            if (e.PropertyName == IsSelected)
                NotifyPropertyChanged("CustomerCount");
                */
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (TabNavigationCustomerViewModel custVm in e.NewItems)
                    custVm.PropertyChanged += OnCustomerViewModelPropertyChanged;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (TabNavigationCustomerViewModel custVm in e.OldItems)
                    custVm.PropertyChanged -= OnCustomerViewModelPropertyChanged;
        }

        #endregion Event-Handlers

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            foreach (var custVm in AllCustomers)
                custVm.Dispose();

            AllCustomers.Clear();
            AllCustomers.CollectionChanged -= OnCollectionChanged;

            _repository.CustomerAdded -= OnCustomerAddedToRepository;
            _repository.CustomerEdited -= OnCustomerEditedToRepository;

            base.Dispose(disposing);
        }

        #endregion Dispose
    }
}