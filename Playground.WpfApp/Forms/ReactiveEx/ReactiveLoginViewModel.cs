using System;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;

namespace Playground.WpfApp.Forms.ReactiveEx
{
    public class ReactiveLoginViewModel : BindableBase
    {
        public override string Title => "Reactive Login: Enter credentials";

        public bool IsUserAuthenticated { get; set; }

        private readonly ReactiveLogin _login;

        private string _email;
        public string Email
        {
            get => _email;
            set => this.RaiseAndSetIfChanged(ref _email, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        ObservableAsPropertyHelper<bool> _isLoading;

        public bool IsLoading => _isLoading?.Value ?? false;

        ObservableAsPropertyHelper<bool> _isValid;
        public bool IsValid => _isValid?.Value ?? false;

        private string _errMsg;

        public string ErrMsg
        {
            get => _errMsg;
            set => this.RaiseAndSetIfChanged(ref _errMsg, value);
        }

        public ReactiveLoginViewModel()
        {
            _login = new ReactiveLogin();
            Email = "t@t.com";
            Password = "ttt";

            this.WhenAnyValue(e => e.Email, p => p.Password,
                    (emailAddress, password) =>
                        /* Validate our email address */
                        (
                            !string.IsNullOrEmpty(emailAddress)
                            &&
                            Regex.Matches(emailAddress, "^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$").Count == 1
                        )
                        &&
                        /* Validate our password */
                        (
                            !string.IsNullOrEmpty(password)
                            &&
                            password.Length > 2
                        ))
                .ToProperty(this, v => v.IsValid, out _isValid);

            var canExecuteLogin =
                this.WhenAnyValue(x => x.IsLoading, x => x.IsValid,
                        (isLoading, isValid) => !isLoading && isValid)
                    .Do(x => System.Diagnostics.Debug.WriteLine($"Can Login: {x}"));

            canExecuteLogin.Subscribe(r =>
            {
                if (r)
                {
                    ErrMsg = "";
                }
                else
                {
                    ErrMsg = "Invalid credentials entered!";
                }
            });

            LoginCommand = ReactiveCommand.CreateFromTask<string, bool>(_ => DoLogin(), canExecuteLogin);
            LoginCommand.Subscribe(r =>
            {
                if (r)
                {
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.Title == Title)
                        {
                            IsUserAuthenticated = true;
                            window.Close();
                        }
                    }
                }
                else
                {
                    ErrMsg = "Invalid credentials entered!";
                }
            });
            LoginCommand.ThrownExceptions.Subscribe(ex => MessageBox.Show(ex.Message));

            this.WhenAnyObservable(x => x.LoginCommand.IsExecuting)
                .StartWith(false)
                .ToProperty(this, x => x.IsLoading, out _isLoading);

            this.WhenAnyValue(x => x.Email).Subscribe(n => _login.Email = n);
            this.WhenAnyValue(x => x.Password).Subscribe(p => _login.Password = p);
        }

        public ReactiveCommand<string, bool> LoginCommand { get; }

        private async Task<bool> DoLogin()
        {
            return await _login.DoLogin();
        }
    }
}
