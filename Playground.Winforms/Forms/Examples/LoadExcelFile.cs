using System;
using System.Windows.Forms;
using Playground.Winforms.Utilities;
using Excel = Microsoft.Office.Interop.Excel;

namespace Playground.Winforms.Forms.Examples
{
    public partial class LoadExcelFile : Form
    {
        // ReSharper disable once NotAccessedField.Local
        private DialogResult _iAnswer;

        public LoadExcelFile()
        {
            InitializeComponent();
        }

        public void ShowForm(Form oParent)
        {
            _iAnswer = ShowDialog(oParent);

            Close();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"Excel Files|*.xls;*.xlsx;*.xlsm",
                FilterIndex = 1,
                Multiselect = false
            };
            // Set filter options and filter index.


            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = openFileDialog.FileName;
            }
        }

        private void BtnProcess_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPath.Text))
            {
                Helpers.DisplayInfo(this, "File path is required! Try again.");
                return;
            }

            rTxtBox.Clear();
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(txtPath.Text);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            // ReSharper disable once UnusedVariable
            int colCount = xlRange.Columns.Count;
            string msg;

            //Loop starting at 2 to skip the first row in Excel sheet that contains the headers of each column!
            for (int i = 2; i <= rowCount; i++)
            {
                // ReSharper disable once UseIndexedProperty
                Excel.Range range = xlWorksheet.get_Range("A" + i, "E" + i);
                var currRowValues = (Array)range.Cells.Value;
                string[] strArray = ConvertToStringArray(currRowValues);
                msg = string.Format("Row " + (i - 1) + " = First Name: {0}, Last Name: {1}, Display Name: {2}, Email: {3}, Department: {4}", strArray[0], strArray[1], strArray[2], strArray[3], strArray[4]);
                rTxtBox.AppendText(msg + Environment.NewLine);
            }

            //for (int i = 1; i <= _rowCount; i++)
            //{
            //    for (int j = 2; j <= _colCount; j++)
            //    {
            //        MessageBox.Show(xlRange.Cells[i, j].Value2.ToString());
            //    }
            //}

            rTxtBox.AppendText("\n\r\n\r");
            rTxtBox.AppendText("*** Processing Completed ***");
        }

        private string[] ConvertToStringArray(Array values)
        {

            // create a new string array
            string[] theArray = new string[values.Length];

            // loop through the 2-D System.Array and populate the 1-D String Array
            for (int i = 1; i <= values.Length; i++)
            {
                if (values.GetValue(1, i) == null)
                    theArray[i - 1] = "";
                else
                    theArray[i - 1] = values.GetValue(1, i).ToString();
            }

            return theArray;
        }
    }
}
