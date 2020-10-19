using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using Playground.WpfApp.Forms.DataGridsEx.SelectAllDataGrid;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ProgressDialog = Playground.WpfApp.Forms.ReactiveEx.ProgressDialog;

namespace Playground.WpfApp.Forms.Main
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly IDialogCoordinator _dialogCoordinator;

        public MainWindowViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;

            FormatPhoneNumberField("0019722072406");

            //logout at startup
            IsCheckedLogin = false;
            IsEnabledLogin = true;

            IsCheckedLogout = true;
            IsEnabledLogout = false;

            StatusBarContent = "Logged-out: Please login!";

            //Flyout
            FlyoutIsOpen = false;
            AccentColors = ThemeManager.Accents
                .Select(a => new AccentColorMenuData
                {
                    Name = a.Name,
                    ColorBrush = a.Resources["AccentColorBrush"] as Brush
                })
                .ToList();

            var theme = ThemeManager.DetectAppStyle(Application.Current);
            _currentAccentColor = AccentColors.Single(accent => accent.Name == theme.Item2.Name);
            FlyoutOkCommand = new DelegateCommand(() => FlyoutIsOpen = false);

            //Top-Right-Menu
            ChangeThemeCommand = new DelegateCommand(() => FlyoutIsOpen = true);
            LoginCommand = new DelegateCommand(() => OnLogin());
            LogoutCommand = new DelegateCommand(() => Logout());
            ExitCommand = new DelegateCommand(() => OnExit());

            //Async
            ShowAdvAsyncAwaitCommand = new DelegateCommand(() => OnShowAdvAsyncAwait());
            ShowAsyncWithProgressbarCommand = new DelegateCommand(() => OnShowAsyncWithProgressbar());
            ShowBasicAsyncAwaitCommand = new DelegateCommand(() => OnShowBasicAsyncAwait());

            //DataGrids
            ShowNavigationDataGridCommand = new DelegateCommand(() => OnShowNavigationDataGrid());
            ShowCheckBoxDataGridCommand = new DelegateCommand(() => OnShowCheckBoxDataGrid());
            ShowMultipleDataGridsCommand = new DelegateCommand(() => OnShowMultipleDataGrids());
            ShowAccountMgrCommand = new DelegateCommand(() => OnShowAccountMgr());
            ShowSelectAllDataGridCommand = new DelegateCommand(() => OnShowSelectAllDataGrid());

            //TreeViews
            ShowTreeViewSelectedItemCommand = new DelegateCommand(() => OnShowTreeViewSelectedItem());

            //Others
            ShowXamlMiscCommand = new DelegateCommand(() => OnShowXamlMisc());
            ShowDataTemplatingCommand = new DelegateCommand(() => OnShowDataTemplating());
            ShowDataCachingViewCommand = new DelegateCommand(() => OnShowDataCaching());
            ShowBasicDataBindingCommand = new DelegateCommand(() => OnShowBasicDataBinding());
            ShowFileDialogCommand = new DelegateCommand(() => OnShowFileDialog());
            ShowItemsControlEx1Command = new DelegateCommand(() => OnShowItemsControlEx1());
            ShowItemsControlEx2Command = new DelegateCommand(() => OnShowItemsControlEx2());
            ShowItemsControlEx3Command = new DelegateCommand(() => OnShowItemsControlEx3());
            ShowThreeStateCheckBoxCommand = new DelegateCommand(() => OnShowThreeStateCheckBox());
            SimpleNavigationCommand = new DelegateCommand(() => OnSimpleNavigation());

            //Tabs
            ShowDynamicTabsCommand = new DelegateCommand(() => OnShowDynamicTabs());
            SowTabNavigationCommand = new DelegateCommand(() => OnSowTabNavigation());

            //ListViews
            ShowDelegateCommand = new DelegateCommand(() => OnShowDelegate());
            ShowListViewFilteringCommand = new DelegateCommand(() => OnShowListViewFiltering());
            ShowListViewPagingCommand = new DelegateCommand(() => OnShowListViewPaging());

            //Reactive
            ShowSimpleCommand = new DelegateCommand(() => OnShowSimple());
            ShowReactiveGettingStartedCommand = new DelegateCommand(() => OnShowReactiveGettingStarted());
            ShowReactiveLoginCommand = new DelegateCommand(() => OnShowReactiveLogin());
            ShowTeapotsCommand = new DelegateCommand(() => OnShowTeapots());
            ShowOpenDialogCommand = new DelegateCommand(() => OnShowOpenDialog());
            ShowProgressDialogCommand = new DelegateCommand(() => OnShowProgressDialog());
            ShowCrudCommand = new DelegateCommand(() => OnShowCrud());
            ShowCrud2Command = new DelegateCommand(() => OnShowCrud2());
            ShowTodoCommand = new DelegateCommand(() => OnShowTodo());
            ShowDataEntryFormCommand = new DelegateCommand(() => OnShowDataEntryForm());
            ShowTreeAssignmentsCommand = new DelegateCommand(() => OnShowTreeAssignments());
        }

        private void FormatPhoneNumberField(string fullPhoneNumber)
        {
            CountryCode = "+" + fullPhoneNumber.Substring(0, 2);
            AreaCode = " (" + fullPhoneNumber.Substring(2, 1) + ")" + fullPhoneNumber.Substring(3, 3);
            PhoneNumber = " " + fullPhoneNumber.Substring(6);
        }

        #region Properties

        private string _countryCode;

        public string CountryCode
        {
            get => _countryCode;
            set => SetPropertyValue(ref _countryCode, value);
        }

        private string _areaCode;

        public string AreaCode
        {
            get => _areaCode;
            set => SetPropertyValue(ref _areaCode, value);
        }

        private string _phoneNumber;

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetPropertyValue(ref _phoneNumber, value);
        }
        #endregion

        #region Flyout Stuff

        private AccentColorMenuData _currentAccentColor;

        public AccentColorMenuData CurrentAccentColor
        {
            get => _currentAccentColor;
            set
            {
                if (_currentAccentColor != value)
                {
                    _currentAccentColor = value;
                    var theme = ThemeManager.DetectAppStyle(Application.Current);
                    var accent = ThemeManager.GetAccent(_currentAccentColor.Name);
                    ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
                }
            }
        }

        public List<AccentColorMenuData> AccentColors { get; }

        private bool _flyoutIsOpen;

        public bool FlyoutIsOpen
        {
            get => _flyoutIsOpen;
            set => SetPropertyValue(ref _flyoutIsOpen, value);
        }

        public ICommand FlyoutOkCommand { get; }

        #endregion

        #region Top Menu (login/Logout/Exit)
        private string _statusBarContent;

        public string StatusBarContent
        {
            get => _statusBarContent;
            set => SetPropertyValue(ref _statusBarContent, value);
        }

        private bool _isCheckedLogin;

        public bool IsCheckedLogin
        {
            get => _isCheckedLogin;
            set => SetPropertyValue(ref _isCheckedLogin, value);
        }

        private bool _isEnabledLogin;

        public bool IsEnabledLogin
        {
            get => _isEnabledLogin;
            set => SetPropertyValue(ref _isEnabledLogin, value);
        }

        private bool _isCheckedLogout;

        public bool IsCheckedLogout
        {
            get => _isCheckedLogout;
            set => SetPropertyValue(ref _isCheckedLogout, value);
        }

        private bool _isEnabledLogout;

        public bool IsEnabledLogout
        {
            get => _isEnabledLogout;
            set => SetPropertyValue(ref _isEnabledLogout, value);
        }

        public ICommand LogoutCommand { get; }

        private void Logout()
        {
            IsCheckedLogin = false;
            IsEnabledLogin = true;

            IsCheckedLogout = true;
            IsEnabledLogout = false;

            StatusBarContent = "Logged-out: Please login!";
        }

        public ICommand LoginCommand { get; }

        private void OnLogin()
        {
            var viewModel = new Login.LoginViewModel();
            var view = new Login.LoginView { DataContext = viewModel };
            view.ShowDialog();

            if (viewModel.DialogResultValue == true)
            {
                LoginSuccessful();
                viewModel.Dispose();
                return;
            }

            Logout();
        }

        private void LoginSuccessful()
        {
            StatusBarContent = "Logged in as kmubarak.";
            IsCheckedLogin = true;
            IsEnabledLogin = false;

            IsCheckedLogout = false;
            IsEnabledLogout = true;
        }

        public ICommand ExitCommand { get; }

        private void OnExit()
        {
            _dialogCoordinator.ShowMessageAsync(this, "Wpf Playground", "Are you sure, you want to EXIT the application?", MessageDialogStyle.AffirmativeAndNegative)
                .ContinueWith(t =>
                {
                    if (t.Result == MessageDialogResult.Affirmative)
                    {
                        Application.Current.Shutdown();
                    }

                }, TaskScheduler.FromCurrentSynchronizationContext());
            /*
            var result = MessageBox.Show("Are you sure, you want Exit the application?","Quit",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
            */
        }

        public ICommand ChangeThemeCommand { get; }

        #endregion

        #region Async

        public ICommand ShowAdvAsyncAwaitCommand { get; }

        private void OnShowAdvAsyncAwait()
        {
            var viewModel = new AsyncEx.AdvAsyncAwaitEx.AdvAsyncAwaitViewModel();
            var view = new AsyncEx.AdvAsyncAwaitEx.AdvAsyncAwaitView { DataContext = viewModel };
            view.ShowDialog();

            viewModel.Dispose();
        }

        public ICommand ShowAsyncWithProgressbarCommand { get; }

        private void OnShowAsyncWithProgressbar()
        {
            var view = new AsyncEx.AsyncProgressbar.AsyncProgressbarView
            {
                DataContext = new AsyncEx.AsyncProgressbar.AsyncProgressbarViewModel()
            };
            view.ShowDialog();
        }

        public ICommand ShowBasicAsyncAwaitCommand { get; }

        private void OnShowBasicAsyncAwait()
        {
            var view = new AsyncEx.BasicAsyncAwaitEx.BasicAsyncAwaitExView();
            view.ShowDialog();
        }

        #endregion

        #region DataGrid

        public ICommand ShowNavigationDataGridCommand { get; }

        private void OnShowNavigationDataGrid()
        {
            var view = new DataGridsEx.NavigationEx.NavigationMainView();
            view.ShowDialog();
        }

        public ICommand ShowCheckBoxDataGridCommand { get; }

        private void OnShowCheckBoxDataGrid()
        {
            var roleDialogView = new DataGridsEx.CheckBoxDataGrid.SelectRoleDialogView();
            var roleDialogViewModel = new DataGridsEx.CheckBoxDataGrid.SelectRoleDialogViewModel(true, true);
            roleDialogView.DataContext = roleDialogViewModel;
            roleDialogView.ShowDialog();

            var view = new DataGridsEx.CheckBoxDataGrid.CheckBoxDataGridView();
            var viewModel = new DataGridsEx.CheckBoxDataGrid.CheckBoxDataGridViewModel(_dialogCoordinator, roleDialogViewModel.SelectedRoleEnumVal.ToString());
            view.DataContext = viewModel;
            view.ShowDialog();

            viewModel.Dispose();
        }

        public ICommand ShowMultipleDataGridsCommand { get; }

        private void OnShowMultipleDataGrids()
        {
            var view = new DataGridsEx.MultipleDataGrids.MultipleDataGridsView();
            view.DataContext = new DataGridsEx.MultipleDataGrids.MultipleDataGridsViewModel(_dialogCoordinator);
            view.ShowDialog();
        }

        public ICommand ShowAccountMgrCommand { get; }

        private void OnShowAccountMgr()
        {
            var view = new DataGridsEx.AccountMgr.AccountMgrView(_dialogCoordinator);
            view.ShowDialog();
        }

        public ICommand ShowSelectAllDataGridCommand { get; }

        private void OnShowSelectAllDataGrid()
        {
            var view = new SelectAllDataGridView { DataContext = new SelectAllDataGridViewModel(_dialogCoordinator) };
            view.ShowDialog();
        }

        #endregion

        #region Others
        public ICommand ShowXamlMiscCommand { get; }

        private void OnShowXamlMisc()
        {
            var view = new OtherEx.WpfXaml.MyWpfXaml();
            view.ShowDialog();
        }

        public ICommand ShowDataTemplatingCommand { get; }

        private void OnShowDataTemplating()
        {
            var view = new OtherEx.DataTemplating.DataTemplatingView();
            var viewModel = new OtherEx.DataTemplating.DataTemplatingViewModel();
            view.DataContext = viewModel;
            view.ShowDialog();
        }

        public ICommand ShowDataCachingViewCommand { get; }

        private void OnShowDataCaching()
        {
            var viewModel = new OtherEx.CachingEx.CachingViewModel();
            var view = new OtherEx.CachingEx.CachingView { DataContext = viewModel };
            view.ShowDialog();

            viewModel.Dispose();
        }

        public ICommand ShowBasicDataBindingCommand { get; }

        private void OnShowBasicDataBinding()
        {
            var view = new OtherEx.DataBinding.DataBindingView();
            view.DataContext = new OtherEx.DataBinding.DataBindingViewModel();
            view.ShowDialog();
        }

        public ICommand ShowFileDialogCommand { get; }

        private void OnShowFileDialog()
        {
            var view = new OtherEx.FileDialogEx.FileDialogView
            {
                DataContext = new OtherEx.FileDialogEx.FileDialogViewModel(new OpenFileDialogService())
            };
            view.ShowDialog();
        }

        public ICommand ShowItemsControlEx1Command { get; }

        private void OnShowItemsControlEx1()
        {
            var view = new OtherEx.ItemsControlEx.Ex1.ItemsControlView();
            view.DataContext = new OtherEx.ItemsControlEx.Ex1.ItemsControlViewModel(_dialogCoordinator);
            view.ShowDialog();
        }

        public ICommand ShowItemsControlEx2Command { get; }

        private void OnShowItemsControlEx2()
        {
            var view = new OtherEx.ItemsControlEx.Ex2.ItemsControlMainView();
            view.DataContext = new OtherEx.ItemsControlEx.Ex2.ItemsControlMainViewModel();
            view.ShowDialog();
        }

        public ICommand ShowItemsControlEx3Command { get; }

        private void OnShowItemsControlEx3()
        {
            var view = new OtherEx.ItemsControlEx.Ex3.ItemsCtrlView();
            var viewModel = new OtherEx.ItemsControlEx.Ex3.ItemsCtrlViewModel();
            view.DataContext = viewModel;
            view.ShowDialog();

            viewModel.Dispose();
        }

        public ICommand ShowThreeStateCheckBoxCommand { get; }

        private void OnShowThreeStateCheckBox()
        {
            var view = new OtherEx.ThreeStateCheckBox.ThreeStateCheckBoxView();
            view.DataContext = new OtherEx.ThreeStateCheckBox.ThreeStateCheckBoxViewModel();
            view.ShowDialog();

        }

        public ICommand SimpleNavigationCommand { get; }

        private void OnSimpleNavigation()
        {
            var view = new OtherEx.SimpleNavigation.NavigationMainView();
            view.DataContext = new OtherEx.SimpleNavigation.NavigationMainViewModel(_dialogCoordinator);
            view.ShowDialog();
        }
        #endregion

        #region TreeViews
        public ICommand ShowTreeViewSelectedItemCommand { get; }

        private void OnShowTreeViewSelectedItem()
        {
            var view = new TreeViewEx.TreeViewSelectItem.TreeViewSelectItemView();
            view.DataContext = new TreeViewEx.TreeViewSelectItem.TreeViewSelectItemViewModel();
            view.ShowDialog();
        }

        #endregion

        #region Tabs
        public ICommand ShowDynamicTabsCommand { get; }

        private void OnShowDynamicTabs()
        {
            var view = new TabsEx.DynamicTabs.DynamicTabsView
            {
                DataContext = new TabsEx.DynamicTabs.DynamicTabsViewModel()
            };
            view.ShowDialog();
        }

        public ICommand SowTabNavigationCommand { get; }

        private void OnSowTabNavigation()
        {
            var view = new TabsEx.TabNavigation.TabNavigationMainView
            {
                DataContext = new TabsEx.TabNavigation.TabNavigationMainViewModel()
            };
            view.ShowDialog();
        }

        #endregion

        #region ListViews
        public ICommand ShowListViewPagingCommand { get; }

        private void OnShowListViewPaging()
        {
            var view = new ListViewEx.ListViewPaging.ListViewPagingView();
            view.DataContext = new ListViewEx.ListViewPaging.ListViewPagingViewModel();
            view.ShowDialog();
        }

        public ICommand ShowListViewFilteringCommand { get; }

        private void OnShowListViewFiltering()
        {
            var viewModel = new ListViewEx.ListViewFiltering.ListViewFilteringViewModel();
            var view = new ListViewEx.ListViewFiltering.ListViewFilteringView { DataContext = viewModel };
            view.ShowDialog();

            viewModel.Dispose();
        }

        public ICommand ShowDelegateCommand { get; }

        private void OnShowDelegate()
        {
            var viewModel = new ListViewEx.Delegates.PersonDelegateViewModel();
            var view = new ListViewEx.Delegates.PersonDelegateView { DataContext = viewModel };
            view.ShowDialog();

            viewModel.Dispose();
        }

        #endregion

        #region Reactive

        public ICommand ShowSimpleCommand { get; }

        private void OnShowSimple()
        {
            var view = new ReactiveEx.Simple.SimpleView { DataContext = new ReactiveEx.Simple.SimpleViewModel() };
            view.ShowDialog();
        }

        public ICommand ShowReactiveGettingStartedCommand { get; }

        private void OnShowReactiveGettingStarted()
        {
            var view = new ReactiveEx.GettingStarted.GettingStartedMain();
            view.ShowDialog();
        }

        public ICommand ShowReactiveLoginCommand { get; }

        private void OnShowReactiveLogin()
        {
            var loginViewModel = new ReactiveEx.ReactiveLoginViewModel();
            var loginView = new ReactiveEx.ReactiveLoginView { DataContext = loginViewModel };
            loginView.ShowDialog();

            _dialogCoordinator.ShowMessageAsync(this, "Login",
                loginViewModel.IsUserAuthenticated ? "Login was successful!" : "Login Failed!");
        }

        public ICommand ShowTeapotsCommand { get; }

        private void OnShowTeapots()
        {
            var view = new ReactiveEx.TeapotCheckBoxes.TeapotsView
            { DataContext = new ReactiveEx.TeapotCheckBoxes.TeapotsViewModel() };
            view.ShowDialog();
        }

        public ICommand ShowOpenDialogCommand { get; }

        private void OnShowOpenDialog()
        {
            var mainOpenDialogView = new ReactiveEx.OpenDialogEx.OpenDialogMainView();
            mainOpenDialogView.ShowDialog();
        }

        public ICommand ShowProgressDialogCommand { get; }

        private ReactiveEx.ProgressDialog _progressDialog;
        private async void OnShowProgressDialog()
        {
            var machineName = "DFWKMUBARAK-L";
            var existingDialog = Application.Current.Windows.OfType<ReactiveEx.ProgressDialog>().FirstOrDefault(w => w.Tag.ToString() == machineName);

            if (existingDialog != null)
            {
                existingDialog.Activate();
                return;
            }

            var result = MessageBox.Show($"Are you sure, you want to launch Progress Dialog on {machineName}?", "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            _progressDialog = new ReactiveEx.ProgressDialog
            {
                Tag = machineName,
                ViewModel = { Message = $"Launching progress dialog on '{machineName}' for 6 seconds." }
            };

            _progressDialog.Closed += progressDialog_Closed;
            _progressDialog.Show();

            var x = await DummyAsyncAwait();

            if (Application.Current.Windows.OfType<ProgressDialog>().Any(w => ReferenceEquals(w, _progressDialog)))
            {
                // if the window is still open, close it
                _progressDialog.Closed -= progressDialog_Closed;
                _progressDialog.Close();
            }
        }

        private void progressDialog_Closed(object s, EventArgs ea)
        {
            _progressDialog.Closed -= progressDialog_Closed;
        }

        private async Task<bool> DummyAsyncAwait()
        {
            await Task.Delay(6000);
            return true;
        }

        public ICommand ShowCrudCommand { get; }

        private void OnShowCrud()
        {
            var view = new ReactiveEx.Crud.EmpView();
            view.ShowDialog();
        }

        public ICommand ShowCrud2Command { get; }

        private void OnShowCrud2()
        {
            var view = new ReactiveEx.Crud2.ReactiveEmployeeView();
            view.ShowDialog();
        }

        public ICommand ShowTodoCommand { get; }

        private void OnShowTodo()
        {
            var view = new ReactiveEx.TodoEx.TodoMainView { DataContext = new ReactiveEx.TodoEx.TodoMainViewModel() };
            view.ShowDialog();
        }

        public ICommand ShowDataEntryFormCommand { get; }

        private void OnShowDataEntryForm()
        {
            var view = new ReactiveEx.MultipleDataGrids.EntryFormView();
            var viewModel = new ReactiveEx.MultipleDataGrids.EntryFormViewModel();

            view.DataContext = viewModel;
            view.ShowDialog();
            viewModel.Dispose();
        }

        public ICommand ShowTreeAssignmentsCommand { get; }

        private void OnShowTreeAssignments()
        {
            var view = new ReactiveEx.TreeAssignments.TreeAssignmentsEditorView();
            view.ShowDialog();

            var viewModel = (ReactiveEx.TreeAssignments.TreeAssignmentsEditorViewModel)view.DataContext;
            viewModel.Dispose();
        }
        #endregion
    }
}
