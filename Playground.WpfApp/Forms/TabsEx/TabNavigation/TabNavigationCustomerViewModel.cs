using System;
using System.ComponentModel.DataAnnotations;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Mvvm.AttributedValidation;

namespace Playground.WpfApp.Forms.TabsEx.TabNavigation
{
    public class TabNavigationCustomerViewModel : Tab
    {
        private readonly TabNavigationCustomer _customer;
        private readonly TabNavigationCustomerRepository _repository;
        private bool _isSelected;

        public TabNavigationCustomerViewModel(TabNavigationCustomer customer, TabNavigationCustomerRepository repository)
        {
            _customer = customer;
            _repository = repository;
            _saveCommand = new DelegateCommand(() => Save(), () => CanSave);
            PropertyChanged += TabCustomerViewModel_PropertyChanged;

            //Apply validation for new Customer with empty fields
            if (customer.IsNew)
            {
                ValidateProperty("", $"FirstName");
                ValidateProperty("", $"LastName");
                ValidateProperty("", $"Email");
                ValidateProperty(0, $"Age");
                Name = "New Customer: ";
            }
            else
            {
                Name = $"Editing Customer: {_customer.Id}";
            }
        }

        private void TabCustomerViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsDirty")
            {
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        #region Properties

        public int Id
        {
            get => _customer.Id;
            set
            {
                if (value == _customer.Id) return;

                _customer.Id = value;
                NotifyPropertyChanged("Id");
                ValidateProperty(value);
            }
        }


        [Required(ErrorMessage = "First Name is Required!")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "First Name contains invalid characters.")]
        public string FirstName
        {
            get => _customer.FirstName;
            set
            {
                if (value == _customer.FirstName) return;

                _customer.FirstName = value;
                NotifyPropertyChanged("FirstName");
                ValidateProperty(value);
            }
        }

        [Required(ErrorMessage = "Last Name is Required!")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "Last Name contains invalid characters.")]
        public string LastName
        {
            get => _customer.LastName;
            set
            {
                if (value == _customer.LastName) return;

                _customer.LastName = value;
                NotifyPropertyChanged("LastName");
                ValidateProperty(value);
            }
        }

        [Required(ErrorMessage = "Age is Required!")]
        [Range(1, 100, ErrorMessage = "Age should be between 1 to 100.")]
        public int Age
        {
            get => _customer.Age;
            set
            {
                if (value == _customer.Age) return;

                _customer.Age = value;
                NotifyPropertyChanged("Age");
                ValidateProperty(value);
            }
        }

        [Required(ErrorMessage = "Email is Required!")]
        [EmailAddress(ErrorMessage = "Email Address is Invalid.")]
        public string Email
        {
            get => _customer.Email;
            set
            {
                if (value == _customer.Email) return;

                _customer.Email = value;
                NotifyPropertyChanged("Email");
                ValidateProperty(value);
            }
        }

        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(_customer.FirstName) && !string.IsNullOrEmpty(_customer.LastName))
                {
                    return _customer.FirstName + " " + _customer.LastName;
                }

                return "";
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value == _isSelected) return;

                SetPropertyValue(ref _isSelected, value);
            }
        }

        #endregion Properties

        #region Save Commands/Methods

        public event EventHandler CloseTabOnSave;

        private DelegateCommand _saveCommand;

        public DelegateCommand SaveCommand => _saveCommand;

        private bool IsNewCustomer => !_repository.ContainsCustomer(_customer);

        private bool IsEditing => _repository.ContainsCustomer(_customer);

        private bool CanSave
        {
            get
            {
                NotifyPropertyChanged("HasErrors"); //Call NotifyPropertyChanged on "HasErrors" for displaying/hiding tooltip
                NotifyPropertyChanged("AllErrors"); //Call NotifyPropertyChanged on "AllErrors" to update errors in tooltip
                //return this.ErrorCount == 0 && _customer.IsValid;
                return !HasErrors;
            }
        }

        public void Save()
        {
            if (IsNewCustomer)
            {
                _repository.AddCustomer(_customer);
                CloseTabOnSave?.Invoke(this, EventArgs.Empty);
            }

            if (IsEditing)
            {
                ValidateProperty(_customer.FirstName, $"FirstName");
                ValidateProperty(_customer.LastName, $"LastName");
                ValidateProperty(_customer.Email, $"Email");
                ValidateProperty(_customer.Age, $"Age");

                _repository.EditCustomer(_customer);
                CloseTabOnSave?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion Save Commands/Methods

        protected override void Dispose(bool disposing)
        {
            PropertyChanged -= TabCustomerViewModel_PropertyChanged;
        }
    }
}