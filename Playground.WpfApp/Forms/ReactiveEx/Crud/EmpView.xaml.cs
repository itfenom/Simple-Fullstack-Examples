using System.Windows;

namespace Playground.WpfApp.Forms.ReactiveEx.Crud
{
    public partial class EmpView
    {
        private readonly EmpViewModel _viewModel;

        public EmpView()
        {
            InitializeComponent();

            _viewModel = new EmpViewModel();
            DataContext = _viewModel;
            Closing += EmpView_Closing;
        }

        private void EmpView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
