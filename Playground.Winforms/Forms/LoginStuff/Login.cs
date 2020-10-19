using System;
using System.Data;
using System.Windows.Forms;
using Playground.Core;
using Playground.Core.AdoNet;
using Playground.Core.Utilities;
using Playground.Winforms.Utilities;

namespace Playground.Winforms.Forms.LoginStuff
{
    public partial class Login : Form
    {
        // ReSharper disable once NotAccessedField.Local
        private DialogResult _iAnswer;
        private bool _isLoginSuccessful;
        private DataTable _loginNameDt;
        private UserInfo _userInfo;

        public Login()
        {
            InitializeComponent();
        }

        public bool ShowForm(Form oParent)
        {
            LoadData();
            _iAnswer = ShowDialog(oParent);

            return _isLoginSuccessful;
        }

        private void LoadData()
        {
            _loginNameDt = DAL.Seraph.ExecuteQuery("SELECT ID, LOGIN_NAME FROM APP_USER ORDER BY ID");

            if (_loginNameDt.Rows.Count > 0)
            {
                cboLoginName.DataSource = _loginNameDt;
                cboLoginName.DisplayMember = "LOGIN_NAME";
                cboLoginName.ValueMember = "ID";
                cboLoginName.SelectedIndex = -1;
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (cboLoginName.SelectedIndex < 0 || string.IsNullOrEmpty(txtPassword.Text))
            {
                Helpers.DisplayInfo(this, "Invalid login name or password. Try again.", "Login");
                return;
            }

            _isLoginSuccessful = AuthenticateUser();
            Close();
        }

        private bool AuthenticateUser()
        {
            bool retVal = false;

            var dt = DAL.Seraph.ExecuteQuery("SELECT * FROM APP_USER");

            if (dt.Rows.Count == 0)
                return false;

            var password = Encryption.Encrypt(txtPassword.Text.Trim());
            var loginName = cboLoginName.Text.Trim();

            foreach (DataRow r in dt.Rows)
            {
                if (r["LOGIN_NAME"].ToString() == loginName)
                {
                    var lastModifiedDate = Convert.ToDateTime(r["DATE_MODIFIED"].ToString());
                    var nextPwdChangeDate = lastModifiedDate.AddDays(30);

                    if (string.CompareOrdinal(password, r["LOGIN_PASSWORD"].ToString().Trim()) == 0)
                    {
                        if (Convert.ToInt32(r["STATUS"].ToString()) != 1)
                        {
                            Helpers.DisplayInfo(this, "Inactive user. Contact Administrator.", "Login");
                            break;
                        }

                        _userInfo = UserInfo.GetUserInfo(loginName, password);

                        if (_userInfo != null)
                        {
                            UserInfo.ActiveUser = _userInfo;
                            retVal = true;
                        }
                        else
                        {
                            retVal = false;
                        }

                        if (DateTime.Today > nextPwdChangeDate)
                        {
                            MessageBox.Show(@"Password expired. You must change password now.", @"Information");
                            var oPasswordChangeForm = new ChangePassword();

                            if (oPasswordChangeForm.ShowForm(this))
                            {
                                retVal = true;
                                break;
                            }

                            _userInfo = UserInfo.GetUserInfo(loginName, password);

                            if (_userInfo != null)
                            {
                                UserInfo.ActiveUser = _userInfo;
                                retVal = true;
                                break;
                            }

                            retVal = false;
                            break;
                        }

                        var diffInDays = (DateTime.Today.Date.Subtract(nextPwdChangeDate.Date)).Days;
                        if (diffInDays < 0)
                        {
                            diffInDays = diffInDays * (-1);
                        }

                        if (diffInDays <= 7)
                        {
                            if (MessageBox.Show(@"Your password will expire in " + diffInDays + @" day(s). Change now?", @"Change Password?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                var oForm = new ChangePassword();

                                if (oForm.ShowForm(this))
                                {
                                    retVal = true;
                                    break;
                                }

                                retVal = true;
                                break;
                            }
                        }
                    }
                }
            }

            return retVal;
        }

        private void Login_Load(object sender, EventArgs e)
        {
            var toolTip = new ToolTip { AutomaticDelay = 1000, AutoPopDelay = 10000, InitialDelay = 500 };

            toolTip.SetToolTip(lblLoginName, "ToolTip Example: This is Login Name label.");
            toolTip.SetToolTip(lblPassword, "ToolTip Example: This is Password label.");
            toolTip.SetToolTip(txtPassword, "ToolTip Example: Enter your password here");
            toolTip.SetToolTip(cboLoginName, "ToolTip Example: Select Login Name from the drop-down");
            toolTip.SetToolTip(btnOK, "ToolTip Example: Click here after you enter your login Name and Password.");
            toolTip.SetToolTip(cboLoginName, "ToolTip Example: Click here if you want to Exit!");
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            _isLoginSuccessful = false;
            Close();
        }
    }
}
