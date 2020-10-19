using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Mvvm.AttributedValidation;

namespace Playground.WpfApp.Forms.OtherEx.SimpleNavigation
{
    public class NavigationAViewModel : ValidationPropertyChangedBase
    {
        public NavigationAViewModel()
        {
            _updateCommand = new DelegateCommand(() => UpdateCurrentDateTime(), () => CanUpdate);
            PropertyChanged += ViewAViewModel_PropertyChanged;
            ValidateProperty("", $"FirstName");
            ValidateProperty("", $"LastName");
        }

        private void ViewAViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsDirty")
            {
                _updateCommand.RaiseCanExecuteChanged();
            }
        }

        private string _firstName;

        [Required(ErrorMessage = "First Name must not be empty")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "Name contains invalid letters")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "FirstName should be minimum of 3 characters and a maximum of 20 characters long.")]
        [DataType(DataType.Text)]
        public string FirstName
        {
            get => _firstName;
            set
            {
                SetPropertyValue(ref _firstName, value);
                ValidateProperty(value);
            }
        }

        private string _lastName;

        [Required(ErrorMessage = "Last Name must not be empty")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "Name contains invalid letters")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "LastName should be minimum of 3 characters and a maximum of 10 characters long.")]
        [DataType(DataType.Text)]
        public string LastName
        {
            get => _lastName;
            set
            {
                SetPropertyValue(ref _lastName, value);
                ValidateProperty(value);
            }
        }

        private DateTime? _lastUpdated;

        public DateTime? LastUpdated
        {
            get => _lastUpdated;
            set => SetPropertyValue(ref _lastUpdated, value);
        }

        private DelegateCommand _updateCommand;

        public ICommand UpdateCommand => _updateCommand;

        private bool CanUpdate
        {
            get
            {
                NotifyPropertyChanged("HasErrors"); //Call NotifyPropertyChanged on "HasErrors" for displaying/hiding tooltip
                NotifyPropertyChanged("AllErrors"); //Call NotifyPropertyChanged on "AllErrors" to update errors in tooltip
                return !HasErrors;
            }
        }

        private void UpdateCurrentDateTime()
        {
            LastUpdated = DateTime.Now;
        }
    }
}