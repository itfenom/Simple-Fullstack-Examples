using System.Data;
using System.Windows.Forms;

namespace Playground.Winforms.Forms.Examples
{
    public partial class ListViewSearchDialog : Form
    {
        // ReSharper disable once NotAccessedField.Local
        private DialogResult _iAnswer;
        private string _retVal = string.Empty;
        private DataTable _dt = new DataTable();

        public ListViewSearchDialog()
        {
            InitializeComponent();
        }

        public string ShowForm(Form oParent, DataTable dt)
        {
            _dt = dt;
            dgvUsers.DataSource = _dt;
            _iAnswer = ShowDialog(oParent);
            return _retVal;
        }

        private void dgvUsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                _retVal = $"ID: {dgvUsers.Rows[e.RowIndex].Cells["ID"].Value}" +
                          $"\nLogin Name: {dgvUsers.Rows[e.RowIndex].Cells["LOGIN_NAME"].Value}" +
                          $"\nEMAIL: {dgvUsers.Rows[e.RowIndex].Cells["EMAIL"].Value}" +
                          $"\nDate Modified: {dgvUsers.Rows[e.RowIndex].Cells["DATE_MODIFIED"].Value}";

                if (!string.IsNullOrEmpty(_retVal))
                {
                    Close();
                }
            }
        }
    }
}
