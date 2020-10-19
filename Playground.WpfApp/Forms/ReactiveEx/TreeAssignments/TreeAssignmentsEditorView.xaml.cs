using System.Windows;

namespace Playground.WpfApp.Forms.ReactiveEx.TreeAssignments
{
    public partial class TreeAssignmentsEditorView
    {
        private readonly TreeAssignmentsEditorViewModel _viewModel;

        public TreeAssignmentsEditorView()
        {
            InitializeComponent();

            _viewModel = new TreeAssignmentsEditorViewModel();
            DataContext = _viewModel;
            Closing += TreeAssignmentsEditorView_Closing;
        }

        private void TreeAssignmentsEditorView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
