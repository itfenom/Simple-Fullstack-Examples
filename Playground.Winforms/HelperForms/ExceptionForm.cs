using System;

// ReSharper disable once IdentifierTypo
namespace Playground.Winforms.HelperForms
{
    public partial class ExceptionForm : BaseForm
    {
        private Exception _exception;
        public Exception Exception
        {
            get => _exception;
            set
            {
                _exception = value;
                if (_exception != null)
                {
                    TypeTextBox.Text = _exception.GetType().ToString();
                    MessageTextBox.Text = _exception.Message;
                    StackTraceTextBox.Text = _exception.StackTrace;
                    ShowInnerExceptionButton.Visible = _exception.InnerException != null;
                }
                else
                {
                    TypeTextBox.Text = string.Empty;
                    MessageTextBox.Text = string.Empty;
                    StackTraceTextBox.Text = string.Empty;
                    ShowInnerExceptionButton.Visible = false;
                }
            }
        }

        public ExceptionForm()
        {
            InitializeComponent();
        }

        private void ShowInnerExceptionButton_Click(object sender, EventArgs e)
        {
            var innerExceptionForm = new ExceptionForm { Exception = Exception.InnerException };
            innerExceptionForm.Show();
        }
    }
}
