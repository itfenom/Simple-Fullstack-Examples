using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Playground.Core.AdoNet;
using Playground.Winforms.Utilities;

namespace Playground.Winforms.Forms.Examples
{
    public partial class OracleDb : BaseForm
    {
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once NotAccessedField.Local
        private DialogResult _iAnwer;

        public OracleDb()
        {
            InitializeComponent();
        }

        public void ShowForm(Form oParent)
        {
            _iAnwer = ShowDialog(Parent);
        }

        private void LoadData(string sql, string schemaName)
        {
            dgvData.DataSource = null;
            lblMessage.Text = string.Empty;

            var database = DAL.DalList[schemaName];

            var dt = database.ExecuteQuery(sql);

            if (dt.Rows.Count > 0)
            {
                dgvData.DataSource = dt;
                lblMessage.Text = @"Showing APP_USER table data from '" + schemaName + @"' Schema.";
            }
            else
            {
                lblMessage.Text = @"Unable to load data from '" + schemaName + @"' Schema.";
            }

        }

        private void LoadDataFromSeraphSchemaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dgvData.DataSource = null;
            lblMessage.Text = string.Empty;
            LoadData("SELECT DISTINCT A.DISPLAY_NAME AS NAME, A.EMAIL, B.ROLE_NAME FROM APP_USER A, APP_USER_ROLE B", "Seraph");
        }

        private void LoadDataFromHRSchemaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dgvData.DataSource = null;
            lblMessage.Text = string.Empty;
            //MessageBox.Show(@"This code has been commented out due to unavailability of HR Schema!", @"Load data from HR", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadData(@"SELECT A.FIRST_NAME, A.LAST_NAME, B.DEPARTMENT_NAME 
                        FROM EMPLOYEES A 
                        INNER JOIN DEPARTMENTS B
                            ON A.DEPARTMENT_ID = B.DEPARTMENT_ID
                        WHERE ROWNUM <= 15", "HR");
        }

        private void RetrieveAssociativeArraysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dgvData.DataSource = null;
            lblMessage.Text = string.Empty;

            var data = DAL.Seraph.RetrieveAppUsersAsAssociativeArrays();

            if (data.Rows.Count > 0)
            {
                dgvData.DataSource = data;
                lblMessage.Text = @"Showing Associative Arrays data retrieved from Seraph Schema.";
            }
            else
            {
                lblMessage.Text = @"Unable to load Associative Arrays data returned by Oracle Procedure.";
            }
        }

        private void PassAssociativeArraysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dgvData.DataSource = null;
            lblMessage.Text = string.Empty;

            var namesList = new List<string>();
            var numberList = new List<int>();

            namesList.Add("Jim");
            namesList.Add("Marry");
            namesList.Add("John");
            namesList.Add("Mike");

            numberList.Add(35);
            numberList.Add(85);
            numberList.Add(95);

            var outputMsg = DAL.Seraph.PassAssociativeArraysToOracle(namesList, numberList);
            Helpers.DisplayInfo(this, outputMsg, "Pass collection to Oracle Proc");
        }

        private void OracleTimestampToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sql = "SELECT CURRENT_TIMESTAMP AS CURRENT_ORACLE_TIME_STAMP, "
                   + "TO_TIMESTAMP('Sep-10-2002 14:10:10.423000', 'Mon-DD-YYYY HH24:MI:SS:FF') AS CUSTOM_FORMATTED_TIME_STAMP FROM DUAL";
            var dt = DAL.Seraph.ExecuteQuery(sql);
            string dateFormat = "MMM-dd-yyyy hh:mm:ss:ffffff tt";

            var timeStampMsg = $"Oracle Time Stamp; {dt.Rows[0]["CURRENT_ORACLE_TIME_STAMP"]}" +
                               $"\nCustom Formatted Time Stamp: {Convert.ToDateTime(dt.Rows[0]["CUSTOM_FORMATTED_TIME_STAMP"]).ToString(dateFormat)}";

            if (!string.IsNullOrEmpty(timeStampMsg))
            {
                Helpers.DisplayInfo(this, timeStampMsg, "Oracle Timestamp");
            }
        }

        private void DownloadUploadBlobsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new DownloadUploadBlob();
            form.ShowForm(this);
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
