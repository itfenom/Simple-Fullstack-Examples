using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Playground.WpfApp.Mvvm;
using ReactiveUI;

namespace Playground.WpfApp.Forms.ReactiveEx
{
    public class ReactiveMainViewModel : BindableBase
    {
        private readonly IDialogCoordinator _dialogCoordinator;

        public override string Title => "Reactive Main";

        public ReactiveCommand<Unit, Unit> CloseCommand { get; }

        public ReactiveMainViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;

            //Simple binding
            _fullName = this
                .WhenAnyValue(x => x.FirstName, x => x.LastName, (f, l) => $"{f}  {l}")
                .ToProperty(this, x => x.FullName);

            var canClickMe = this
                .WhenAnyValue(x => x.FirstName, x => x.LastName, (f, l) => !string.IsNullOrEmpty(f) && !string.IsNullOrEmpty(l));
            ClickMeCommand = ReactiveCommand.CreateFromObservable(() => DummyAsync(), canClickMe);
            ClickMeCommand.Subscribe(_ =>
            {
                Message = "Finished";
                StatusBarContentMsg = "Ready";
            });

            //Teapots
            ShowTeapotsCommand = new DelegateCommand(() => OnShowTeapots());

            //Open Dialog
            ShowOpenDialogCommand = new DelegateCommand(() => OnShowOpenDialog());

            //Progress Dialog
            ShowProgressDialogCommand = new DelegateCommand(() => OnShowProgressDialog());

            //Closing
            CloseCommand = ReactiveCommand.Create(() => OnClose());
            StatusBarContentMsg = "Ready";
        }

        #region Simple
        private readonly ObservableAsPropertyHelper<string> _fullName;
        public string FullName => _fullName.Value;

        private string _firstName;

        public string FirstName
        {
            get => _firstName;
            set => this.RaiseAndSetIfChanged(ref _firstName, value);
        }

        private string _lastName;

        public string LastName
        {
            get => _lastName;
            set => this.RaiseAndSetIfChanged(ref _lastName, value);
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        public ReactiveCommand<Unit, Unit> ClickMeCommand { get; }

        private IObservable<Unit> DummyAsync()
        {
            StatusBarContentMsg = "Running...";
            return Observable
                .Return(new Random().Next(0, 5))
                .Delay(TimeSpan.FromMilliseconds(50))
                .Do(
                    result =>
                    {
                        if (result > 5)
                        {
                            throw new InvalidOperationException("Failed to proceed...");
                        }
                    }
                ).Select(_ => Unit.Default);
        }

        #endregion

        #region Teapot checkBoxes
        public ICommand ShowTeapotsCommand { get; }

        private void OnShowTeapots()
        {
            var teapotView = new TeapotCheckBoxes.TeapotsView
            {
                DataContext = new TeapotCheckBoxes.TeapotsViewModel()
            };
            teapotView.ShowDialog();
        }
        #endregion

        #region Open Dialog
        public ICommand ShowOpenDialogCommand { get; }

        private void OnShowOpenDialog()
        {
            var mainOpenDialogView = new ReactiveEx.OpenDialogEx.OpenDialogMainView();
            mainOpenDialogView.ShowDialog();
        }
        #endregion

        #region Progress Dialog
        public ICommand ShowProgressDialogCommand { get; }

        private ProgressDialog _progressDialog;
        private async void OnShowProgressDialog()
        {
            var machineName = "DFWKMUBARAK-L";
            var existingDialog = Application.Current.Windows.OfType<ProgressDialog>().FirstOrDefault(w => w.Tag.ToString() == machineName);

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

            _progressDialog = new ProgressDialog
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

        #endregion

        //End
        private string _statusBarContentMsg;

        public string StatusBarContentMsg
        {
            get => _statusBarContentMsg;
            set => this.RaiseAndSetIfChanged(ref _statusBarContentMsg, value);
        }
        private void OnClose()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.Title == Title)
                {
                    window.Close();
                }
            }
        }
    }
}
