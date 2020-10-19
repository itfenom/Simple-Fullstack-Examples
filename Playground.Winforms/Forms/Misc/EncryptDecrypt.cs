using System;
using Playground.Core.Utilities;
using Playground.Winforms.Utilities;

namespace Playground.Winforms.Forms.Misc
{
    public partial class EncryptDecrypt : BaseForm
    {
        private string _outputVal = string.Empty;

        public EncryptDecrypt()
        {
            InitializeComponent();
        }

        private void BtnEncrypt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtInputVal.Text))
            {
                Helpers.DisplayInfo(this, "Enter text to Encrypt.", "Encrypt");
                return;
            }

            _outputVal = string.Empty;
            _outputVal = Encryption.Encrypt(txtInputVal.Text.Trim());

            if (!string.IsNullOrEmpty(_outputVal))
            {
                txtOutputVal.Text = _outputVal;
            }
            else
            {
                Helpers.DisplayInfo(this, "The string may be Encrypted already.", "Encrypt");
            }
        }

        private void BtnDecrypt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtInputVal.Text))
            {
                Helpers.DisplayInfo(this, "Enter text to Decrypt.", "Decrypt");
                return;
            }

            _outputVal = string.Empty;
            _outputVal = Encryption.Decrypt(txtInputVal.Text.Trim());

            if (!string.IsNullOrEmpty(_outputVal))
            {
                txtOutputVal.Text = _outputVal;
            }
            else
            {
                Helpers.DisplayInfo(this, "The string may be Decrypted already.", "Decrypt");
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void ClearAll()
        {
            txtInputVal.Clear();
            txtOutputVal.Clear();
            txtInputVal.Focus();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
