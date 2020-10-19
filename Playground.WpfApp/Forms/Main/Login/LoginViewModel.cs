using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Mvvm.AttributedValidation;

namespace Playground.WpfApp.Forms.Main.Login
{
    public class LoginViewModel : ValidationPropertyChangedBase, IRequestFocus
    {
        private readonly string _title;

        public override string Title => _title;

        private string _userName;

        [Required(ErrorMessage = "User name is required!")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "User name contains invalid character(s).")]
        [DataType(DataType.Text)]
        public string UserName
        {
            get => _userName;
            set
            {
                SetPropertyValue(ref _userName, value);
                ValidateProperty(value);
            }
        }

        private string _password;

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Text)]
        public string Password
        {
            get => _password;
            set
            {
                SetPropertyValue(ref _password, value);
                ValidateProperty(value);
            }
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetPropertyValue(ref _errorMessage, value);
        }

        private bool? _dialogResultValue;

        public bool? DialogResultValue
        {
            get => _dialogResultValue;
            set => SetPropertyValue(ref _dialogResultValue, value);
        }

        public LoginViewModel()
        {
            _title = "Login";
            LoginOkCommand = new DelegateCommand(() => Login(), () => (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password) && ErrorCount == 0));
            PropertyChanged += LoginViewModel_PropertyChanged;

            ValidateProperty("", $@"UserName");
            ValidateProperty("", $@"Password");
        }

        private void LoginViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsDirty")
            {
                LoginOkCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand LoginOkCommand { get; }

        private void Login()
        {
            ErrorMessage = string.Empty;
            if (UserName == "kmubarak")
            {
                if (Password == "letmein")
                {
                    DialogResultValue = true;
                }
                else
                {
                    ErrorMessage = "Invalid Password! Please try again.";
                    OnFocusRequested("Password");
                }
            }
            else
            {
                ErrorMessage = "Invalid UserName/Password";
            }
        }

        public ICommand LoginCancelCommand => new DelegateCommand(CloseWindow);

        private void CloseWindow()
        {
            DialogResultValue = true;
        }

        #region IFocusRequest Implementation

        public event EventHandler<FocusRequestedEventArgs> FocusRequested;

        protected virtual void OnFocusRequested(string propertyName)
        {
            FocusRequested?.Invoke(this, new FocusRequestedEventArgs(propertyName));
        }

        #endregion IFocusRequest Implementation

        #region Dispose
        protected override void DisposeManagedResources()
        {
            PropertyChanged -= LoginViewModel_PropertyChanged;
        }
        #endregion
    }
}
