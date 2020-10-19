using System;
using System.Windows.Forms;

namespace Playground.Winforms.HelperForms
{
    public partial class InputBox : Form
    {
        // ReSharper disable once NotAccessedField.Local
        private DialogResult _iAnswer;
        private string _retVal;

        public InputBox()
        {
            InitializeComponent();
        }

        public string ShowForm(Form oParent, string message, string title)
        {
            lblLabel.Text = message;
            _iAnswer = ShowDialog(oParent);
            Text = title;
            Close();

            return _retVal;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            _retVal = txtValue.Text;
            Close();
        }
    }
}
