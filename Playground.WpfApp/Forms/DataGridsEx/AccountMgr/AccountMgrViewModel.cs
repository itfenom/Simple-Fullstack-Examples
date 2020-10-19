using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Playground.WpfApp.Mvvm;
using Playground.WpfApp.Repositories;

namespace Playground.WpfApp.Forms.DataGridsEx.AccountMgr
{
    public class AccountMgrViewModel : PropertyChangedBase
    {
        public override string Title => "Account Mgr: TreeView with GridView";
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IAccountRepository _repository;
        private List<CategoryModel> _allCategories;
        private List<CategoryNode> _expandedCategoryNodes;
        private List<AccountNode> _expandedAccountNodes;
        public bool UserAgreesToClose;

        private ObservableCollection<CategoryNode> _categoryNodes;

        public ObservableCollection<CategoryNode> CategoryNodes
        {
            get => _categoryNodes;
            set => SetPropertyValue(ref _categoryNodes, value);
        }

        private object _selectedObject;

        public object SelectedObject
        {
            get => _selectedObject;
            set => SetPropertyValue(ref _selectedObject, value);
        }

        private int _selectedTabIndex; //Setting tab value to 0 by Default so that window opens to this-Tab

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetPropertyValue(ref _selectedTabIndex, value);
        }

        private List<Predicate<CategoryModel>> CategoryFilterCriteria;
        private List<CategoryModel> _deletedCategoryObjects;

        private List<Predicate<AccountModel>> AccountFilterCriteria;
        private List<AccountModel> _deletedAccountObjects;

        private string _accountCategorySearchText;

        public string AccountCategorySearchText
        {
            get => _accountCategorySearchText;
            set => SetPropertyValue(ref _accountCategorySearchText, value);
        }

        public AccountMgrViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
            _repository = new AccountRepository();

            _selectedTabIndex = 0;
            _selectedObject = null;

            _allCategories = _repository.GetAllCategories();
            _expandedCategoryNodes = new List<CategoryNode>();
            _expandedAccountNodes = new List<AccountNode>();

            CategoryFilterCriteria = new List<Predicate<CategoryModel>>();
            _deletedCategoryObjects = new List<CategoryModel>();
            _categoryNameFilter = string.Empty;

            AccountFilterCriteria = new List<Predicate<AccountModel>>();
            _deletedAccountObjects = new List<AccountModel>();
            _accountNameFilter = string.Empty;

            //Initialize commands
            CloseCommand = new DelegateCommand(() => OnClosing());
            ReloadTreeViewCommand = new DelegateCommand(() => LoadTreeView());
            ClearSearchCommand = new DelegateCommand(() => ClearSearch());
            SearchAccountCategoryCommand = new DelegateCommand(() => PerformAccountCategorySearch());
            ClearAccountFilterCommand = new DelegateCommand(() => ClearAccountFilters());
            ClearCategoriesFilterCommand = new DelegateCommand(() => ClearCategoryFilters());
            AccountContextMenuCommand = new DelegateCommand<object>(param => AccountContextMenu_Click(param));
            CategoryContextMenuCommand = new DelegateCommand<object>(param => CategoryContextMenu_Click(param));

            //Account Commands
            AddNewAccountCommand = new DelegateCommand(() => AddNewAccount());
            _deleteAccountCommand = new DelegateCommand(() => DeleteAccount(), () => (SelectedAccount != null));
            _saveAccountCommand = new DelegateCommand(() => SaveAccounts(), () => CanSaveAccount);

            //Category Commands
            AddNewCategoryCommand = new DelegateCommand(() => AddNewCategory());
            _deleteCategoryCommand = new DelegateCommand(() => DeleteCategory(), () => (SelectedCategory != null));
            _saveCategoryCommand = new DelegateCommand(() => SaveCategories(), () => CanSaveCategory);

            //Load Categories for comboBox
            _categoriesComboBox = new ObservableCollection<CategoryModel>();
            foreach (var item in _allCategories)
            {
                _categoriesComboBox.Add(new CategoryModel
                {
                    CategoryId = item.CategoryId,
                    CategoryName = item.CategoryName
                });
            }

            CategoriesComboBox = _categoriesComboBox;

            LoadTreeView();

            PropertyChanged += AccountMgrViewModel_PropertyChanged;
        }

        private void AccountMgrViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsDirty")
            {
                //Categories
                _deleteCategoryCommand.RaiseCanExecuteChanged();
                _saveCategoryCommand.RaiseCanExecuteChanged();

                //Accounts
                _deleteAccountCommand.RaiseCanExecuteChanged();
                _saveAccountCommand.RaiseCanExecuteChanged();
            }
        }

        private void LoadAccountsIntoGridView(int accountId, int parentId)
        {
            if (Accounts == null || Accounts.Count == 0)
            {
                LoadAccountsByCategoryId(parentId);
            }
            else
            {
                // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
                var existingAccount = Accounts.Where(a => a.AccountId == accountId).FirstOrDefault();

                if (existingAccount == null)
                {
                    LoadAccountsByCategoryId(parentId);
                }
            }

            SelectedAccount = Accounts.FirstOrDefault(a => a.AccountId == accountId);
        }

        private void LoadAccountsByCategoryId(int categoryId)
        {
            var accounts = _repository.GetAccountsByCategoryId(categoryId);
            Accounts = new ObservableCollection<AccountModel>();
            foreach (var item in accounts)
            {
                var acct = new AccountModel
                {
                    AccountId = item.AccountId,
                    AccountName = item.AccountName,
                    AccountLoginId = item.AccountLoginId,
                    AccountPassword = item.AccountPassword,
                    CategoryId = item.CategoryId,
                    Notes = item.Notes,
                    IsPasswordEncrypted = item.IsPasswordEncrypted
                };

                acct.PropertyChanged += AccountMgrViewModel_PropertyChanged;

                Accounts.Add(acct);
            }

            AccountsView = (CollectionView)new CollectionViewSource { Source = Accounts }.View;
            NotifyPropertyChanged("AccountsView");
            Accounts.CollectionChanged += Accounts_CollectionChanged;
        }

        private void LoadCategoriesIntoGridView(int categoryId)
        {
            if (_categories == null || _categories.Count == 0)
            {
                LoadAllCategories();

                SelectedCategory = Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            }
            else
            {
                SelectedCategory = Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            }
        }

        public void OnSelectedItemChanged()
        {
            if (_selectedObject != null)
            {
                //Reset accounts in the Account DataGrid.
                Accounts = null;
                NotifyPropertyChanged("Accounts");
                AccountsView = null;
                NotifyPropertyChanged("AccountsView");

                if (_selectedObject is CategoryNode categoryNode)
                {
                    LoadCategoriesIntoGridView(categoryNode.CategoryId);

                    var categoryModel = new CategoryModel
                    {
                        CategoryId = categoryNode.CategoryId,
                        CategoryName = categoryNode.CategoryName
                    };

                    SelectedCategory = categoryModel;
                    SelectedTabIndex = 1;
                }
                else if (_selectedObject is AccountNode accountNode)
                {
                    var parentNode = (CategoryNode)accountNode.Parent;
                    LoadAccountsIntoGridView(accountNode.AccountId, parentNode.CategoryId);

                    var acct = _repository.GetAccountByAccountId(accountNode.AccountId);
                    var accountModel = new AccountModel
                    {
                        AccountId = acct.AccountId,
                        AccountName = acct.AccountName,
                        AccountLoginId = acct.AccountLoginId,
                        AccountPassword = acct.AccountPassword,
                        Notes = acct.Notes,
                        CategoryId = acct.CategoryId
                    };

                    SelectedAccount = accountModel;
                    SelectedTabIndex = 2;
                }
            }
        }

        #region TreeView

        private void LoadTreeView()
        {
            if (CategoryNodes != null)
            {
                _expandedCategoryNodes.Clear();
                _expandedAccountNodes.Clear();

                foreach (var categoryNode in _categoryNodes)
                {
                    foreach (var childNode in categoryNode.Children)
                    {
                        if (childNode is AccountNode)
                        {
                            var accountNode = (AccountNode)childNode;
                            if (!_expandedAccountNodes.Contains(accountNode))
                            {
                                _expandedAccountNodes.Add(accountNode);
                            }
                        }
                    }

                    if (categoryNode.IsExpanded)
                    {
                        if (!_expandedCategoryNodes.Contains(categoryNode))
                        {
                            _expandedCategoryNodes.Add(categoryNode);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(AccountCategorySearchText))
            {
                var filteredCategories = (from a in _allCategories where a.CategoryName.ToLower().Contains(AccountCategorySearchText.ToLower()) select a).ToList();

                _categoryNodes = new ObservableCollection<CategoryNode>(
                    (from category in filteredCategories
                     select new CategoryNode(category))
                    .ToList());
            }
            else
            {
                _categoryNodes = new ObservableCollection<CategoryNode>(
                    (from category in _allCategories
                     select new CategoryNode(category))
                    .ToList());
            }

            CategoryNodes = _categoryNodes;

            if (_expandedCategoryNodes.Count > 0)
            {
                foreach (var item in CategoryNodes)
                {
                    foreach (var childItem in item.Children)
                    {
                        if (childItem is AccountNode)
                        {
                            var accountNode = (AccountNode)childItem;
                            if (DoesAccountNodeNeedToBeExpanded(accountNode))
                            {
                                childItem.IsExpanded = true;
                            }
                        }
                    }

                    if (DoesCategoryNodeNeedToBeExpanded(item))
                    {
                        item.IsExpanded = true;
                    }
                }
            }
        }

        private void LoadAllCategories()
        {
            var allCategories = _repository.GetAllCategories();
            _categories = new ObservableCollection<CategoryModel>();
            foreach (var item in allCategories)
            {
                var category = new CategoryModel();
                category.CategoryId = item.CategoryId;
                category.CategoryName = item.CategoryName;
                category.PropertyChanged += AccountMgrViewModel_PropertyChanged;

                _categories.Add(category);
            }

            CategoriesView = (CollectionView)new CollectionViewSource { Source = _categories }.View;
            NotifyPropertyChanged("CategoriesView");
            Categories.CollectionChanged += Categories_CollectionChanged;
        }

        private bool DoesCategoryNodeNeedToBeExpanded(CategoryNode categoryNode)
        {
            bool retVal = false;
            foreach (var item in _expandedCategoryNodes)
            {
                if (item.CategoryName == categoryNode.CategoryName)
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        private bool DoesAccountNodeNeedToBeExpanded(AccountNode accountNode)
        {
            bool retVal = false;
            foreach (var item in _expandedAccountNodes)
            {
                if (item.AccountName == accountNode.AccountName)
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        #endregion TreeView

        #region DataGrid: Category

        private void Categories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    break;

                case NotifyCollectionChangedAction.Remove:
                    break;

                case NotifyCollectionChangedAction.Replace:
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    break;
            }

            RefreshCategoriesView();
        }

        private ObservableCollection<CategoryModel> _categories;
        public ObservableCollection<CategoryModel> Categories => _categories;

        public CollectionView CategoriesView { get; set; }

        private CategoryModel _selectedCategory;

        public CategoryModel SelectedCategory
        {
            get => _selectedCategory;
            set => SetPropertyValue(ref _selectedCategory, value);
        }

        private string _categoryNameFilter;

        public string CategoryNameFilter
        {
            get => _categoryNameFilter;
            set
            {
                if (_categoryNameFilter == value) return;
                SetPropertyValue(ref _categoryNameFilter, value);
                ApplyCategoryFilters();
            }
        }

        private void ApplyCategoryFilters()
        {
            if (CategoriesView == null) return;

            if (string.IsNullOrEmpty(CategoryNameFilter))
            {
                RefreshCategoriesView();
                return;
            }

            try
            {
                CategoryFilterCriteria.Clear();

                if (!string.IsNullOrEmpty(CategoryNameFilter))
                {
                    CategoryFilterCriteria.Add(x => x.CategoryName.ToLower().Contains(CategoryNameFilter.ToLower()));
                }

                CategoriesView.Filter = Category_Filter;
                NotifyPropertyChanged("CategoriesView");

                RefreshCategoriesView();
            }
            catch (Exception oEx)
            {
                if (CategoriesView is IEditableCollectionView editableCollectionView)
                {
                    if (editableCollectionView.IsAddingNew)
                    {
                        editableCollectionView.CommitNew();
                    }

                    if (editableCollectionView.IsEditingItem)
                    {
                        editableCollectionView.CommitEdit();
                    }

                    CategoriesView.Filter = Category_Filter;
                    NotifyPropertyChanged("CategoriesView");
                }
                else
                {
                    _dialogCoordinator.ShowMessageAsync(this, "Exception", oEx.Message);
                }
            }
        }

        private bool Category_Filter(object item)
        {
            if (CategoryFilterCriteria.Count == 0)
                return true;
            var cat = item as CategoryModel;
            return CategoryFilterCriteria.TrueForAll(x => x(cat));
        }

        private void ClearCategoryFilters()
        {
            try
            {
                CategoryFilterCriteria.Clear();
                RefreshCategoriesView();

                CategoryNameFilter = string.Empty;

                // Bring the current record back into view in case it moved
                if (SelectedCategory != null && (SelectedCategory.IsDeleted == false))
                {
                    CategoryModel current = SelectedCategory;
                    CategoriesView.MoveCurrentToFirst();
                    CategoriesView.MoveCurrentTo(current);
                }

                RefreshCategoriesView();
            }
            catch (Exception oEx)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Exception", oEx.Message);
            }
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private object RefreshCategoriesView()
        {
            try
            {
                CategoriesView.Refresh();
                NotifyPropertyChanged("CategoriesView");
            }
            catch (Exception oEx)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Exception", oEx.Message);
            }
            return null;
        }

        #endregion DataGrid: Category

        #region DataGrid: Account

        private void Accounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshAccountsView();
        }

        private ObservableCollection<AccountModel> _accounts;

        public ObservableCollection<AccountModel> Accounts
        {
            get => _accounts;
            set => SetPropertyValue(ref _accounts, value);
        }

        private ObservableCollection<CategoryModel> _categoriesComboBox;

        public ObservableCollection<CategoryModel> CategoriesComboBox
        {
            get => _categoriesComboBox;
            set => SetPropertyValue(ref _categoriesComboBox, value);
        }

        public CollectionView AccountsView { get; set; }

        private AccountModel _selectedAccount;

        public AccountModel SelectedAccount
        {
            get => _selectedAccount;
            set => SetPropertyValue(ref _selectedAccount, value);
        }

        private string _accountNameFilter;

        public string AccountNameFilter
        {
            get => _accountNameFilter;
            set
            {
                if (_accountNameFilter == value) return;
                SetPropertyValue(ref _accountNameFilter, value);
                ApplyAccountFilters();
            }
        }

        private void ApplyAccountFilters()
        {
            if (AccountsView == null) return;

            if (string.IsNullOrEmpty(AccountNameFilter))
            {
                RefreshAccountsView();
                return;
            }

            try
            {
                AccountFilterCriteria.Clear();

                if (!string.IsNullOrEmpty(AccountNameFilter))
                {
                    AccountFilterCriteria.Add(x => x.AccountName.ToLower().Contains(AccountNameFilter.ToLower()));
                }

                AccountsView.Filter = Account_Filter;
                NotifyPropertyChanged("AccountsView");

                RefreshAccountsView();
            }
            catch (Exception oEx)
            {
                if (AccountsView is IEditableCollectionView editableCollectionView)
                {
                    if (editableCollectionView.IsAddingNew)
                    {
                        editableCollectionView.CommitNew();
                    }

                    if (editableCollectionView.IsEditingItem)
                    {
                        editableCollectionView.CommitEdit();
                    }

                    AccountsView.Filter = Account_Filter;
                    NotifyPropertyChanged("AccountsView");
                }
                else
                {
                    _dialogCoordinator.ShowMessageAsync(this, "Exception", oEx.Message);
                }
            }
        }

        private bool Account_Filter(object item)
        {
            if (AccountFilterCriteria.Count == 0)
                return true;

            var acct = item as AccountModel;
            return AccountFilterCriteria.TrueForAll(x => x(acct));
        }

        private void ClearAccountFilters()
        {
            try
            {
                if (AccountsView == null || AccountsView.Filter == null) return;

                AccountFilterCriteria.Clear();

                AccountNameFilter = string.Empty;

                // Bring the current record back into view in case it moved
                if (SelectedAccount != null && (SelectedAccount.IsDeleted == false))
                {
                    AccountModel current = SelectedAccount;
                    AccountsView.MoveCurrentToFirst();
                    AccountsView.MoveCurrentTo(current);
                }

                RefreshAccountsView();
            }
            catch (Exception oEx)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Exception", oEx.Message);
            }
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private object RefreshAccountsView()
        {
            try
            {
                AccountsView.Refresh();
                NotifyPropertyChanged("AccountsView");
            }
            catch (Exception oEx)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Exception", oEx.Message);
            }
            return null;
        }

        #endregion DataGrid: Account

        #region Button/Commands

        public ICommand CloseCommand { get; }
        public ICommand ReloadTreeViewCommand { get; }
        public ICommand ClearSearchCommand { get; }

        private void ClearSearch()
        {
            AccountCategorySearchText = string.Empty;
            LoadTreeView();
        }

        public ICommand SearchAccountCategoryCommand { get; }

        private void PerformAccountCategorySearch()
        {
            if (string.IsNullOrEmpty(AccountCategorySearchText)) return;
            LoadTreeView();
        }

        public ICommand ClearAccountFilterCommand { get; }

        public ICommand AddNewAccountCommand { get; }

        private void AddNewAccount()
        {
            var accountModel = new AccountModel
            {
                AccountId = -99,
                AccountName = string.Empty,
                AccountLoginId = string.Empty,
                AccountPassword = string.Empty,
                Notes = string.Empty,
                // ReSharper disable once MergeConditionalExpression
                CategoryId = (SelectedCategory == null ? 0 : SelectedCategory.CategoryId),
                IsPasswordEncrypted = "N",
                IsDirty = true,
                IsNew = true
            };

            accountModel.PropertyChanged += AccountMgrViewModel_PropertyChanged;
            SelectedAccount = accountModel;

            if (_accounts == null)
            {
                _accounts = new ObservableCollection<AccountModel>();

                AccountsView = (CollectionView)new CollectionViewSource { Source = Accounts }.View;
                NotifyPropertyChanged("AccountsView");
                Accounts.CollectionChanged += Accounts_CollectionChanged;
            }

            _accounts.Add(accountModel);
        }

        private DelegateCommand _deleteAccountCommand;

        public DelegateCommand DeleteAccountCommand => _deleteAccountCommand;

        private void DeleteAccount()
        {
            if (!_deletedAccountObjects.Contains(SelectedAccount))
            {
                _deletedAccountObjects.Add(SelectedAccount);
            }

            SelectedAccount.IsDeleted = true;
            SelectedAccount.IsDirty = true;
            Accounts.Remove(SelectedAccount);
        }

        private DelegateCommand _saveAccountCommand;

        public DelegateCommand SaveAccountCommand => _saveAccountCommand;

        public bool CanSaveAccount
        {
            get
            {
                //if (this.SelectedCategory == null) return false;

                if (HasUnSavedAccounts() == false) return false;

                var errObjects = (from a in _accounts where a.ErrorCount > 0 select a).ToList();

                if (errObjects.Any()) return false;

                return true;
            }
        }

        private void SaveAccounts()
        {
            try
            {
                var isDirtyObjects = (from a in Accounts where a.IsDirty select a).ToList();

                foreach (var item in isDirtyObjects)
                {
                    if (item.IsNew)
                    {
                        var newAcctModel = new AccountModel();
                        newAcctModel.AccountId = item.AccountId;
                        newAcctModel.AccountName = Core.Utilities.HelperTools.FormatSqlString(item.AccountName);
                        newAcctModel.AccountLoginId = Core.Utilities.HelperTools.FormatSqlString(item.AccountLoginId);
                        newAcctModel.AccountPassword = Core.Utilities.Encryption.Encrypt(item.AccountPassword);
                        newAcctModel.Notes = Core.Utilities.HelperTools.FormatSqlString(item.Notes);
                        newAcctModel.DateCreated = DateTime.Today;
                        newAcctModel.CategoryId = item.CategoryId;
                        newAcctModel.IsPasswordEncrypted = "Y";

                        _repository.InsertNewAccount(newAcctModel);
                        item.IsNew = false;
                        item.IsDirty = false;
                        item.IsPasswordEncrypted = "Y";
                    }
                    else if (item.IsDeleted)
                    {
                        _repository.DeleteAccount(item.AccountId);
                        item.IsDeleted = false;
                        item.IsDirty = false;
                    }
                    else //its an update
                    {
                        var updatedAcctModel = new AccountModel();
                        updatedAcctModel.AccountId = item.AccountId;
                        updatedAcctModel.AccountName = Core.Utilities.HelperTools.FormatSqlString(item.AccountName);
                        updatedAcctModel.AccountLoginId = Core.Utilities.HelperTools.FormatSqlString(item.AccountLoginId);

                        var existingPassword = _repository.GetPassword(item.AccountId);
                        if (existingPassword == item.AccountPassword)
                        {
                            updatedAcctModel.AccountPassword = item.AccountPassword;
                        }
                        else
                        {
                            updatedAcctModel.AccountPassword = Core.Utilities.Encryption.Encrypt(item.AccountPassword);
                        }

                        updatedAcctModel.IsPasswordEncrypted = "Y";
                        updatedAcctModel.Notes = Core.Utilities.HelperTools.FormatSqlString(item.Notes);
                        updatedAcctModel.DateCreated = DateTime.Today;
                        updatedAcctModel.CategoryId = item.CategoryId;

                        _repository.UpdateAccount(updatedAcctModel);
                        item.IsDirty = false;
                    }
                }

                if (_deletedAccountObjects.Count > 0)
                {
                    foreach (var item in _deletedAccountObjects)
                    {
                        _repository.DeleteAccount(item.AccountId);
                    }
                }

                //Load TreeView
                LoadTreeView();

                //Load Accounts
                if (SelectedCategory != null)
                {
                    LoadAccountsByCategoryId(Convert.ToInt32(SelectedCategory.CategoryId));
                }
            }
            catch (Exception oEx)
            {
                MessageBox.Show(oEx.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public ICommand ClearCategoriesFilterCommand { get; }

        public ICommand AddNewCategoryCommand { get; }

        private void AddNewCategory()
        {
            var categoryModel = new CategoryModel();
            categoryModel.CategoryId = (_repository.GetAllCategories().Count() + 1);
            categoryModel.CategoryName = string.Empty;

            categoryModel.IsDirty = true;
            categoryModel.IsNew = true;
            categoryModel.PropertyChanged += AccountMgrViewModel_PropertyChanged;
            SelectedCategory = categoryModel;
            _categories.Add(categoryModel);
        }

        private DelegateCommand _deleteCategoryCommand;

        public DelegateCommand DeleteCategoryCommand
        {
            get => _deleteCategoryCommand;
            set => _deleteCategoryCommand = value;
        }

        private void DeleteCategory()
        {
            if (!_deletedCategoryObjects.Contains(SelectedCategory))
            {
                _deletedCategoryObjects.Add(SelectedCategory);
            }

            SelectedCategory.IsDeleted = true;
            SelectedCategory.IsDirty = true;
            Categories.Remove(SelectedCategory);
        }

        private DelegateCommand _saveCategoryCommand;

        public DelegateCommand SaveCategoryCommand
        {
            get => _saveCategoryCommand;
            set => _saveCategoryCommand = value;
        }

        public bool CanSaveCategory
        {
            get
            {
                //if (this.SelectedCategory == null) return false;

                if (HasUnSavedCategories() == false) return false;

                var errObjects = (from c in _categories where c.ErrorCount > 0 select c).ToList();

                if (errObjects.Any()) return false;

                return true;
            }
        }

        private void SaveCategories()
        {
            try
            {
                var isDirtyObjects = (from c in Categories where c.IsDirty select c).ToList();

                foreach (var item in isDirtyObjects)
                {
                    if (item.IsNew)
                    {
                        _repository.InsertNewAccountCategory(item.CategoryName);
                    }
                    else if (item.IsDeleted)
                    {
                        _repository.DeleteAccountCategory(Convert.ToInt32(item.CategoryId));
                    }
                    else //its an update
                    {
                        _repository.UpdateAccountCategory(item.CategoryId, item.CategoryName);
                    }
                }

                if (_deletedCategoryObjects.Count > 0)
                {
                    foreach (var item in _deletedCategoryObjects)
                    {
                        _repository.DeleteAccountCategory(item.CategoryId);
                    }
                }

                //Load TreeView
                LoadTreeView();

                //Load Categories
                LoadAllCategories();
            }
            catch (Exception oEx)
            {
                MessageBox.Show(oEx.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public ICommand AccountContextMenuCommand { get; }

        private void AccountContextMenu_Click(object param)
        {
            if (SelectedAccount != null && param != null)
            {
                if (param.ToString().ToUpper() == "VIEWDETAIL")
                {
                    var sb = new StringBuilder();

                    sb.AppendLine("Name: " + SelectedAccount.AccountName);
                    sb.AppendLine("Login: " + SelectedAccount.AccountLoginId);

                    if (SelectedAccount.IsPasswordEncrypted == "Y")
                    {
                        sb.AppendLine("Password: " + Core.Utilities.Encryption.Decrypt(SelectedAccount.AccountPassword));
                    }
                    else
                    {
                        sb.AppendLine("Password: " + SelectedAccount.AccountPassword);
                    }

                    sb.AppendLine("Notes: " + SelectedAccount.Notes);

                    MessageBox.Show(sb.ToString(), "View Details", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        public ICommand CategoryContextMenuCommand { get; }

        private void CategoryContextMenu_Click(object param)
        {
            if (SelectedCategory != null && SelectedObject != null && param != null)
            {
                var categoryNode = (CategoryNode)SelectedObject;
                if (categoryNode == null) throw new ArgumentNullException(nameof(categoryNode));

                switch (param.ToString().ToUpper())
                {
                    case "DISPLAY":
                        _dialogCoordinator.ShowMessageAsync(this, "Category Context Menu: " + categoryNode.CategoryName, "Display Clicked!");
                        break;
                    case "EDIT":
                        _dialogCoordinator.ShowMessageAsync(this, "Category Context Menu: " + categoryNode.CategoryName, "Edit Clicked!");
                        break;
                }
            }
        }

        #endregion Button/Commands

        #region Closing

        public bool HasUnSavedCategories()
        {
            if (Categories == null) return false;

            var isDirtyObjects = (from c in Categories where c.IsDirty select c).ToList();

            if (isDirtyObjects.Any()) return true;

            if (_deletedCategoryObjects.Count > 0) return true;

            return false;
        }

        public bool HasUnSavedAccounts()
        {
            if (Accounts == null) return false;

            var isDirtyObjects = (from a in Accounts where a.IsDirty select a).ToList();

            if (isDirtyObjects.Any()) return true;

            if (_deletedAccountObjects.Count > 0) return true;

            return false;
        }

        private void CloseWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    UserAgreesToClose = true;
                    window.Close();
                }
            }
        }


        public async void OnClosing()
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Ok",
                NegativeButtonText = "Cancel",
                AnimateShow = true,
                AnimateHide = false
            };

            if (HasUnSavedCategories() || HasUnSavedAccounts())
            {
                var result = await _dialogCoordinator.ShowMessageAsync(this,
                    "Confirm Close",
                    "Are you sure, you want to close without Saving?",
                    MessageDialogStyle.AffirmativeAndNegative, mySettings);

                if (result == MessageDialogResult.Affirmative)
                {
                    CloseWindow();
                    return;
                }

                return;
            }

            CloseWindow();
        }

        public override void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (UserAgreesToClose)
            {
                e.Cancel = false; //go ahead and close!
                Dispose();
                return;
            }

            if (HasUnSavedCategories() || HasUnSavedAccounts() && (UserAgreesToClose == false))
            {
                var result = MessageBox.Show(
                    "Are you sure, you want to close without Saving?",
                    "Confirm Close",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                UserAgreesToClose = (result == MessageBoxResult.Yes);

                if (UserAgreesToClose)
                {
                    e.Cancel = false; //go ahead and close!
                    Dispose();
                    return;
                }
                else
                {
                    e.Cancel = true; //stop window from closing
                    return;
                }
            }

            e.Cancel = false; //go ahead and close!
            Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            _categoriesComboBox = null;
            _allCategories = null;
            _expandedCategoryNodes = null;
            _expandedAccountNodes = null;

            PropertyChanged -= AccountMgrViewModel_PropertyChanged;

            base.Dispose(disposing);
        }

        #endregion Closing
    }
}