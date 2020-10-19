using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;

namespace Playground.WpfApp.Forms.ReactiveEx.OpenDialogEx
{
    public class TestDialogViewModel : ReactiveObject
    {
        private string _username = "";
        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isValidUsername;
        public bool IsValidUsername => _isValidUsername.Value;

        public ReactiveCommand<Unit, Unit> OkCommand { get; }

        private bool? _dialogResultDependencyProperty;

        public bool? DialogResultDependencyProperty
        {
            get => _dialogResultDependencyProperty;
            set => this.RaiseAndSetIfChanged(ref _dialogResultDependencyProperty, value);
        }


        public TestDialogViewModel()
        {
            var canClickOk = this.WhenAnyValue(
                x => x.Username,
                (u) => !string.IsNullOrEmpty(u) &&
                       u.Length > 0);

            OkCommand = ReactiveCommand.CreateFromObservable(DummyAsync, canClickOk);
            OkCommand.Subscribe(r =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType().Name.Contains("TestDialog"))
                    {
                        window.Close();
                    }
                }
            });
            this.WhenAnyValue(vm => vm.Username).Select(_ => Username.Length > 0).ToProperty(this, vm => vm.IsValidUsername, out _isValidUsername);
            this.WhenAnyValue(vm => vm.IsValidUsername).Subscribe(_ => { });
        }

        private IObservable<Unit> DummyAsync() =>
            Observable
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
    public partial class TestDialog : IViewFor<TestDialogViewModel>
    {
        #region ViewModel
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel",
            typeof(TestDialogViewModel), typeof(TestDialog), new PropertyMetadata(null));

        public TestDialogViewModel ViewModel
        {
            get => (TestDialogViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestDialogViewModel)value;
        }
        #endregion

        public TestDialog()
        {
            InitializeComponent();

            this.WhenAnyValue(v => v.ViewModel.IsValidUsername).Subscribe(_ => { });
            this.Bind(ViewModel, vm => vm.Username, v => v.UsernameBox.Text);
            this.OneWayBind(ViewModel, vm => vm.IsValidUsername, v => v.UsernameErrorBorder.BorderThickness, valid => valid ? new Thickness(0) : new Thickness(1));
            this.OneWayBind(ViewModel, vm => vm.OkCommand, v => v.BtnOk.Command);
            this.OneWayBind(ViewModel, vm => vm.DialogResultDependencyProperty, v => v.DialogResult);
        }
    }
}
