using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;

namespace Playground.WpfApp.Forms.ReactiveEx.OpenDialogEx
{
    public partial class OpenDialogMainView : IViewFor<OpenDialogMainViewModel>
    {
        public OpenDialogMainView()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, vm => vm.SelectedValue, v => v.TxtSelectedValue.Text);
            this.OneWayBind(ViewModel, vm => vm.OpenAsDialog, v => v.OpenAsDialogButton.Command);
            this.WhenActivated(d => d(new CompositeDisposable(
                ViewModel.OpenAsDialogInteraction.RegisterHandler(interaction =>
                {
                    TestDialog dialog = new TestDialog
                    {
                        ViewModel = new TestDialogViewModel()
                    };
                    return Observable.Start(() =>
                    {
                        dialog.ShowDialog();
                        if (dialog.ViewModel.IsValidUsername)
                        {
                            ViewModel.SelectedValue = "Selected Value: " + dialog.ViewModel.Username;
                        }
                        else
                        {
                            ViewModel.SelectedValue = "";
                        }
                        interaction.SetOutput(Unit.Default);
                    }, RxApp.MainThreadScheduler);
                })
            )));

            ViewModel = new OpenDialogMainViewModel();
        }

        #region ViewModel
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel",
            typeof(OpenDialogMainViewModel), typeof(OpenDialogMainView), new PropertyMetadata(null));

        public OpenDialogMainViewModel ViewModel
        {
            get => (OpenDialogMainViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (OpenDialogMainViewModel)value;
        }
        #endregion
    }
}
