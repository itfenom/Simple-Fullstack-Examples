using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Playground.Core.AdoNet;
using Playground.Winforms.Utilities;

// ReSharper disable PossibleNullReferenceException

namespace Playground.Winforms.Forms.Examples
{
    public partial class DataGridToExcel : Form
    {
        // ReSharper disable once NotAccessedField.Local
        private DialogResult _iAnswer;
        private DataTable _appUsersDt;
        private string _sql = string.Empty;
        private CheckBox _headerCheckBox;
        private BindingSource _bs;

        public DataGridToExcel()
        {
            InitializeComponent();
        }

        public void ShowForm(Form oParent)
        {
            LoadData();
            _iAnswer = ShowDialog(oParent);

            Close();
        }

        private void LoadData()
        {
            _sql = "SELECT * FROM APP_USER ORDER BY ID";
            _appUsersDt = new DataTable();
            _appUsersDt = DAL.Seraph.ExecuteQuery(_sql);

            if (_appUsersDt.Rows.Count > 0)
            {
                var dataCol = new DataColumn
                {
                    ColumnName = "SELECT",
                    Caption = "",
                    DataType = typeof(bool)
                };
                _appUsersDt.Columns.Add(dataCol);

                foreach (DataRow row in _appUsersDt.Rows)
                {
                    row["SELECT"] = false;
                }

                _appUsersDt.AcceptChanges();

                _bs = new BindingSource {DataSource = _appUsersDt};
                dgvAppUsers.DataSource = _bs;

                dgvAppUsers.Columns["LOGIN_PASSWORD"].Visible = false;
                dgvAppUsers.Columns["DISPLAY_NAME"].Visible = false;
                dgvAppUsers.Columns["ROLE_ID"].Visible = false;
                dgvAppUsers.Columns["DATE_MODIFIED"].Visible = false;
                dgvAppUsers.Columns["STATUS"].Visible = false;

                dgvAppUsers.Columns["SELECT"].DisplayIndex = 0;
                dgvAppUsers.Columns["ID"].DisplayIndex = 1;
                dgvAppUsers.Columns["LOGIN_NAME"].DisplayIndex = 2;
                dgvAppUsers.Columns["EMAIL"].DisplayIndex = 3;
                dgvAppUsers.Columns["DATE_CREATED"].DisplayIndex = 4;

                //set some properties of the grid.
                dgvAppUsers.AllowUserToAddRows = false;
                dgvAppUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvAppUsers.RowsDefaultCellStyle.BackColor = Color.LightGray;
                dgvAppUsers.AlternatingRowsDefaultCellStyle.BackColor = Color.DarkGray;
                dgvAppUsers.RowHeadersVisible = false;

                //Add checkBox header
                CreatCheckBoxInGridViewHeader();
            }
        }

        private void CreatCheckBoxInGridViewHeader()
        {
            _headerCheckBox = new CheckBox();
            var rect = dgvAppUsers.GetCellDisplayRectangle(0, -1, true);
            rect.X = rect.Location.X + 71; //Move the checkBox position [left/Right]
            rect.Y = rect.Location.Y + 5;  //Move the checkBox position [up/down]
            rect.Width = rect.Size.Width;
            rect.Height = rect.Size.Height;

            _headerCheckBox.Name = "headerCheckBox";
            _headerCheckBox.Size = new Size(15, 15);
            _headerCheckBox.Location = rect.Location;
            _headerCheckBox.Padding = new Padding(0);
            _headerCheckBox.Margin = new Padding(0);
            _headerCheckBox.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
            _headerCheckBox.CheckedChanged += HeaderCheckBox_CheckedChanged;
            dgvAppUsers.Controls.Add(_headerCheckBox);
        }

        private void HeaderCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (dgvAppUsers.Rows.Count > 0)
            {
                var headerBox = ((CheckBox)dgvAppUsers.Controls.Find("headerCheckBox", true)[0]);
                for (int i = 0; i < dgvAppUsers.Rows.Count; i++)
                {
                    dgvAppUsers.Rows[i].Cells["SELECT"].Value = headerBox.Checked;
                    //string x = dataGridView1.Rows[i].Cells["COUNTRYNAME"].Value.ToString();
                    //if(x.StartsWith("P"))
                    //{
                    //    dataGridView1.Rows[i].Cells["SELECT"].Value = headerBox.Checked;
                    //}
                }
                _bs.ResetBindings(false);
            }
        }

        private void BtnExportToExcel_Click(object sender, EventArgs e)
        {
            if (dgvAppUsers.Rows.Count <= 0) return;

            if (MessageBox.Show(@"Are you sure, you want to export to Excel?", @"Confirm Export", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            try
            {
                // creating Excel Application
                Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();

                // creating new WorkBook within Excel application
                Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);

                // creating new Excel sheet in workbook
                Microsoft.Office.Interop.Excel._Worksheet worksheet;

                // see the excel sheet behind the program
                app.Visible = true;

                if (!Directory.Exists(@"C:\Temp"))
                {
                    Directory.CreateDirectory(@"C:\Temp");
                }

                var fileName = @"C:\Temp\ExportedFromGridView_" + DateTime.Now.ToString("MM_dd_yy_HH_mm") + ".xls";

                // get the reference of first sheet. By default its name is Sheet1.
                // store its reference to worksheet
                // ReSharper disable once RedundantAssignment
                worksheet = workbook.Sheets["Sheet1"];
                worksheet = workbook.ActiveSheet;

                // changing the name of active sheet
                worksheet.Name = "Exported from DataGridView";

                // storing header part in Excel
                for (int i = 1; i < dgvAppUsers.Columns.Count + 1; i++)
                {
                    worksheet.Cells[1, i] = dgvAppUsers.Columns[i - 1].HeaderText;
                }

                // storing Each row and column value to excel sheet [DataGridView1.Rows.Count - 1] minus 1 if you have extra row at the bottom of the grid!
                for (int i = 0; i < dgvAppUsers.Rows.Count; i++)
                {
                    for (int j = 0; j < dgvAppUsers.Columns.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1] = dgvAppUsers.Rows[i].Cells[j].Value.ToString();
                    }
                }

                // save the application
                workbook.SaveAs(fileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                          Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive,
                                          Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                // Exit from the application
                app.Quit();
                Helpers.DisplayInfo(this, "Data exported successfully to excel file: " + fileName);
            }
            catch (Exception ex)
            {
                Helpers.DisplayException(this, ex);
            }
        }

        private void BtnDisplayChecked_Click(object sender, EventArgs e)
        {
            if (dgvAppUsers.Rows.Count > 0)
            {
                var sb = new StringBuilder();
                // ReSharper disable once UnusedVariable
                CheckBox headerBox = ((CheckBox)dgvAppUsers.Controls.Find("headerCheckBox", true)[0]);
                for (int i = 0; i < dgvAppUsers.Rows.Count; i++)
                {
                    if ((bool)dgvAppUsers.Rows[i].Cells["SELECT"].Value)
                    {
                        sb.Append("ID: " + dgvAppUsers.Rows[i].Cells["ID"].Value + "\n");
                        sb.Append("LOGIN_NAME: " + dgvAppUsers.Rows[i].Cells["LOGIN_NAME"].Value + "\n");
                        sb.Append("EMAIL: " + dgvAppUsers.Rows[i].Cells["EMAIL"].Value + "\n");
                        sb.Append("DATE_CREATED: " + Convert.ToDateTime(dgvAppUsers.Rows[i].Cells["DATE_CREATED"].Value.ToString()).ToShortDateString() + "\n\n");
                    }
                }

                if (sb.Length > 0)
                {
                    Helpers.DisplayInfo(this, sb.ToString(), "Display Checked Rows");
                }
                else
                {
                    Helpers.DisplayInfo(this, "No rows selected!", "Display Checked Rows");
                }
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
