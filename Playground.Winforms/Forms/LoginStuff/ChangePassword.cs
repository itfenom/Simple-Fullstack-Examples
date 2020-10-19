using System;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Playground.Core;
using Playground.Core.AdoNet;
using Playground.Core.Utilities;
using Playground.Winforms.Utilities;

namespace Playground.Winforms.Forms.LoginStuff
{
    public partial class ChangePassword : BaseForm
    {
        private bool _isSavedSuccessful;
        private int _salienceVal;
        // ReSharper disable once NotAccessedField.Local
        private DialogResult _iAnswer;
        private string _sql = string.Empty;

        public ChangePassword()
        {
            InitializeComponent();
        }

        public bool ShowForm(Form oParent)
        {
            _iAnswer = ShowDialog(oParent);

            if (_isSavedSuccessful)
            {
                MessageBox.Show(@"Password changed successfully.");
            }

            return _isSavedSuccessful;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateFormContents())
            {
                _sql = string.Empty;
                int salienceInsertVal;

                _sql = "UPDATE APP_USER SET LOGIN_PASSWORD = '" + Encryption.Encrypt(txtConfirmPassword.Text.Trim())
                     + "', DATE_MODIFIED = SYSDATE WHERE ID = " + UserInfo.ActiveUser.UserId;
                DAL.Seraph.ExecuteNonQuery(_sql);

                _sql = string.Empty;

                if (_salienceVal <= 3)
                {
                    salienceInsertVal = _salienceVal;
                    _sql = "INSERT INTO APP_USER_PASSWORD_TRACKING (USER_ID, PASSWORD, SALIENCE) "
                         + "VALUES(" + UserInfo.ActiveUser.UserId + ", '" + Encryption.Encrypt(txtConfirmPassword.Text.Trim()) + "', "
                         + salienceInsertVal + ")";
                }
                else if (_salienceVal > 3)
                {
                    // ReSharper disable once RedundantAssignment
                    salienceInsertVal = _salienceVal - 3;
                    _sql = "UPDATE APP_USER_PASSWORD_TRACKING SET PASSWORD = '" + Encryption.Encrypt(txtConfirmPassword.Text.Trim()) + "' "
                         + " WHERE USER_ID = " + UserInfo.ActiveUser.UserId + " AND SALIENCE = " + _salienceVal;
                }

                DAL.Seraph.ExecuteNonQuery(_sql);
                _isSavedSuccessful = true;
                Close();
            }
        }

        private bool ValidateFormContents()
        {
            var retVal = false;
            var oErr = new ArrayList();

            if (string.Compare(Encryption.Decrypt(UserInfo.ActiveUser.Password), txtCurrentPassword.Text.Trim(), StringComparison.OrdinalIgnoreCase) != 0)
            {
                oErr.Add("Old Password: Incorrect old password entered.");
            }

            if (string.CompareOrdinal(txtNewPassword.Text.Trim(), txtConfirmPassword.Text.Trim()) != 0)
            {
                oErr.Add("New / Verify Password: Password was not type correctly.");
            }

            if (txtNewPassword.Text == "" || txtNewPassword.Text.Length < 0 || txtConfirmPassword.Text == "" || txtConfirmPassword.Text.Length < 0)
            {
                oErr.Add("New / Verify Password: Password cannot be blank.");
            }

            if (string.CompareOrdinal(txtCurrentPassword.Text.Trim(), txtConfirmPassword.Text.Trim()) == 0)
            {
                oErr.Add("New /Old Password: New Password cannot be same as your current password.");
            }

            if (string.CompareOrdinal(txtNewPassword.Text.Trim(), txtConfirmPassword.Text.Trim()) == 0)
            {
                if (ValidatePassword(txtConfirmPassword.Text.Trim()))
                {
                    _sql = string.Empty;
                    _sql = "SELECT * FROM APP_USER_PASSWORD_TRACKING WHERE USER_ID = " + UserInfo.ActiveUser.UserId;
                    var dt = DAL.Seraph.ExecuteQuery(_sql);

                    if (dt.Rows.Count == 0)
                    {
                        _salienceVal = 1;
                    }
                    else if (dt.Rows.Count > 0)
                    {
                        _salienceVal = Convert.ToInt32(dt.Compute("MAX(SALIENCE)", "")) + 1;
                    }

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow r in dt.Rows)
                        {
                            if (r["PASSWORD"].ToString() == Encryption.Encrypt(txtConfirmPassword.Text.Trim()))
                            {
                                Helpers.DisplayInfo(this, "Change Password", "A new password cannot be one of your old passwords");
                                txtNewPassword.Clear();
                                txtConfirmPassword.Clear();
                                txtNewPassword.Focus();
                                retVal = false;
                                break;
                            }

                            retVal = true;
                        }
                    }
                }
                else
                {
                    return false;
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

        public bool ValidatePassword(string inputVal)
        {
            var retVal = false;
            var oErr = new ArrayList();
            Regex upperCase = new Regex("[A-Z]");
            Regex lowerCase = new Regex("[a-z]");
            Regex numeric = new Regex("[0-9]");

            if (upperCase.Matches(inputVal).Count <= 0)
            {
                oErr.Add("Password must contain at least one upper case character.");
            }

            if (lowerCase.Matches(inputVal).Count <= 0)
            {
                oErr.Add("Password must contain at least one lower case character.");
            }

            if (numeric.Matches(inputVal).Count <= 0)
            {
                oErr.Add("Password must contain at least one numeric character.");
            }

            if (HasSpecialCharacters(inputVal))
            {
                oErr.Add("Password must not contain any special character.");
            }

            if (inputVal.Length > 7 || inputVal.Length < 7)
            {
                oErr.Add("Password must be 7 characters long.");
            }

            if (oErr.Count > 0)
            {
                Helpers.DisplayMultipleErrors(null, oErr);
            }
            else
            {
                retVal = true;
            }

            return retVal;
        }

        private bool HasSpecialCharacters(string inputVal)
        {
            bool retVal = false;
            string[] specialChars = {"`", "`", "!", "@", "#", "$", "%", "^", "&", "*", "(",
                                                  ")", "-", "_", "+", "=", "|", "\"", "[", "]", "{", "}",
                                                  ";", ":", "'", "?", "/", "<", ",", ">"};
            for (int i = 0; i < specialChars.Length - 1; i++)
            {
                if (inputVal.Contains(specialChars[i]))
                {
                    retVal = true;
                    break;
                }
            }
            return retVal;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
