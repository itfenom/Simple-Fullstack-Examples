using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Playground.Core.AdoNet;

// ReSharper disable PossibleNullReferenceException

namespace Playground.Winforms.Forms.Examples
{
    public partial class DataGridComboBox : Form
    {
        // ReSharper disable once NotAccessedField.Local
        private DialogResult _iAnswer;
        private DataTable _mainDt;
        private DataTable _rolesDt;
        private string _sql = string.Empty;
        private DataGridViewCheckBoxColumn _checkBoxCol;
        private DataGridViewTextBoxColumn _txtBoxCol;
        private DataGridViewComboBoxColumn _comboBoxCol;

        public DataGridComboBox()
        {
            InitializeComponent();
        }

        public void ShowForm(Form oParent)
        {
            //Setup Columns in the GridView!
            SetupGridViewColumns();

            //Load DataTables
            LoadData();

            //Check 3rd and 5th item
            PreSelectCheckBoxes();

            //Set the DataGridView's DataSource
            dgvMain.DataSource = _mainDt;

            //Selecting item in the combo-box must be handled in Form_Load Event!!! (Important)

            _iAnswer = ShowDialog(oParent);

            Close();
        }

        private void SetupGridViewColumns()
        {
            //Check-Box Column
            _checkBoxCol = new DataGridViewCheckBoxColumn
            {
                Name = "colCheckBoxCol",
                HeaderText = @"Select",
                DataPropertyName = "SELECT",
                Width = 100
            };
            dgvMain.Columns.Add(_checkBoxCol);
            dgvMain.Columns["colCheckBoxCol"].Visible = true;

            //ID Column
            _txtBoxCol = new DataGridViewTextBoxColumn
            { Name = "colID",
                HeaderText = @"ID",
                DataPropertyName = "ID"

            };
            dgvMain.Columns.Add(_txtBoxCol);
            dgvMain.Columns["colID"].Visible = false;

            //Login_Name Column
            _txtBoxCol = new DataGridViewTextBoxColumn
            {
                Name = "colLoginName",
                HeaderText = @"Login Name",
                DataPropertyName = "LOGIN_NAME",
                Width = 180
            };
            dgvMain.Columns.Add(_txtBoxCol);
            dgvMain.Columns["colLoginName"].Visible = true;

            _comboBoxCol = new DataGridViewComboBoxColumn
            {
                Name = "colRole",
                HeaderText = @"Role",
                DataPropertyName = "ROLE_ID",
                DisplayMember = "ROLE_NAME",
                ValueMember = "ROLE_ID",
                Width = 185
            };
            dgvMain.Columns.Add(_comboBoxCol);
            dgvMain.Columns["colRole"].Visible = true;


            dgvMain.AutoGenerateColumns = false;
        }

        private void LoadData()
        {
            _sql = "SELECT LOGIN_NAME FROM APP_USER";
            _mainDt = new DataTable();
            _mainDt = DAL.Seraph.ExecuteQuery(_sql);

            _mainDt.Columns.Add(new DataColumn("SELECT", typeof(bool)));
            _mainDt.AcceptChanges();

            _rolesDt = new DataTable();
            _rolesDt.Columns.Add(new DataColumn("ROLE_ID", typeof(Int32)));
            _rolesDt.Columns.Add(new DataColumn("ROLE_NAME", typeof(String)));

            var row = _rolesDt.NewRow();
            row["ROLE_ID"] = 1;
            row["ROLE_NAME"] = "ADMIN";
            _rolesDt.Rows.Add(row);

            row = _rolesDt.NewRow();
            row["ROLE_ID"] = 2;
            row["ROLE_NAME"] = "ADVANCED USER";
            _rolesDt.Rows.Add(row);

            row = _rolesDt.NewRow();
            row["ROLE_ID"] = 3;
            row["ROLE_NAME"] = "ADVANCED OPERATOR";
            _rolesDt.Rows.Add(row);

            row = _rolesDt.NewRow();
            row["ROLE_ID"] = 4;
            row["ROLE_NAME"] = "OPERATOR";
            _rolesDt.Rows.Add(row);

            row = _rolesDt.NewRow();
            row["ROLE_ID"] = 5;
            row["ROLE_NAME"] = "GUEST";
            _rolesDt.Rows.Add(row);

            _rolesDt.AcceptChanges();

            ((DataGridViewComboBoxColumn)dgvMain.Columns["colRole"]).DataSource = _rolesDt;
        }

        private void PreSelectCheckBoxes()
        {
            for (int i = 0; i < _mainDt.Rows.Count; i++)
            {
                if (i == 2)
                {
                    _mainDt.Rows[i]["SELECT"] = true;
                }
                else if (i == 4)
                {
                    _mainDt.Rows[i]["SELECT"] = true;
                }
            }
        }

        private void PreSelectComBoxValuesForCheckedRows()
        {
            int i = 0;
            foreach (DataGridViewRow row in dgvMain.Rows)
            {
                if (row.Cells["colCheckBoxCol"].Value != DBNull.Value)
                {
                    dgvMain.Rows[i].Cells["colRole"].Value = 2;
                }
                i++;
            }
        }

        private void ShowAll()
        {
            var cm = (CurrencyManager)BindingContext[dgvMain.DataSource];
            foreach (DataGridViewRow row in dgvMain.Rows)
            {
                cm.SuspendBinding();
                row.Visible = true;
            }

            btnDisplayAll.Enabled = false;
            btnDisplaySelected.Enabled = true;
        }

        private void ShowOnlySelected()
        {
            var cm = (CurrencyManager)BindingContext[dgvMain.DataSource];
            foreach (DataGridViewRow row in dgvMain.Rows)
            {
                if (row.Cells["colCheckBoxCol"].Value == DBNull.Value)
                {
                    cm.SuspendBinding();
                    row.Visible = false;
                }
                else
                {
                    row.Visible = true;
                }
            }

            btnDisplayAll.Enabled = true;
            btnDisplaySelected.Enabled = false;
        }

        private int GetSelectedCount()
        {
            int retVal = 0;

            foreach (DataGridViewRow row in dgvMain.Rows)
            {
                if (row.Cells["colCheckBoxCol"].Value != DBNull.Value)
                {
                    if (Convert.ToBoolean(row.Cells["colCheckBoxCol"].Value))
                    {
                        retVal++;
                    }
                }
            }

            return retVal;
        }

        private string GetDetails()
        {
            var sb = new StringBuilder();

            foreach (DataGridViewRow row in dgvMain.Rows)
            {
                if (row.Cells["colCheckBoxCol"].Value != DBNull.Value)
                {
                    if (Convert.ToBoolean(row.Cells["colCheckBoxCol"].Value))
                    {
                        sb.Append("Login : " + row.Cells["colLoginName"].FormattedValue +
                                   " Role ID: " + row.Cells["colRole"].Value +
                                   " Role Name: " + row.Cells["colRole"].FormattedValue + "\n");
                    }
                }
            }

            return sb.ToString();
        }

        private void DataGridComboBox_Load(object sender, EventArgs e)
        {
            PreSelectComBoxValuesForCheckedRows();
            btnDisplayAll.Enabled = false;
        }

        private void BtnDisplayAll_Click(object sender, EventArgs e)
        {
            ShowAll();
        }

        private void BtnDisplaySelected_Click(object sender, EventArgs e)
        {
            ShowOnlySelected();
        }

        private void BtnViewDetail_Click(object sender, EventArgs e)
        {
            var msg = string.Empty;

            var totalSelectedRows = GetSelectedCount();

            if (totalSelectedRows == 0)
            {
                msg = "No row was Selected!";
            }
            else if (totalSelectedRows > 0)
            {
                var details = GetDetails();
                msg = $"Total Selected: {totalSelectedRows}\n{details}";
            }

            MessageBox.Show(msg, @"Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
