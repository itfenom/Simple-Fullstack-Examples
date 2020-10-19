using System.Windows;

namespace Playground.WpfApp.Forms.Main.Login
{
    public partial class LoginView
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtUserName.Clear();
            TxtUserName.Clear();
            TxtPassword.Focus();
            TxtPassword.SelectAll();

            IRequestFocus focus = (IRequestFocus)DataContext;
            focus.FocusRequested += OnFocusRequested;
        }

        private void OnFocusRequested(object sender, FocusRequestedEventArgs e)
        {
            // ReSharper disable once RedundantAssignment
            // ReSharper disable once LocalNameCapturedOnly
            using (var viewModel = (LoginViewModel) DataContext)
            {
                switch (e.PropertyName)
                {
                    case nameof(viewModel.Password):
                        TxtPassword.Focus();
                        TxtPassword.SelectAll();
                        break;

                    case nameof(viewModel.UserName):
                        TxtUserName.Focus();
                        TxtUserName.SelectAll();
                        break;
                }
            }
        }
    }
}
