using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Repositories;

namespace Playground.WpfApp.Forms.DataGridsEx.AccountMgr
{
    public class CategoryNode : TreeViewItemVm
    {
        private readonly CategoryModel _category;
        private readonly IAccountRepository _repository;

        public CategoryNode(CategoryModel category)
            : base(null, true)
        {
            _category = category;
            _repository = new AccountRepository();
        }

        public string CategoryName => _category.CategoryName;

        public int CategoryId => _category.CategoryId;

        public string ToolTipText => $"{_category.CategoryName} - {_category.CategoryId}";

        protected override void LoadChildren()
        {
            var accounts = _repository.GetAccountsByCategoryId(_category.CategoryId);

            if (accounts == null || accounts.Count == 0) return;

            foreach (AccountModel account in accounts)
            {
                Children.Add(new AccountNode(account, this));
            }
        }
    }
}