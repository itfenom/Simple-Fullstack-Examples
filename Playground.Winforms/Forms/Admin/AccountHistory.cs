using System.Windows.Forms;
using Playground.Core.AdoNet;
using Playground.Core.ColumnFilters.Columns;
using Playground.Core.ColumnFilters.Filters;

namespace Playground.Winforms.Forms.Admin
{
    public partial class AccountHistory : BaseForm
    {
        // ReSharper disable once NotAccessedField.Local
        private DialogResult _iAnswer;
        private DataGridViewTextBoxColumnFiltered _txtBoxColumn;

        public AccountHistory()
        {
            InitializeComponent();

            SetupGridViewColumns();
        }

        private void SetupGridViewColumns()
        {
            _txtBoxColumn = new DataGridViewTextBoxColumnFiltered();
            _txtBoxColumn.FilterType = ColumnFilterType.TextBoxFilter;
            _txtBoxColumn.Name = "colTransType";
            _txtBoxColumn.DataPropertyName = "TRANSACTION_TYPE";
            _txtBoxColumn.HeaderText = @"Trans Type";
            _txtBoxColumn.ReadOnly = true;
            _txtBoxColumn.Visible = true;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvAccountHistory.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumnFiltered();
            _txtBoxColumn.FilterType = ColumnFilterType.TextBoxFilter;
            _txtBoxColumn.Name = "colTransTimestamp";
            _txtBoxColumn.DataPropertyName = "TIME_STAMP";
            _txtBoxColumn.HeaderText = @"Time Stamp";
            _txtBoxColumn.ReadOnly = true;
            _txtBoxColumn.Visible = true;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvAccountHistory.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumnFiltered();
            _txtBoxColumn.FilterType = ColumnFilterType.TextBoxFilter;
            _txtBoxColumn.Name = "colID";
            _txtBoxColumn.DataPropertyName = "ID";
            _txtBoxColumn.HeaderText = @"ID";
            _txtBoxColumn.ReadOnly = true;
            _txtBoxColumn.Visible = true;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvAccountHistory.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumnFiltered();
            _txtBoxColumn.FilterType = ColumnFilterType.TextBoxFilter;
            _txtBoxColumn.Name = "colAcctName";
            _txtBoxColumn.DataPropertyName = "ACCT_NAME";
            _txtBoxColumn.HeaderText = @"Name";
            _txtBoxColumn.ReadOnly = true;
            _txtBoxColumn.Visible = true;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvAccountHistory.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumnFiltered();
            _txtBoxColumn.FilterType = ColumnFilterType.TextBoxFilter;
            _txtBoxColumn.Name = "colAcctLoginID";
            _txtBoxColumn.DataPropertyName = "ACCT_LOGIN_ID";
            _txtBoxColumn.HeaderText = @"Login ID";
            _txtBoxColumn.ReadOnly = true;
            _txtBoxColumn.Visible = true;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvAccountHistory.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumnFiltered();
            _txtBoxColumn.FilterType = ColumnFilterType.TextBoxFilter;
            _txtBoxColumn.Name = "colAcctPassword";
            _txtBoxColumn.DataPropertyName = "ACCT_PASSWORD";
            _txtBoxColumn.HeaderText = @"Password";
            _txtBoxColumn.ReadOnly = true;
            _txtBoxColumn.Visible = true;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvAccountHistory.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumnFiltered();
            _txtBoxColumn.FilterType = ColumnFilterType.TextBoxFilter;
            _txtBoxColumn.Name = "colAcctNotes";
            _txtBoxColumn.DataPropertyName = "ACCT_NOTES";
            _txtBoxColumn.HeaderText = @"Notes";
            _txtBoxColumn.ReadOnly = true;
            _txtBoxColumn.Visible = true;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvAccountHistory.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumnFiltered();
            _txtBoxColumn.FilterType = ColumnFilterType.TextBoxFilter;
            _txtBoxColumn.Name = "colDateCreated";
            _txtBoxColumn.DataPropertyName = "DATE_CREATED";
            _txtBoxColumn.HeaderText = @"Created on";
            _txtBoxColumn.ReadOnly = true;
            _txtBoxColumn.Visible = true;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvAccountHistory.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumnFiltered();
            _txtBoxColumn.FilterType = ColumnFilterType.TextBoxFilter;
            _txtBoxColumn.Name = "colDateModified";
            _txtBoxColumn.DataPropertyName = "DATE_Modified";
            _txtBoxColumn.HeaderText = @"Modified on";
            _txtBoxColumn.ReadOnly = true;
            _txtBoxColumn.Visible = true;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvAccountHistory.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumnFiltered();
            _txtBoxColumn.FilterType = ColumnFilterType.TextBoxFilter;
            _txtBoxColumn.Name = "colCategoryId";
            _txtBoxColumn.DataPropertyName = "CATEGORY_ID";
            _txtBoxColumn.HeaderText = @"Category Id";
            _txtBoxColumn.ReadOnly = true;
            _txtBoxColumn.Visible = true;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            dgvAccountHistory.Columns.Add(_txtBoxColumn);

            dgvAccountHistory.AutoGenerateColumns = false;
        }

        public void ShowForm(Form oParent)
        {
            LoadData();
            _iAnswer = ShowDialog(oParent);
            Close();
        }

        private void LoadData()
        {
            var dt = DAL.Seraph.ExecuteQuery("SELECT * FROM ACCT_HISTORY ORDER BY TIME_STAMP DESC");

            if (dt.Rows.Count > 0)
            {
                var bs = new BindingSource();
                bs.DataSource = dt;
                dgvAccountHistory.DataSource = bs;
            }
        }
    }
}
