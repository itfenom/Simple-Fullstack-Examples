using System.Windows;

namespace Playground.WpfApp.Forms.ReactiveEx.Crud2
{
    public partial class ReactiveEmployeeView
    {
        private readonly ReactiveEmployeeViewModel _viewModel;

        public ReactiveEmployeeView()
        {
            InitializeComponent();

            _viewModel = new ReactiveEmployeeViewModel();
            DataContext = _viewModel;
            Closing += ReactiveEmployeeView_Closing;
        }

        private void ReactiveEmployeeView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_viewModel.HasUnsavedChanges())
            {
                var result = MessageBox.Show("Unsaved changes found.\nDiscard changes and close?", "Confirm close",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true; //cancel closing!
                    return;
                }
            }

            _viewModel.Dispose();
            e.Cancel = false; //go ahead and close!
        }
    }
}
