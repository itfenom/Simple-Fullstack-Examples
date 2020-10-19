using System.Windows;

namespace Playground.WpfApp.Forms.ReactiveEx.Crud2
{
    public partial class ReactiveEmployeeEditorView
    {
        private readonly ReactiveEmployeeEditorViewModel _viewModel;

        public ReactiveEmployeeEditorView(int id, string transactionType)
        {
            InitializeComponent();

            _viewModel = new ReactiveEmployeeEditorViewModel(id, transactionType);
            DataContext = _viewModel;
            Closing += ReactiveEmployeeEditorView_Closing;
        }

        private void ReactiveEmployeeEditorView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
            e.Cancel = false; //go ahead and close!
        }
    }
}
