using System;
using System.Collections.Generic;
using System.Linq;

namespace Playground.WpfApp.Forms.TabsEx.TabNavigation
{
    public class TabNavigationCustomerRepository
    {
        private readonly List<TabNavigationCustomer> _customers;

        public TabNavigationCustomerRepository()
        {
            _customers = LoadCustomers();
        }

        private static List<TabNavigationCustomer> LoadCustomers()
        {
            var retVal = new List<TabNavigationCustomer>
            {
                new TabNavigationCustomer
                {
                    Id  = 101, FirstName = "Josh", LastName = "Smith", Age = 42, Email = "josh@contoso.com", IsSelected = false
                },
                new TabNavigationCustomer
                {
                    Id  = 102,  FirstName = "Greg", LastName = "Bujak", Age = 24, Email = "greg@contoso.com", IsSelected = false
                },
                new TabNavigationCustomer
                {
                    Id  = 103, FirstName = "Jim", LastName = "Crafton", Age = 25, Email = "crafton@contoso.com", IsSelected = false
                },
                new TabNavigationCustomer
                {
                    Id  = 104, FirstName = "Jordan", LastName = "Nolan", Age = 22, Email = "jordan@contoso.com", IsSelected = false
                },
                new TabNavigationCustomer
                {
                    Id  = 105, FirstName = "Grant", LastName = "Hinkson", Age = 29, Email = "hinkson@contoso.com", IsSelected = false
                },
                new TabNavigationCustomer
                {
                    Id  = 106, FirstName = "Karl", LastName = "Shifflett", Age = 32, Email = "kdawg@contoso.com", IsSelected = false
                },
                new TabNavigationCustomer
                {
                    Id  = 107, FirstName = "Wilfred", LastName = "Walker", Age = 33, Email = "will@contoso.com", IsSelected = false
                },
                new TabNavigationCustomer
                {
                    Id  = 108, FirstName = "Denise", LastName = "McCort", Age = 45, Email = "denise@contoso.com", IsSelected = false
                },
                new TabNavigationCustomer
                {
                    Id  = 109, FirstName = "Jason", LastName = "Phillip", Age = 50, Email = "philip@contoso.com", IsSelected = false
                }
            };

            return retVal;
        }

        public event EventHandler<CustomerAddedEventArgs> CustomerAdded;

        public event EventHandler<CustomerAddedEventArgs> CustomerEdited;

        public void AddCustomer(TabNavigationCustomer customer)
        {
            if (customer == null)
                throw new ArgumentNullException($"customer");

            if (!_customers.Contains(customer))
            {
                var existingIds = (from c in _customers where !string.IsNullOrEmpty(c.FirstName) select c.Id).ToList();
                customer.Id = existingIds.Max() + 1;
                _customers.Add(customer);

                // ReSharper disable once UseNullPropagation
                if (CustomerAdded != null)
                    CustomerAdded(this, new CustomerAddedEventArgs(customer));
            }
        }

        public void EditCustomer(TabNavigationCustomer customer)
        {
            var customerToEdit = _customers.FirstOrDefault(c => c.Id == customer.Id);
            if (customerToEdit != null)
            {
                customerToEdit.FirstName = customer.FirstName;
                customerToEdit.LastName = customer.LastName;
                customerToEdit.Age = customer.Age;
                customerToEdit.Email = customer.Email;

                // ReSharper disable once UseNullPropagation
                if (CustomerEdited != null)
                    CustomerEdited(this, new CustomerAddedEventArgs(customerToEdit));
            }
        }

        public void DeleteCustomer(int customerId)
        {
            var customerToDelete = _customers.FirstOrDefault(c => c.Id == customerId);
            if (customerToDelete != null)
            {
                _customers.Remove(customerToDelete);
            }
        }

        public bool ContainsCustomer(TabNavigationCustomer customer)
        {
            var customerExists = _customers.FirstOrDefault(c => c.Id == customer.Id);
            return customerExists != null;
        }

        public List<TabNavigationCustomer> GetCustomers()
        {
            return new List<TabNavigationCustomer>(_customers);
        }
    }
}