using System;
using System.Windows.Forms;

namespace Playground.Winforms.Forms.Admin
{
    public partial class FreeTextForm : BaseForm
    {
        // ReSharper disable once NotAccessedField.Local
        private DialogResult _iAnswer;
        private string _retVal = string.Empty;

        public FreeTextForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        public string ShowForm(Form oParent, string valueToDisplay)
        {
            if (!string.IsNullOrEmpty(valueToDisplay))
            {
                rTxtBox.Text = valueToDisplay;
            }

            _iAnswer = ShowDialog(oParent);

            if (!string.IsNullOrEmpty(rTxtBox.Text))
            {
                _retVal = rTxtBox.Text.Trim();
            }

            return _retVal;
        }
    }
}
