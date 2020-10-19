using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Playground.Core.AdoNet;
using Playground.Core.Utilities;
using Playground.Winforms.Utilities;
// ReSharper disable InconsistentNaming

namespace Playground.Winforms.Forms.Admin
{
    public partial class AccountMgr : BaseForm
    {
        private DataTable _categoryDt;
        private DataTable _accountDt;

        private AccountTreeNode _rootNode;
        private AccountTreeNode _mainCategoryNode;
        private AccountTreeNode _subCategoryAccountNode;
        private AccountTreeNode _mainAccountNode;
        private AccountTreeNode _subAccountNode;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private BindingSource _bsCategory = new BindingSource();
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private BindingSource _bsAccount = new BindingSource();

        private DataGridViewTextBoxColumn _txtBoxColumn;
        private string _sql = string.Empty;

        //---------------
        private int _oldTabIndex = -1;
        private int _toTabIndex = -1;

        public AccountMgr()
        {
            InitializeComponent();
        }

        private void AccountMgr_Load(object sender, EventArgs e)
        {
            SetupColumnsInCategoryGridView();
            SetupColumnsInAccountGridView();
            LoadData();
            LoadTree();
        }

        #region Setup Columns in GridViews
        private void SetupColumnsInCategoryGridView()
        {
            _txtBoxColumn = new DataGridViewTextBoxColumn();
            _txtBoxColumn.Name = "colCategoryID";
            _txtBoxColumn.DataPropertyName = "CATEGORY_ID";
            _txtBoxColumn.HeaderText = @"Category ID";
            _txtBoxColumn.ReadOnly = true;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvCategories.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumn();
            _txtBoxColumn.Name = "colCategoryName";
            _txtBoxColumn.DataPropertyName = "CATEGORY_NAME";
            _txtBoxColumn.HeaderText = @"Category Name";
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvCategories.Columns.Add(_txtBoxColumn);

            dgvCategories.AutoGenerateColumns = false;
        }

        private void SetupColumnsInAccountGridView()
        {
            _txtBoxColumn = new DataGridViewTextBoxColumn();
            _txtBoxColumn.Name = "colID";
            _txtBoxColumn.DataPropertyName = "ID";
            _txtBoxColumn.HeaderText = @"ID";
            _txtBoxColumn.ReadOnly = true;
            _txtBoxColumn.Visible = false;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvAccounts.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumn();
            _txtBoxColumn.Name = "colAcctName";
            _txtBoxColumn.DataPropertyName = "ACCT_NAME";
            _txtBoxColumn.HeaderText = @"Name";
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvAccounts.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumn();
            _txtBoxColumn.Name = "colAcctLoginID";
            _txtBoxColumn.DataPropertyName = "ACCT_LOGIN_ID";
            _txtBoxColumn.HeaderText = @"Login ID";
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvAccounts.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumn();
            _txtBoxColumn.Name = "colAcctPassword";
            _txtBoxColumn.DataPropertyName = "ACCT_PASSWORD";
            _txtBoxColumn.HeaderText = @"Password";
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvAccounts.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumn();
            _txtBoxColumn.Name = "colAcctNotes";
            _txtBoxColumn.DataPropertyName = "ACCT_NOTES";
            _txtBoxColumn.HeaderText = @"Notes";
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvAccounts.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumn();
            _txtBoxColumn.Name = "colDateCreated";
            _txtBoxColumn.DataPropertyName = "DATE_CREATED";
            _txtBoxColumn.HeaderText = @"Created on";
            _txtBoxColumn.ReadOnly = true;
            _txtBoxColumn.Visible = false;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvAccounts.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumn();
            _txtBoxColumn.Name = "colDateModified";
            _txtBoxColumn.DataPropertyName = "DATE_Modified";
            _txtBoxColumn.HeaderText = @"Modified on";
            _txtBoxColumn.ReadOnly = true;
            _txtBoxColumn.Visible = false;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvAccounts.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumn();
            _txtBoxColumn.Name = "colIsPasswordEncrypted";
            _txtBoxColumn.DataPropertyName = "IS_PASSWORD_ENCRYPTED";
            _txtBoxColumn.HeaderText = @"Password Encrypted";
            _txtBoxColumn.ReadOnly = true;
            _txtBoxColumn.Visible = false;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvAccounts.Columns.Add(_txtBoxColumn);

            dgvAccounts.AutoGenerateColumns = false;
        }
        #endregion

        #region Load DataTables
        private void LoadData()
        {
            _sql = "SELECT * FROM ACCT_CATEGORY ORDER BY CATEGORY_NAME";
            _categoryDt = new DataTable();
            _categoryDt = DAL.Seraph.ExecuteQuery(_sql);

            _sql = "SELECT * FROM ACCT_MGR ORDER BY CATEGORY_ID, ACCT_NAME";
            _accountDt = DAL.Seraph.ExecuteQuery(_sql);
            //_accountDt.Columns.Add(new DataColumn("IS_PASSWORD_ENCRYPTED", typeof(string)));

            //foreach (DataRow row in _accountDt.Rows)
            //{
            //    var decryptedPassword = Encryption.Decrypt(row["ACCT_PASSWORD"].ToString());
            //    row["IS_PASSWORD_ENCRYPTED"] = (string.IsNullOrEmpty(decryptedPassword) ? "N" : "Y");
            //}

            _accountDt.AcceptChanges();
        }
        #endregion

        #region Load TreeView
        private void LoadTree()
        {
            trvMain.Nodes.Clear();
            _rootNode = new AccountTreeNode();
            _rootNode.Text = @"Categories";
            _rootNode.ToolTipText = "Categories";
            _rootNode.NodeLevel = AccountTreeLevel.DEFAULT_CATEGORY;
            _rootNode.ImageIndex = (int)AccountTreeIcons.Object;
            _rootNode.SelectedImageIndex = (int)AccountTreeIcons.Object;

            foreach (DataRow item in _categoryDt.Rows)
            {
                _mainCategoryNode = new AccountTreeNode();
                _mainCategoryNode.Text = item["CATEGORY_NAME"].ToString();
                _mainCategoryNode.ToolTipText = item["CATEGORY_NAME"].ToString();
                _mainCategoryNode.CategoryId = Convert.ToInt32(item["CATEGORY_ID"].ToString());
                _mainCategoryNode.NodeLevel = AccountTreeLevel.MAIN_CATEGORY;

                _subCategoryAccountNode = new AccountTreeNode();
                _subCategoryAccountNode.Text = @"Accounts";
                _subCategoryAccountNode.ToolTipText = "Accounts";
                _subCategoryAccountNode.CategoryId = Convert.ToInt32(item["CATEGORY_ID"].ToString());
                _subCategoryAccountNode.CategoryName = item["CATEGORY_NAME"].ToString();
                _subCategoryAccountNode.NodeLevel = AccountTreeLevel.SUB_CATEGORY_ACCOUNT;

                if (!IsNodeAlreadyExist(_mainCategoryNode, "Accounts"))
                {
                    AddNode(_mainCategoryNode, _subCategoryAccountNode, Convert.ToInt32(AccountTreeIcons.Object));
                    LoadMainAccounts(_subCategoryAccountNode);
                }

                AddNode(_rootNode, _mainCategoryNode, Convert.ToInt32(AccountTreeIcons.Array));
            }

            _rootNode.Expand();
            trvMain.ShowNodeToolTips = true;
            trvMain.Nodes.Add(_rootNode);
        }

        private void LoadMainAccounts(AccountTreeNode parentNode)
        {
            foreach (DataRow item in _accountDt.Rows)
            {
                if (Convert.ToInt32(item["CATEGORY_ID"]) == parentNode.CategoryId)
                {
                    _mainAccountNode = new AccountTreeNode();

                    _mainAccountNode.Text = item["ACCT_NAME"].ToString();
                    _mainAccountNode.ToolTipText = item["ACCT_NAME"].ToString();
                    _mainAccountNode.CategoryId = Convert.ToInt32(item["CATEGORY_ID"]);
                    _mainAccountNode.AccountName = item["ACCT_NAME"].ToString();
                    _mainAccountNode.ID = Convert.ToInt32(item["ID"]);
                    _mainAccountNode.NodeLevel = AccountTreeLevel.SUB_ACCOUNT;

                    if (!IsNodeAlreadyExist(parentNode, item["ACCT_NAME"].ToString()))
                    {
                        AddNode(parentNode, _mainAccountNode, Convert.ToInt32(AccountTreeIcons.Array));
                        LoadSubAccounts(_mainAccountNode);
                    }
                }
            }
        }

        private void LoadSubAccounts(AccountTreeNode parentNode)
        {
            DataRow[] rows = _accountDt.Select("ID = " + parentNode.ID);

            if (rows.Length > 0)
            {
                string notes = string.Empty;

                var loginId = rows[0]["ACCT_LOGIN_ID"].ToString();
                var acctPassword = rows[0]["ACCT_PASSWORD"].ToString();
                var dateCreated = (rows[0]["DATE_CREATED"] == DBNull.Value ? null : Convert.ToDateTime(rows[0]["DATE_CREATED"].ToString()).ToString("MM/dd/yyyy"));
                var dateModified = (rows[0]["DATE_MODIFIED"] == DBNull.Value ? null : Convert.ToDateTime(rows[0]["DATE_MODIFIED"].ToString()).ToString("MM/dd/yyyy"));

                if (!IsNodeAlreadyExist(parentNode, loginId))
                {
                    _subAccountNode = new AccountTreeNode();
                    AddAttributeNode(parentNode, _subAccountNode, "LOGIN ID: " + loginId, loginId);
                }

                if (!IsNodeAlreadyExist(parentNode, acctPassword))
                {
                    _subAccountNode = new AccountTreeNode();
                    AddAttributeNode(parentNode, _subAccountNode, "PASSWORD: " + acctPassword, acctPassword);
                }

                if (!IsNodeAlreadyExist(parentNode, dateCreated))
                {
                    _subAccountNode = new AccountTreeNode();
                    AddAttributeNode(parentNode, _subAccountNode, "DATE CREATED: " + dateCreated, dateCreated);
                }

                if (!IsNodeAlreadyExist(parentNode, dateModified))
                {
                    _subAccountNode = new AccountTreeNode();
                    AddAttributeNode(parentNode, _subAccountNode, "DATE MODIFIED: " + dateModified, dateModified);
                }

                if (rows[0]["ACCT_NOTES"] != DBNull.Value)
                {
                    notes = rows[0]["ACCT_NOTES"].ToString();
                    if (notes.Length > 10)
                    {
                        notes = notes.Substring(0, 7) + "...";
                    }
                }

                if (!IsNodeAlreadyExist(parentNode, notes))
                {

                    _subAccountNode = new AccountTreeNode();
                    AddAttributeNode(parentNode, _subAccountNode, "NOTES:" + notes, notes);
                }
            }
        }

        private bool IsNodeAlreadyExist(AccountTreeNode parentNode, string nodeTextVal)
        {
            bool retVal = false;
            for (int i = 0; i < parentNode.Nodes.Count; i++)
            {
                if (parentNode.Nodes[i].Text == nodeTextVal)
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        private void AddNode(AccountTreeNode parentNode, AccountTreeNode childNode, int imageIndex)
        {
            childNode.ImageIndex = imageIndex;
            childNode.SelectedImageIndex = imageIndex;
            parentNode.Nodes.Add(childNode);
        }

        private void AddAttributeNode(AccountTreeNode parentNode, AccountTreeNode attributeNode, string text, string toolTip, int treeIcon = 2)
        {
            attributeNode.Text = text;
            attributeNode.NodeLevel = AccountTreeLevel.SUB_ACCOUNT;
            attributeNode.ToolTipText = toolTip;
            attributeNode.ImageIndex = treeIcon;
            attributeNode.SelectedImageIndex = treeIcon;
            parentNode.Nodes.Add(attributeNode);
        }
        #endregion

        #region TreeView Events
        private void trvMain_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            HandleNodeSelection(e.Node as AccountTreeNode);
        }

        private void HandleNodeSelection(AccountTreeNode oNode)
        {
            if (HasChanges())
            {
                if (MessageBox.Show(@"There are pending changes.\nContinue without saving?", @"Pending Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // ReSharper disable once UnusedVariable
                    string toIndex = _oldTabIndex.ToString();
                    string fromIndex = _toTabIndex.ToString();

                    LoadData();

                    if (fromIndex == "0")
                    {
                        LoadAllCategoriesIntoGridView();
                    }
                    else if (fromIndex == "1")
                    {
                        LoadAllAccountIntoGridView(oNode);
                    }

                    switch (oNode.NodeLevel)
                    {
                        case AccountTreeLevel.DEFAULT_CATEGORY:
                            LoadAllCategoriesIntoGridView();
                            break;
                        case AccountTreeLevel.MAIN_CATEGORY:
                            LoadSubCategoriesIntoGridView(oNode);
                            break;
                        case AccountTreeLevel.SUB_CATEGORY_ACCOUNT:
                            LoadAllAccountIntoGridView(oNode);
                            break;
                        case AccountTreeLevel.SUB_ACCOUNT:
                            LoadSubAccountIntoGridView(oNode);
                            break;
                    }
                }
                else
                {
                    // ReSharper disable once RedundantJumpStatement
                    return;
                }
            }
            else
            {
                switch (oNode.NodeLevel)
                {
                    case AccountTreeLevel.DEFAULT_CATEGORY:
                        LoadAllCategoriesIntoGridView();
                        break;
                    case AccountTreeLevel.MAIN_CATEGORY:
                        LoadSubCategoriesIntoGridView(oNode);
                        break;
                    case AccountTreeLevel.SUB_CATEGORY_ACCOUNT:
                        LoadAllAccountIntoGridView(oNode);
                        break;
                    case AccountTreeLevel.SUB_ACCOUNT:
                        LoadSubAccountIntoGridView(oNode);
                        break;
                }
            }
        }
        #endregion

        #region Load GridViews
        private void LoadAllCategoriesIntoGridView()
        {
            tabControl1.SelectedTab = tabPageCategory;
            dgvCategories.DataSource = null;
            _bsCategory.DataSource = null;
            _bsCategory.Filter = null;
            _bsCategory.DataSource = _categoryDt;
            dgvCategories.DataSource = _bsCategory;
            bnCategories.BindingSource = _bsCategory;
        }

        private void LoadSubCategoriesIntoGridView(AccountTreeNode parentNode)
        {
            tabControl1.SelectedTab = tabPageCategory;
            _bsCategory.DataSource = null;
            dgvCategories.DataSource = null;

            _bsCategory.DataSource = _categoryDt;
            //_bsCategory.Filter = "CATEGORY_ID = " + parentNode.CategoryID;
            dgvCategories.DataSource = _bsCategory;
            bnCategories.BindingSource = _bsCategory;

            foreach (DataGridViewRow r in dgvCategories.Rows)
            {
                if (r.Cells["colCategoryID"].Value.ToString() == parentNode.CategoryId.ToString())
                {
                    r.Selected = true;
                }
                else
                {
                    r.Selected = false;
                }
            }
        }

        private void LoadAllAccountIntoGridView(AccountTreeNode parentNode)
        {
            tabControl1.SelectedTab = tabPageAccounts;
            _bsAccount.DataSource = null;
            dgvAccounts.DataSource = null;

            _bsAccount.Filter = "CATEGORY_ID = " + parentNode.CategoryId;
            _bsAccount.DataSource = _accountDt;
            dgvAccounts.DataSource = _bsAccount;
            bnAccounts.BindingSource = _bsAccount;
        }

        private void LoadSubAccountIntoGridView(AccountTreeNode parentNode)
        {
            tabControl1.SelectedTab = tabPageAccounts;
            _bsAccount.DataSource = null;
            dgvAccounts.DataSource = null;

            _bsAccount.Filter = "CATEGORY_ID = " + parentNode.CategoryId;
            _bsAccount.DataSource = _accountDt;
            dgvAccounts.DataSource = _bsAccount;
            bnAccounts.BindingSource = _bsAccount;

            foreach (DataGridViewRow r in dgvAccounts.Rows)
            {
                if (r.Cells["colID"].Value.ToString() == parentNode.ID.ToString())
                {
                    r.Selected = true;
                }
                else
                {
                    r.Selected = false;
                }
            }
        }
        #endregion

        #region Category GridView: Insert/Update/Delete
        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            var selectedNode = trvMain.SelectedNode;

            if (selectedNode == null) return;

            var catNode = (AccountTreeNode)selectedNode;

            if (catNode.NodeLevel == AccountTreeLevel.MAIN_CATEGORY || catNode.NodeLevel == AccountTreeLevel.DEFAULT_CATEGORY)
            {
                dgvCategories.EndEdit();
                DataRow categoryRow = _categoryDt.NewRow();
                categoryRow["CATEGORY_ID"] = 0;
                categoryRow["CATEGORY_NAME"] = string.Empty;
                _categoryDt.Rows.Add(categoryRow);
            }
            else
            {
                Helpers.DisplayInfo(this, "Invalid tree node selected!", "Adding New Category");
            }
        }

        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            dgvCategories.EndEdit();

            if (dgvCategories.SelectedRows.Count == 0)
            {
                MessageBox.Show(@"Please select a row to delete.", @"Delete Category", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var dataRowView = dgvCategories.SelectedRows[0].DataBoundItem as DataRowView;

            // ReSharper disable once UseNullPropagation
            if (dataRowView != null)
            {
                dataRowView.Row.Delete();
            }
            //TreeNode _selectedNode = trvMain.SelectedNode;

            //if (_selectedNode == null) return;

            //AccountTreeNode _catNode = (AccountTreeNode)_selectedNode;

            //if (_catNode.NodeLevel == AccountTreeLevel.MAIN_CATEGORY || _catNode.NodeLevel == AccountTreeLevel.DEFAULT_CATEGORY)
            //{
            //    DataRowView _dataRowView = dgvCategories.CurrentRow.DataBoundItem as DataRowView;

            //    if (_dataRowView != null)
            //    {
            //        _dataRowView.Row.Delete();
            //    }
            //}
            //else
            //{
            //    HelperTools.DisplayInfo(this, "Deleting a Category", "Invalid tree node selected!");
            //}
        }

        private void btnSaveCategory_Click(object sender, EventArgs e)
        {
            SaveCategories();
        }

        private void dgvCategories_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private string GetCategoryName()
        {
            string retVal = string.Empty;
            dgvCategories.EndEdit();
            foreach (DataGridViewRow row in dgvCategories.Rows)
            {
                if (row.Cells["colCategoryName"].Value != null)
                {
                    if (row.Cells["colCategoryName"].Value.ToString().Length > 20)
                    {
                        retVal = row.Cells["colCategoryName"].Value.ToString();
                    }
                }
            }
            return retVal;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool SaveCategories()
        {
            string oErrors = string.Empty;
            var categoryName = GetCategoryName();

            if (!string.IsNullOrEmpty(categoryName))
            {
                Helpers.DisplayInfo(this, "Category name\n" + categoryName + "\nis too long!", "Save Category");
                return false;
            }

            dgvCategories.EndEdit();
            _bsCategory.EndEdit();

            if (HasChanges())
            {
                var dt = _categoryDt.GetChanges();
                // ReSharper disable once PossibleNullReferenceException
                foreach (DataRow row in dt.Rows)
                {
                    if (row.RowState == DataRowState.Added)
                    {
                        if (string.IsNullOrEmpty(row["CATEGORY_NAME"].ToString()))
                        {
                            oErrors = "Category is required!";
                        }

                        if (_categoryDt.Select("CATEGORY_NAME ='" + row["CATEGORY_NAME"].ToString().ToUpper() + "'").Length > 1)
                        {
                            oErrors = $"Category name '{row["CATEGORY_NAME"].ToString().ToUpper()}' already exist!";
                        }

                        if (!string.IsNullOrEmpty(oErrors))
                        {
                            break;
                        }

                        var newCategoryId = Core.Repositories.ManageAccountsRepository.InsertNewCategory(row["CATEGORY_NAME"].ToString());

                        if (newCategoryId != 0)
                        {
                            row["Category_ID"] = newCategoryId;
                        }
                    }
                    else if (row.RowState == DataRowState.Modified)
                    {
                        if (_categoryDt.Select("CATEGORY_NAME ='" + row["CATEGORY_NAME"].ToString().ToUpper() + "'").Length > 1)
                        {
                            oErrors = $"Category name '{row["CATEGORY_NAME"].ToString().ToUpper()}' already exist!";
                            break;
                        }

                        Core.Repositories.ManageAccountsRepository.UpdateCategory(row["CATEGORY_NAME"].ToString(), Convert.ToInt32(row["CATEGORY_ID"].ToString()));
                    }
                    else if (row.RowState == DataRowState.Deleted)
                    {
                        int idToBeDeleted = Convert.ToInt32(row["CATEGORY_ID", DataRowVersion.Original]);

                        if (!IsChildRecordExist(idToBeDeleted))
                        {
                            Core.Repositories.ManageAccountsRepository.DeleteCategory(idToBeDeleted);
                        }
                        else
                        {
                            oErrors = "Child record exist. Delete failed!";
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(oErrors))
                    {
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(oErrors))
                {
                    Helpers.DisplayInfo(this, oErrors);
                    return false;
                }

                Helpers.DisplayInfo(this, "Changes saved successfully!", "Save Category");
                _categoryDt.AcceptChanges();
                RefreshData();
            }
            else
            {
                Helpers.DisplayInfo(this, "Nothing to save!", "Save Category");
            }


            return false;
        }

        private bool IsChildRecordExist(int categoryId)
        {
            bool retVal = true;
            _sql = string.Empty;

            _sql = "SELECT COUNT(*) FROM ACCT_MGR WHERE CATEGORY_ID = " + categoryId;
            var count = Convert.ToInt32(DAL.Seraph.ExecuteScalar(_sql));

            if (count == 0)
            {
                retVal = false;
            }

            return retVal;
        }
        #endregion

        #region Account GridView: Insert/Update/Delete
        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            var selectedNode = trvMain.SelectedNode;

            if (selectedNode == null) return;

            var acctNode = (AccountTreeNode)selectedNode;

            if (acctNode.NodeLevel == AccountTreeLevel.SUB_CATEGORY_ACCOUNT || acctNode.NodeLevel == AccountTreeLevel.SUB_ACCOUNT)
            {
                dgvAccounts.EndEdit();
                DataRow acctRow = _accountDt.NewRow();
                acctRow["ID"] = 0;
                acctRow["ACCT_NAME"] = string.Empty;
                acctRow["ACCT_LOGIN_ID"] = string.Empty;
                acctRow["ACCT_PASSWORD"] = string.Empty;
                acctRow["ACCT_NOTES"] = string.Empty;
                acctRow["DATE_CREATED"] = DateTime.Today.Date;
                acctRow["DATE_MODIFIED"] = DateTime.Today.Date;
                acctRow["CATEGORY_ID"] = acctNode.CategoryId;
                acctRow["IS_PASSWORD_ENCRYPTED"] = 'N';
                _accountDt.Rows.Add(acctRow);
            }
            else
            {
                Helpers.DisplayInfo(this, "Invalid tree node selected!", "Add New Account");
            }
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            dgvAccounts.EndEdit();

            if (dgvAccounts.SelectedRows.Count == 0)
            {
                MessageBox.Show(@"Please select a row to delete.", @"Delete Account", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var dataRowView = dgvAccounts.SelectedRows[0].DataBoundItem as DataRowView;

            // ReSharper disable once UseNullPropagation
            if (dataRowView != null)
            {
                dataRowView.Row.Delete();
            }

            //TreeNode _selectedNode = trvMain.SelectedNode;

            //if (_selectedNode == null) return;

            //AccountTreeNode _acctNode = (AccountTreeNode)_selectedNode;

            //if (_acctNode.NodeLevel == AccountTreeLevel.SUB_CATEGORY_ACCOUNT || _acctNode.NodeLevel == AccountTreeLevel.SUB_ACCOUNT)
            //{
            //    DataRowView _dataRowView = dgvAccounts.CurrentRow.DataBoundItem as DataRowView;

            //    if (_dataRowView != null)
            //    {
            //        _dataRowView.Row.Delete();
            //    }
            //}
            //else
            //{
            //    HelperTools.DisplayInfo(this, "Invalid tree node selected!", "Delete Account");
            //}
        }

        private void btnSaveAccount_Click(object sender, EventArgs e)
        {
            SaveAccounts();
        }

        //Double click in comments cell to view entire comments in a dialog!
        private void dgvAccounts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (dgvAccounts.CurrentCell.ColumnIndex == 4)
                {
                    var currentNotesVal = dgvAccounts.Rows[e.RowIndex].Cells["colAcctNotes"].Value.ToString();
                    var oForm = new FreeTextForm();
                    string newNotesVal = oForm.ShowForm(this, currentNotesVal);

                    if (!string.IsNullOrEmpty(newNotesVal))
                    {
                        dgvAccounts.Rows[e.RowIndex].Cells["colAcctNotes"].Value = newNotesVal;
                    }
                    else
                    {
                        dgvAccounts.Rows[e.RowIndex].Cells["colAcctNotes"].Value = currentNotesVal;
                    }
                }
            }
        }

        private void dgvAccounts_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Ignore if a column or row header is clicked
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                if (e.Button == MouseButtons.Right)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    DataGridViewCell clickedCell = (sender as DataGridView).Rows[e.RowIndex].Cells[e.ColumnIndex];

                    //Select the clicked cell
                    dgvAccounts.CurrentCell = clickedCell;

                    if (clickedCell.ColumnIndex == 3)
                    {
                        //Set the selected row to true
                        dgvAccounts.Rows[e.RowIndex].Selected = true;

                        // Get mouse position relative to the vehicles grid
                        var relativeMousePosition = dgvAccounts.PointToClient(Cursor.Position);

                        //Create context menu
                        ContextMenu rightClickMenu = new ContextMenu();

                        MenuItem miShowDetails = new MenuItem("Show Details");
                        miShowDetails.Click += miShowDetails_Click;
                        rightClickMenu.MenuItems.Add(miShowDetails);

                        //Continent info
                        rightClickMenu.Show(dgvAccounts, relativeMousePosition);
                    }
                }
            }
        }

        private void miShowDetails_Click(object sender, EventArgs e)
        {
            if (dgvAccounts.SelectedRows.Count > 0)
            {
                // ReSharper disable once PossibleNullReferenceException
                string passwordVal = dgvAccounts.SelectedRows[0].Cells[3].FormattedValue.ToString();
                // ReSharper disable once PossibleNullReferenceException
                string isPasswordEncrypted = dgvAccounts.SelectedRows[0].Cells["colIsPasswordEncrypted"].FormattedValue.ToString();

                if (isPasswordEncrypted.ToUpper() == "Y")
                {
                    passwordVal = Encryption.Decrypt(passwordVal);
                }

                var detail =
                    $"Name: {dgvAccounts.SelectedRows[0].Cells[1].FormattedValue}" +
                    $"\nLogin: {dgvAccounts.SelectedRows[0].Cells[2].FormattedValue}" +
                    $"\nPassword: {passwordVal}" +
                    $"\nNotes: {dgvAccounts.SelectedRows[0].Cells[4].FormattedValue}\n";

                Helpers.DisplayInfo(this, detail, "View Detail");
            }
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool SaveAccounts()
        {
            dgvAccounts.EndEdit();
            _bsAccount.EndEdit();

            if (HasChanges())
            {
                var dtChanges = _accountDt.GetChanges();
                if (!ValidateAccountData(dtChanges)) return false;

                var oErr = new ArrayList();
                var insertDataView = new DataView(dtChanges);
                var updateDataView = new DataView(dtChanges);
                var deleteDataView = new DataView(dtChanges);

                insertDataView.RowStateFilter = DataViewRowState.Added;
                updateDataView.RowStateFilter = DataViewRowState.ModifiedCurrent;
                deleteDataView.RowStateFilter = DataViewRowState.Deleted;

                if (insertDataView.Count > 0)
                {
                    oErr = GetDuplicateAccountNames(insertDataView);

                    if (oErr.Count > 0)
                    {
                        Helpers.DisplayMultipleErrors(this, oErr);
                        return false;
                    }

                    for (int i = 0; i < insertDataView.Count; i++)
                    {
                        string password;
                        if (insertDataView[i]["IS_PASSWORD_ENCRYPTED"].ToString() == "N")
                        {
                            password = Encryption.Encrypt(insertDataView[i]["ACCT_PASSWORD"].ToString());
                        }
                        else
                        {
                            password = insertDataView[i]["ACCT_PASSWORD"].ToString();
                        }

                        var newAccountId = Core.Repositories.ManageAccountsRepository.InsertNewAccount
                                    (HelperTools.FormatSqlString(insertDataView[i]["ACCT_NAME"].ToString()),
                                        HelperTools.FormatSqlString(insertDataView[i]["ACCT_LOGIN_ID"].ToString()),
                                        HelperTools.FormatSqlString(password),
                                        HelperTools.FormatSqlString(insertDataView[i]["ACCT_NOTES"].ToString()),
                                        Convert.ToInt32(insertDataView[i]["CATEGORY_ID"].ToString())
                                    );

                        if (newAccountId > 0)
                        {
                            //Update the id in the gridView/DataTable or reload everything.
                        }
                    }
                }

                if (updateDataView.Count > 0)
                {
                    oErr.Clear();
                    oErr = GetDuplicateAccountNames(updateDataView);

                    if (oErr.Count > 0)
                    {
                        Helpers.DisplayMultipleErrors(this, oErr);
                        return false;
                    }

                    var sb = new StringBuilder();
                    _sql = string.Empty;

                    sb.Append("MERGE INTO ACCT_MGR O\nUSING (");

                    for (int i = 0; i < updateDataView.Count; i++)
                    {
                        string passwordToUpdate;

                        var thisPassword = updateDataView[i]["ACCT_PASSWORD"].ToString();
                        var accountId = Convert.ToInt32(updateDataView[i]["ID"]);
                        var savedPassword = GetPassword(accountId);

                        if (thisPassword.Trim() == savedPassword.Trim())
                        {
                            passwordToUpdate = thisPassword;
                        }
                        else
                        {
                            passwordToUpdate = Encryption.Encrypt(thisPassword);
                        }

                        sb.AppendLine(" SELECT " + updateDataView[i]["ID"] + " AS ID, ");
                        sb.AppendLine("'" + updateDataView[i]["ACCT_NAME"] + "' AS ACCT_NAME, ");
                        sb.AppendLine("'" + updateDataView[i]["ACCT_LOGIN_ID"] + "' AS ACCT_LOGIN_ID, ");
                        sb.AppendLine("'" + passwordToUpdate + "' AS ACCT_PASSWORD, ");
                        sb.AppendLine("'" + updateDataView[i]["ACCT_NOTES"] + "' AS ACCT_NOTES, ");
                        sb.AppendLine(updateDataView[i]["CATEGORY_ID"] + " AS CATEGORY_ID, ");
                        sb.AppendLine("NULL AS DATE_MODIFIED FROM DUAL");
                        sb.AppendLine(" UNION ALL ");
                    }

                    if (sb.Length > 0)
                    {
                        int index = sb.ToString().LastIndexOf("UNION ALL", StringComparison.Ordinal);
                        _sql = sb.ToString().Remove(index, 9);

                        _sql += @") U ON (U.ID = O.ID) 
                                WHEN MATCHED THEN UPDATE 
                                SET O.ACCT_NAME = U.ACCT_NAME, 
                                    O.ACCT_LOGIN_ID = U.ACCT_LOGIN_ID, 
                                    O.ACCT_PASSWORD = U.ACCT_PASSWORD, 
                                    O.ACCT_NOTES = U.ACCT_NOTES, 
                                    O.CATEGORY_ID = U.CATEGORY_ID,
                                    O.DATE_MODIFIED = SYSTIMESTAMP";

                        DAL.Seraph.ExecuteNonQuery(_sql);
                    }
                }

                if (deleteDataView.Count > 0)
                {
                    for (int i = 0; i < deleteDataView.Count; i++)
                    {
                        var accountId = Convert.ToInt32(deleteDataView[i]["ID"].ToString());
                        Core.Repositories.ManageAccountsRepository.DeleteAccount(accountId);
                    }
                }

                if (oErr.Count == 0)
                {
                    Helpers.DisplayInfo(this, "Changes saved successfully!", "Save Account(s)");
                    _accountDt.AcceptChanges();
                    RefreshData();
                }
            }
            else
            {
                Helpers.DisplayInfo(this, "Nothing to save!", "Save Account(s)");
            }

            return false;
        }

        private bool ValidateAccountData(DataTable dt)
        {
            bool retVal = false;
            var oErr = new ArrayList();
            foreach (DataRow row in dt.Rows)
            {
                if (row.RowState != DataRowState.Deleted)
                {
                    if (string.IsNullOrEmpty(row["ACCT_NAME"].ToString()))
                    {
                        oErr.Add("Account Name is required!");
                    }
                    if (string.IsNullOrEmpty(row["ACCT_LOGIN_ID"].ToString()))
                    {
                        oErr.Add("Account Login ID is required!");
                    }
                    if (string.IsNullOrEmpty(row["ACCT_PASSWORD"].ToString()))
                    {
                        oErr.Add("Account Password is required!");
                    }
                }

                if (oErr.Count > 0)
                {
                    break;
                }
            }

            if (oErr.Count > 0)
            {
                Helpers.DisplayMultipleErrors(this, oErr);
            }
            else
            {
                retVal = true;
            }

            return retVal;
        }

        private ArrayList GetDuplicateAccountNames(DataView dataView)
        {
            var retVal = new ArrayList();

            for (int i = 0; i < dataView.Count; i++)
            {
                var thisLoginId = dataView[i]["ACCT_LOGIN_ID"].ToString();
                string thisAcctName = HelperTools.FormatSqlString(dataView[i]["ACCT_NAME"].ToString());

                if (_accountDt.Select("ACCT_LOGIN_ID ='" + thisLoginId + "' AND ACCT_NAME = '" + thisAcctName + "'").Length > 1)
                {
                    retVal.Add($"Account name '{thisLoginId}' already exist in this category!");
                }
            }

            return retVal;
        }

        private string GetPassword(int accountId)
        {
            return DAL.Seraph.ExecuteScalar("SELECT ACCT_PASSWORD FROM ACCT_MGR WHERE ID = " + accountId).ToString();
        }

        #endregion

        #region Closing\Refresh
        private bool HasChanges()
        {
            bool retVal = false;

            var dtChanges = _categoryDt.GetChanges();

            if ((dtChanges != null) && (dtChanges.Rows.Count > 0))
            {
                retVal = true;
            }

            // ReSharper disable once RedundantAssignment
            dtChanges = new DataTable();
            dtChanges = _accountDt.GetChanges();

            if ((dtChanges != null) && (dtChanges.Rows.Count > 0))
            {
                retVal = true;
            }

            return retVal;
        }

        private void RefreshData()
        {
            Cursor = Cursors.WaitCursor;
            string fullPath = string.Empty;

            //Before refresh, save the state of the selected TreeNode
            var saveExpansionState = trvMain.Nodes.GetExpansionState();

            //Clear and Reload the TreeView
            LoadData();
            LoadTree();

            //Restore the state of the treeView
            trvMain.Nodes.SetExpansionState(saveExpansionState);

            trvMain.SelectedNode = FindNode(trvMain.Nodes, fullPath);

            trvMain.Focus();

            Cursor = Cursors.Default;
        }

        private TreeNode FindNode(TreeNodeCollection tncoll, string strText)
        {
            TreeNode tnFound;
            foreach (TreeNode tnCurr in tncoll)
            {
                if (tnCurr.FullPath == strText)
                {
                    return tnCurr;
                }
                tnFound = FindNode(tnCurr.Nodes, strText);
                if (tnFound != null)
                {
                    return tnFound;
                }
            }
            return null;
        }

        private void AccountMgr_FormClosing(object sender, FormClosingEventArgs e)
        {
            dgvCategories.EndEdit();
            _bsCategory.EndEdit();

            dgvAccounts.EndEdit();
            _bsAccount.EndEdit();

            if (HasChanges())
            {
                if (MessageBox.Show(@"There are pending changes, still want to exit?", @"Confirm Exit", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                Dispose();
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshData();
        }
        #endregion

        private void tabControl1_Deselected(object sender, TabControlEventArgs e)
        {
            _oldTabIndex = e.TabPageIndex;
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (_oldTabIndex != -1)
            {
                _toTabIndex = e.TabPageIndex;
            }
        }
    }

    public class AccountTreeNode : TreeNode
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ID { get; set; }
        public string AccountName { get; set; }
        public string LoginId { get; set; }
        public string Password { get; set; }
        public string Notes { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public AccountTreeLevel NodeLevel { get; set; }

        public string IsPasswordEncrypted { get; set; } = "N";
    }

    public enum AccountTreeIcons : byte
    {
        Object = 0,
        Array = 1,
        Attribute = 2
    }

    public enum AccountTreeLevel : byte
    {
        DEFAULT_CATEGORY = 0,
        MAIN_CATEGORY = 1,
        SUB_CATEGORY_ACCOUNT = 2,
        MAIN_ACCOUNT = 3,
        SUB_ACCOUNT = 4
    }
}
