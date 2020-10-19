using System.Windows;
using MahApps.Metro.Controls.Dialogs;

namespace Playground.WpfApp.Forms.DataGridsEx.AccountMgr
{
    public partial class AccountMgrView 
    {
        private readonly AccountMgrViewModel _viewModel;

        public AccountMgrView(IDialogCoordinator dialogCoordinator)
        {
            InitializeComponent();

            _viewModel = new AccountMgrViewModel(dialogCoordinator);
            DataContext = _viewModel;
            Closing += _viewModel.OnWindowClosing;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItemHelper.Content = e.NewValue;
            _viewModel.OnSelectedItemChanged();
        }
    }
}