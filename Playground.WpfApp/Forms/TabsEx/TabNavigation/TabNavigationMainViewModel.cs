using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.TabsEx.TabNavigation
{
    public class TabNavigationMainViewModel : PropertyChangedBase
    {
        private readonly TabNavigationCustomerRepository _repository;
        private readonly ObservableCollection<ITab> _tabs;
        public ICollection<ITab> Tabs { get; }

        private bool _allCustomersTabLoaded;

        private Tab _selectedTab;

        public Tab SelectedTab
        {
            get => _selectedTab;
            set => SetPropertyValue(ref _selectedTab, value);
        }

        public override string Title => "Navigation with Tab Control";

        public TabNavigationMainViewModel()
        {
            _repository = new TabNavigationCustomerRepository();
            ShowAllCustomersCommand = new DelegateCommand(() => OnShowAllCustomers());
            AddNewCustomerCommand = new DelegateCommand(() => OnAddNewCustomer());
            _allCustomersTabLoaded = false;
            _tabs = new ObservableCollection<ITab>();
            _tabs.CollectionChanged += Tabs_CollectionChanged;
            Tabs = _tabs;
        }

        private void Tabs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ITab tab;
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                tab = (ITab)e.NewItems[0];
                tab.CloseRequested += OnCloseRequested;
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                tab = (ITab)e.OldItems[0];
                tab.CloseRequested -= OnCloseRequested;
            }
        }

        private void OnCloseRequested(object sender, EventArgs e)
        {
            var tabToBeRemoved = ((ITab)sender);

            if (tabToBeRemoved.Name == "All Customers")
            {
                _allCustomersTabLoaded = false;
            }

            Tabs.Remove(tabToBeRemoved);
            if (tabToBeRemoved.GetType().ToString().Contains("TabNavigationAllCustomersViewModel"))
            {
                var itemToDispose = (TabNavigationAllCustomersViewModel)tabToBeRemoved;
                itemToDispose.Dispose();
            }
            else if (tabToBeRemoved.GetType().ToString().Contains("TabNavigationCustomerViewModel"))
            {
                var itemToDispose = (TabNavigationCustomerViewModel)tabToBeRemoved;
                itemToDispose.Dispose();
            }
        }

        public ICommand ShowAllCustomersCommand { get; }

        private void OnShowAllCustomers()
        {
            if (!_allCustomersTabLoaded)
            {
                var allCustomerVm = new TabNavigationAllCustomersViewModel(_repository, Tabs);
                Tabs.Add(allCustomerVm);
                SelectedTab = allCustomerVm;
                _allCustomersTabLoaded = true;
            }
        }

        public ICommand AddNewCustomerCommand { get; }

        private void OnAddNewCustomer()
        {
            var customer = new TabNavigationCustomer();
            customer.IsNew = true;

            var addNewCustomerVm = new TabNavigationCustomerViewModel(customer, _repository);
            Tabs.Add(addNewCustomerVm);
            addNewCustomerVm.CloseTabOnSave += AddNewCustomerVM_CloseTabOnSave;
            SelectedTab = addNewCustomerVm;
        }

        private void AddNewCustomerVM_CloseTabOnSave(object sender, EventArgs e)
        {
            var tabToBeRemoved = ((ITab)sender);
            Tabs.Remove(tabToBeRemoved);
            var itemToDispose = (TabNavigationCustomerViewModel)tabToBeRemoved;
            itemToDispose.Dispose();
        }
    }
}