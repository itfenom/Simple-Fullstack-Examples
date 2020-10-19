using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Playground.Core.AdoNet;
using Playground.Winforms.Utilities;

namespace Playground.Winforms.Forms.Examples
{
    public partial class DownloadUploadBlob : Form
    {
        // ReSharper disable once NotAccessedField.Local
        private DialogResult _iAnswer;
        private string _sql = string.Empty;

        public DownloadUploadBlob()
        {
            InitializeComponent();
        }

        public void ShowForm(Form oParent)
        {
            _iAnswer = ShowDialog(oParent);
            Dispose();
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"All Files Files|*.*", FilterIndex = 1, Multiselect = false
            };
            // Set filter options and filter index.


            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = openFileDialog.FileName;
            }
        }

        private void BtnDownloadBlob_Click(object sender, EventArgs e)
        {
            if (numUpDownNodeID.Value <= 0)
            {
                Helpers.DisplayInfo(this, "Enter node ID to download Blob from Oracle DB!", "Downloading Blob");
                return;
            }

            int nodeId = Convert.ToInt32(numUpDownNodeID.Value);

            if (IsFileExist(nodeId))
            {
                DownloadBlob(nodeId);
            }
            else
            {
                Helpers.DisplayInfo(this, "No file found for the Node ID = " + nodeId, "Downloading BLob");
            }
        }

        private void DownloadBlob(int id)
        {
            _sql = string.Empty;
            //OracleBlob _oBlob = null;

            _sql = @"SELECT BLOB_DATA FROM BLOB_TBL WHERE ID = " + id;

            var byteArray = (byte[])DAL.Seraph.ExecuteScalar(_sql);

            if (byteArray.Length > 0)
            {
                var assemblyPath = Assembly.GetEntryAssembly()?.Location;
                var currentDirPath = Path.GetDirectoryName(assemblyPath);
                var fileName = currentDirPath + @"\Blob_Download_" + DateTime.Now.ToString("MM_dd_yy HHmmss") + id + ".txt";

                using (var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                }

                Helpers.DisplayInfo(this, "File download successfully at the following location\n\n" + fileName, "Download Blob");
                txtFilePath.Text = string.Empty;
                numUpDownNodeID.Value = 0;
            }

            //_oBlob = tqtBaw.GetBlob(_sql);

            //if (_oBlob != null)
            //{
            //    string _assemblyPath = Assembly.GetEntryAssembly().Location;
            //    string _currentDirPath = Path.GetDirectoryName(_assemblyPath);
            //    string _fileName = _currentDirPath + @"\Blob_Download_" + DateTime.Now.ToString("MM_dd_yy HHmmss") + nodeID + ".txt";

            //    using (FileStream outPutFile = File.Create(_fileName))
            //    {
            //        byte[] buffer = new byte[1024];
            //        int read;

            //        while ((read = _oBlob.Read(buffer, 0, buffer.Length)) > 0)
            //        {
            //            outPutFile.Write(buffer, 0, read);
            //        }

            //        HelperTools.DisplayInfo(this, "File download successfully at the following location\n\n" + _fileName, "Download Blob");
            //        this.txtFilePath.Text = string.Empty;
            //        numUpDownNodeID.Value = 0;
            //    }
            //}
        }

        private bool IsFileExist(int id)
        {
            bool retVal = false;
            _sql = string.Empty;

            _sql = "SELECT COUNT(*) FROM BLOB_TBL WHERE ID IS NOT NULL AND ID = " + id;
            var obj = DAL.Seraph.ExecuteScalar(_sql);

            if (obj != null)
            {
                if (Convert.ToInt32(obj) == 1)
                {
                    retVal = true;
                }
            }

            return retVal;
        }

        private void BtnUploadBlob_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilePath.Text))
            {
                Helpers.DisplayInfo(this, "Browse to a file that needs to be uploaded to Oracle DB!", "Uploading Blob");
                return;
            }

            if (numUpDownNodeID.Value <= 0)
            {
                Helpers.DisplayInfo(this, "Uploading Blob", "Enter Id for selected Blob.");
                return;
            }

            if (!File.Exists(txtFilePath.Text))
            {
                Helpers.DisplayInfo(this, "Invalid file!");
                return;
            }

            var id = Convert.ToInt32(numUpDownNodeID.Value);

            var count = DAL.Seraph.ExecuteScalar("SELECT COUNT(*) FROM BLOB_TBL WHERE ID = " + id);
            string sql;

            if (Convert.ToInt32(count) == 0)
            {
                sql = @"INSERT INTO BLOB_TBL (ID, BLOB_DATA) 
                            VALUES ( (SELECT NVL(MAX(ID), 0) + 1 AS ID FROM BLOB_TBL), 
                            :BLOB_PARAM)";
            }
            else
            {
                sql = @"UPDATE BLOB_TBL SET BLOB_DATA = :BLOB_PARAM WHERE ID = " + id;
            }

            if (DAL.Seraph.InsertOracleBlob(sql, CreateByteArray(txtFilePath.Text)))
            {
                Helpers.DisplayInfo(this, "File uploaded successfully!", "Uploading Blob");
                txtFilePath.Text = string.Empty;
                numUpDownNodeID.Value = 0;
            }
        }

        private byte[] CreateByteArray(string file)
        {
            byte[] data;
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, Convert.ToInt32(fs.Length));
            }

            return data;
        }
    }
}
