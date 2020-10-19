using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.DataGridsEx.AccountMgr
{
    public class AccountNode : TreeViewItemVm
    {
        private readonly AccountModel _account;

        public AccountNode(AccountModel account, CategoryNode category)
            : base(category, true)
        {
            _account = account;
        }

        public int AccountId => _account.AccountId;

        public string AccountName => _account.AccountName;

        public string ToolTipText => $"{_account.AccountName} - {_account.AccountId}";
    }
}