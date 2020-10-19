using System.Windows;

namespace Playground.WpfApp.Forms.DataGridsEx.NavigationEx
{
    public partial class NavigationMainView
    {
        private readonly NavigationMainViewModel _viewModel;

        public NavigationMainView()
        {
            InitializeComponent();

            _viewModel = new NavigationMainViewModel();
            DataContext = _viewModel;
            Closing += NavigationMainView_Closing;
        }

        private void NavigationMainView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_viewModel.SelectedChild?.HasUnSavedChanges() == true)
            {
                var result = MessageBox.Show(
                    "There are unsaved changes waiting to be saved...\nDiscard changes and close?",
                    "Confirm Close",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true; //cancel closing!
                    return;
                }
            }

            _viewModel?.Dispose();
            e.Cancel = false;
        }
    }
}
