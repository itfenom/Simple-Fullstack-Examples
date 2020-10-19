using System.ComponentModel.DataAnnotations;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Mvvm.AttributedValidation;

namespace Playground.WpfApp.Forms.TabsEx.TabNavigation
{
    public class TabNavigationCustomer : ValidationPropertyChangedBase
    {
        #region Creation

        // ReSharper disable once EmptyConstructor
        public TabNavigationCustomer()
        {
        }

        public static TabNavigationCustomer CreateNewCustomer()
        {
            return new TabNavigationCustomer();
        }

        public static TabNavigationCustomer CreateCustomer(string firstName, string lastName, int age, string email)
        {
            return new TabNavigationCustomer
            {
                FirstName = firstName,
                LastName = lastName,
                Age = age,
                Email = email
            };
        }

        #endregion Creation

        #region Properties with Validation

        private int _id;

        public int Id
        {
            get => _id;
            set => SetPropertyValue(ref _id, value);
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;

                SetPropertyValue(ref _isSelected, value);
            }
        }


        private string _firstName;

        [Required(ErrorMessage = "First Name is Required!")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "First Name contains invalid characters.")]
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (value == _firstName) return;
                SetPropertyValue(ref _firstName, value);
                ValidateProperty(value);
            }
        }

        private string _lastName;

        [Required(ErrorMessage = "Last Name is Required!")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "Last Name contains invalid characters.")]
        public string LastName
        {
            get => _lastName;
            set
            {
                if (value == _lastName) return;
                SetPropertyValue(ref _lastName, value);
                ValidateProperty(value);
            }
        }

        private int _age;

        [Required(ErrorMessage = "Age is Required!")]
        [Range(1, 100, ErrorMessage = "Age should be between 1 to 100.")]
        public int Age
        {
            get => _age;
            set
            {
                if (value == _age) return;

                SetPropertyValue(ref _age, value);
                ValidateProperty(value);
            }
        }

        private string _email;

        [Required(ErrorMessage = "Email is Required!")]
        [EmailAddress(ErrorMessage = "Email Address is Invalid.")]
        public string Email
        {
            get => _email;
            set
            {
                if (value == _email) return;
                SetPropertyValue(ref _email, value);
                ValidateProperty(value);
            }
        }

        #endregion Properties with Validation
    }
}