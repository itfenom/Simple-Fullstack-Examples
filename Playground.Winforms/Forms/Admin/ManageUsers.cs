using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Playground.Core.AdoNet;
using Playground.Core.Utilities;
using Playground.Winforms.Forms.LoginStuff;
using Playground.Winforms.Utilities;

namespace Playground.Winforms.Forms.Admin
{
    public partial class ManageUsers : BaseForm
    {
        // ReSharper disable once NotAccessedField.Local
        private DialogResult _iAnswer;
        private string _sql = string.Empty;
        private readonly BindingSource _bsUsers = new BindingSource();
        private DataGridViewTextBoxColumn _txtBoxColumn;
        private DataGridViewComboBoxColumn _comboBoxColumn;
        private DataTable _userDt;
        private DataTable _yesNoDt;
        private DataTable _rolesDt;

        public ManageUsers()
        {
            InitializeComponent();
        }

        public void ShowForm(Form oParent)
        {
            CreateYesNoDataTable();
            LoadUserRolesTable();
            SetupGridViewColumns();
            LoadUserDataTable();
            LoadData();
            _iAnswer = ShowDialog(oParent);
        }

        private void CreateYesNoDataTable()
        {
            _yesNoDt = new DataTable();
            _yesNoDt.Columns.Add("STATUS", typeof(decimal));
            _yesNoDt.Columns.Add("STRING_VALUE", typeof(string));

            var dataRow = _yesNoDt.NewRow();
            dataRow["STATUS"] = 0;
            dataRow["STRING_VALUE"] = "No";
            _yesNoDt.Rows.Add(dataRow);

            dataRow = _yesNoDt.NewRow();
            dataRow["STATUS"] = 1;
            dataRow["STRING_VALUE"] = "Yes";
            _yesNoDt.Rows.Add(dataRow);

            _yesNoDt.AcceptChanges();
        }

        private void LoadUserRolesTable()
        {
            _sql = string.Empty;
            _sql = "SELECT * FROM APP_USER_ROLE";
            _rolesDt = DAL.Seraph.ExecuteQuery(_sql);
        }

        private void LoadUserDataTable()
        {
            _sql = string.Empty;
            _sql = "SELECT * FROM APP_USER";
            _userDt = DAL.Seraph.ExecuteQuery(_sql);
        }

        private void LoadData()
        {
            _userDt.Columns["ID"].AllowDBNull = true;
            _userDt.Columns["LOGIN_NAME"].AllowDBNull = true;
            _userDt.Columns["LOGIN_PASSWORD"].AllowDBNull = true;
            _userDt.Columns["DISPLAY_NAME"].AllowDBNull = true;
            _userDt.Columns["EMAIL"].AllowDBNull = true;
            _userDt.Columns["STATUS"].AllowDBNull = true;
            _userDt.Columns["ROLE_ID"].AllowDBNull = true;
            _userDt.Columns["DATE_CREATED"].AllowDBNull = true;
            _userDt.Columns["DATE_MODIFIED"].AllowDBNull = true;

            _bsUsers.DataSource = _userDt;
            dgvUsers.DataSource = _bsUsers;
            bnUsers.BindingSource = _bsUsers;
        }

        private void SetupGridViewColumns()
        {
            _txtBoxColumn = new DataGridViewTextBoxColumn();
            _txtBoxColumn.Name = "colID";
            _txtBoxColumn.DataPropertyName = "ID";
            _txtBoxColumn.HeaderText = @"ID";
            _txtBoxColumn.ReadOnly = true;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            _txtBoxColumn.Width = 5;
            dgvUsers.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumn();
            _txtBoxColumn.Name = "coLoginName";
            _txtBoxColumn.DataPropertyName = "LOGIN_NAME";
            _txtBoxColumn.HeaderText = @"Login Name";
            _txtBoxColumn.ReadOnly = false;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            _txtBoxColumn.Width = 30;
            dgvUsers.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumn();
            _txtBoxColumn.Name = "colLoginPassword";
            _txtBoxColumn.DataPropertyName = "LOGIN_PASSWORD";
            _txtBoxColumn.HeaderText = @"Password";
            _txtBoxColumn.ReadOnly = false;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            _txtBoxColumn.Width = 30;
            dgvUsers.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumn();
            _txtBoxColumn.Name = "colDisplayName";
            _txtBoxColumn.DataPropertyName = "DISPLAY_NAME";
            _txtBoxColumn.HeaderText = @"Display Name";
            _txtBoxColumn.ReadOnly = false;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            _txtBoxColumn.Width = 30;
            dgvUsers.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumn();
            _txtBoxColumn.Name = "colEmail";
            _txtBoxColumn.DataPropertyName = "EMAIL";
            _txtBoxColumn.HeaderText = @"Email";
            _txtBoxColumn.ReadOnly = false;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            _txtBoxColumn.Width = 30;
            dgvUsers.Columns.Add(_txtBoxColumn);

            _comboBoxColumn = new DataGridViewComboBoxColumn();
            _comboBoxColumn.Name = "colStatus";
            _comboBoxColumn.HeaderText = @"Active?";
            _comboBoxColumn.DataPropertyName = "STATUS";
            _comboBoxColumn.DisplayMember = "STRING_VALUE";
            _comboBoxColumn.ValueMember = "STATUS";
            _comboBoxColumn.DataSource = _yesNoDt;
            _comboBoxColumn.Width = 30;
            dgvUsers.Columns.Add(_comboBoxColumn);

            _comboBoxColumn = new DataGridViewComboBoxColumn();
            _comboBoxColumn.HeaderText = @"Role";
            _comboBoxColumn.DataPropertyName = "ROLE_ID";
            _comboBoxColumn.DisplayMember = "ROLE_NAME";
            _comboBoxColumn.ValueMember = "ROLE_ID";
            _comboBoxColumn.DataSource = _rolesDt;
            _comboBoxColumn.Width = 30;
            dgvUsers.Columns.Add(_comboBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumn();
            _txtBoxColumn.Name = "colDateCreated";
            _txtBoxColumn.DataPropertyName = "DATE_CREATED";
            _txtBoxColumn.HeaderText = @"Date Created";
            _txtBoxColumn.ReadOnly = true;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            _txtBoxColumn.Width = 30;
            dgvUsers.Columns.Add(_txtBoxColumn);

            _txtBoxColumn = new DataGridViewTextBoxColumn();
            _txtBoxColumn.Name = "colDateModified";
            _txtBoxColumn.DataPropertyName = "DATE_MODIFIED";
            _txtBoxColumn.HeaderText = @"Date Modified";
            _txtBoxColumn.ReadOnly = true;
            _txtBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            _txtBoxColumn.Width = 30;
            dgvUsers.Columns.Add(_txtBoxColumn);

            dgvUsers.AutoGenerateColumns = false;
        }

        private bool HasChanges()
        {
            dgvUsers.EndEdit();
            _bsUsers.EndEdit();

            var dtChanges = _userDt.GetChanges();

            if (dtChanges != null && dtChanges.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool Save()
        {
            bool retVal = false;
            string oErrors = string.Empty;

            dgvUsers.EndEdit();
            _bsUsers.EndEdit();

            if (!HasChanges())
                return false;

            var dt = _userDt.GetChanges();

            if (!ValidateData(dt)) return false;

            // ReSharper disable once PossibleNullReferenceException
            foreach (DataRow row in dt.Rows)
            {
                if (row.RowState == DataRowState.Added)
                {
                    if (_userDt.Select("LOGIN_NAME ='" + row["LOGIN_NAME"].ToString().ToUpper() + "'").Length > 1)
                    {
                        oErrors = $"Login Name '{row["LOGIN_NAME"].ToString().ToUpper()}' already exist!";
                    }

                    if (!string.IsNullOrEmpty(oErrors))
                    {
                        break;
                    }

                    var outPut = Core.Repositories.ManageUsersRepository.InsertNewUser
                    (
                        row["LOGIN_NAME"].ToString(),
                        row["LOGIN_PASSWORD"].ToString(),
                        row["DISPLAY_NAME"].ToString(),
                        row["EMAIL"].ToString(),
                        Convert.ToInt32(row["ROLE_ID"].ToString())
                    );

                    if (outPut.Any())
                    {
                        int id = Convert.ToInt32(outPut["P_ID_OUT"]);
                        DataRow[] r = _userDt.Select("LOGIN_NAME = '" + row["LOGIN_NAME"] + "'");

                        r[0]["ID"] = id;
                        _userDt.AcceptChanges();
                    }
                }
                else if (row.RowState == DataRowState.Modified)
                {
                    if (_userDt.Select("LOGIN_NAME ='" + row["LOGIN_NAME"].ToString().ToUpper() + "'").Length > 1)
                    {
                        oErrors = $"Login Name '{row["LOGIN_NAME"].ToString().ToUpper()}' already exist!";
                    }

                    if (!string.IsNullOrEmpty(oErrors))
                    {
                        break;
                    }

                    var password = Encryption.Decrypt(row["LOGIN_PASSWORD"].ToString());

                    if (string.IsNullOrEmpty(password))
                    {
                        //Password is in original form, so encrypt it!
                        password = Encryption.Encrypt(row["LOGIN_PASSWORD"].ToString());
                    }
                    else
                    {
                        password = row["LOGIN_PASSWORD"].ToString();
                    }
                    _sql = string.Empty;
                    _sql = "UPDATE APP_USER SET LOGIN_NAME = '" + row["LOGIN_NAME"] + "', "
                           + "LOGIN_PASSWORD = '" + password + "', "
                           + "DISPLAY_NAME = '" + row["DISPLAY_NAME"] + "', "
                           + "EMAIL = '" + row["EMAIL"] + "', "
                           + "STATUS = " + Convert.ToInt32(row["STATUS"]) + ", "
                           + "ROLE_ID = " + Convert.ToInt32(row["ROLE_ID"]) + ", "
                           + "DATE_MODIFIED = SYSDATE WHERE ID = " + Convert.ToInt32(row["ID"]);
                    DAL.Seraph.ExecuteNonQuery(_sql);
                }
                else if (row.RowState == DataRowState.Deleted)
                {
                    _sql = string.Empty;
                    _sql = "UPDATE APP_USER SET STATUS = 0 WHERE ID = " + Convert.ToInt32(row["ID", DataRowVersion.Original]);
                    DAL.Seraph.ExecuteNonQuery(_sql);
                }
            }

            if (!string.IsNullOrEmpty(oErrors))
            {
                Helpers.DisplayInfo(this, oErrors, "Save Account");
            }
            else
            {
                _userDt.AcceptChanges();
                LoadUserDataTable();
                LoadData();
                retVal = true;
            }

            return retVal;
        }

        private bool ValidateData(DataTable dt)
        {
            bool retVal = false;
            var oErr = new ArrayList();
            var oForm = new ChangePassword();

            foreach (DataRow row in dt.Rows)
            {
                if (row.RowState != DataRowState.Deleted)
                {
                    if (string.IsNullOrEmpty(row["LOGIN_NAME"].ToString()))
                    {
                        oErr.Add("Login Name is required!");
                    }

                    var password = row["LOGIN_PASSWORD"].ToString();

                    if (string.IsNullOrEmpty(password))
                    {
                        oErr.Add("Password is required!");
                    }
                    else
                    {
                        var decryptedPassword = Encryption.Decrypt(row["LOGIN_PASSWORD"].ToString());

                        if (string.IsNullOrEmpty(decryptedPassword))
                        {
                            if (oForm.ValidatePassword(password) == false)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (oForm.ValidatePassword(decryptedPassword) == false)
                            {
                                return false;
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(row["DISPLAY_NAME"].ToString()))
                    {
                        oErr.Add("Display Name is required!");
                    }

                    if (string.IsNullOrEmpty(row["EMAIL"].ToString()))
                    {
                        oErr.Add("Email is required!");
                    }


                    if (oErr.Count > 0)
                    {
                        break;
                    }
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

        private void btnAddNewUser_Click(object sender, EventArgs e)
        {
            var newUserRow = _userDt.NewRow();
            newUserRow["ID"] = 0;
            newUserRow["LOGIN_NAME"] = string.Empty;
            newUserRow["LOGIN_PASSWORD"] = string.Empty;
            newUserRow["DISPLAY_NAME"] = string.Empty;
            newUserRow["EMAIL"] = string.Empty;
            newUserRow["STATUS"] = 1;
            newUserRow["ROLE_ID"] = 4;
            newUserRow["DATE_CREATED"] = DateTime.Today.Date.ToShortDateString();
            newUserRow["DATE_MODIFIED"] = DateTime.Today.Date.ToShortDateString();

            _userDt.Rows.Add(newUserRow);
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (dgvUsers.Rows.Count > 0)
            {
                if (dgvUsers.SelectedRows.Count == 0)
                {
                    Helpers.DisplayInfo(this, "Please select a row to be deleted!", "Manage Users");
                    return;
                }

                if (dgvUsers.SelectedRows.Count == 1)
                {
                    if (dgvUsers.CurrentRow != null && dgvUsers.CurrentRow.DataBoundItem is DataRowView dataRowView)
                    {
                        dataRowView.Row.Delete();
                    }
                }
                else if (dgvUsers.SelectedRows.Count > 1)
                {
                    Helpers.DisplayInfo(this, "Only one row can be deleted at a time!", "Manage Users");
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void dgvUsers_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void ManageUsers_FormClosing(object sender, FormClosingEventArgs e)
        {
            dgvUsers.EndEdit();
            _bsUsers.EndEdit();

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

        private void dgvUsers_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                // ReSharper disable once PossibleNullReferenceException
                var password = dgvUsers.CurrentRow.Cells["colLoginPassword"].Value.ToString();

                if (!string.IsNullOrEmpty(password))
                {
                    var oForm = new ChangePassword();
                    if (oForm.ValidatePassword(password) == false)
                    {
                        dgvUsers.CurrentCell = dgvUsers.CurrentRow.Cells["colLoginPassword"];
                    }
                }
            }
        }
    }
}
