using ReactiveUI;
using System.Reactive.Disposables;
using Playground.WpfApp.WpfUtilities;

namespace Playground.WpfApp.Forms.ReactiveEx
{
    public partial class ProgressDialog : ProgressDialogBase
    {
        public ProgressDialog()
        {
            InitializeComponent();

            if (this.IsInDesignMode())
            {
                return;
            }

            ViewModel = new ProgressDialogViewModel();

            this.WhenActivated(disposable =>
            {
                this
                    .OneWayBind(ViewModel, vm => vm.Title, v => v.Title)
                    .DisposeWith(disposable);

                this
                    .OneWayBind(ViewModel, vm => vm.Message, v => v._txtMessage.Text)
                    .DisposeWith(disposable);
            });
        }
    }

    public class ProgressDialogBase : ReactiveMetroWindow<ProgressDialogViewModel>
    {
    }

    public class ProgressDialogViewModel : BindableBase
    {
        private string _message = "Working...";
        private string _title = "Please Wait...";

        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }
    }
}
