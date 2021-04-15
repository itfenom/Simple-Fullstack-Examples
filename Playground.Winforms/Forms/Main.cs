using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Playground.Core;
using Playground.Core.Diagnostics;
using Playground.Core.Utilities;
using Playground.Winforms.Forms.Admin;
using Playground.Winforms.Forms.AsyncAwaitEx;
using Playground.Winforms.Forms.Examples;
using Playground.Winforms.Forms.LoginStuff;
using Playground.Winforms.Forms.Misc;
using Playground.Winforms.HelperForms;
using Playground.Winforms.Utilities;

namespace Playground.Winforms.Forms
{
    public partial class Main : Form
    {
        private string _msg = string.Empty;
        private bool _showWelcomeMsg;
        private Timer _timer;
        private string _loginMsg = string.Empty;

        public Main()
        {
            InitializeComponent();

            LoadLoginForm();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            _showWelcomeMsg = (bool)Properties.Settings.Default["ViewLoginMessage"];

            HandleWelcomeMessage(_showWelcomeMsg);

            _timer = new Timer {Interval = 200};
            _timer.Tick += Timer_Tick;
            _timer.Start();

            AppShutDownChecker.MainForm = this;
            FileCleanup.StartThread();
            AppShutDownChecker.StartThread();
        }

        #region Connection Menu

        private void LoginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadLoginForm();
        }

        private void LoadLoginForm()
        {
            var oForm = new Login();
            var isLoginSuccessful = oForm.ShowForm(this);

            if (!isLoginSuccessful)
            {
                UserInfo.ActiveUser = null;
            }

            if (UserInfo.ActiveUser != null)
            {
                lblLoginMessage.ForeColor = Color.Green;
                lblLoginMessage.Font = new Font("Arial", 14, FontStyle.Italic);
                lblLoginMessage.Text = @"Login Success. Welcome!";

                Text += @" " + UserInfo.ActiveUser.DisplayName + @" Logged in as: " + UserInfo.ActiveUser.RoleName;

                loginToolStripMenuItem.Enabled = false;
                logoutToolStripMenuItem.Enabled = true;
            }
            else
            {
                lblLoginMessage.ForeColor = Color.Red;
                lblLoginMessage.Font = new Font("Arial", 14, FontStyle.Italic);
                lblLoginMessage.Text = @"Login Failed!";
                logoutToolStripMenuItem.Enabled = false;
                loginToolStripMenuItem.Enabled = true;
            }

            SetupAdminMenus();

            _loginMsg = lblLoginMessage.Text + "... ";
        }

        private void SetupAdminMenus()
        {
            if (UserInfo.ActiveUser == null || UserInfo.ActiveUser.RoleId != 1)
            {
                //manageUsersToolStripMenuItem.Enabled = false;
                //manageAccountsToolStripMenuItem.Enabled = false;
                //viewAccountHistoryToolStripMenuItem.Enabled = false;
                //manageAppSettingToolStripMenuItem.Enabled = false;
            }
            else if (UserInfo.ActiveUser != null && UserInfo.ActiveUser.RoleId == 1)
            {
                //manageUsersToolStripMenuItem.Enabled = true;
                //manageAccountsToolStripMenuItem.Enabled = true;
                //viewAccountHistoryToolStripMenuItem.Enabled = true;
                //manageAppSettingToolStripMenuItem.Enabled = true;
            }
        }

        private void LogoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserInfo.ActiveUser = null;
            Text = @"Logged Out!";
            SetupAdminMenus();
            lblLoginMessage.Text = @"Logged out!";
            loginToolStripMenuItem.Enabled = true;
            logoutToolStripMenuItem.Enabled = false;
            _loginMsg = lblLoginMessage.Text + "... ";
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(@"Are you sure, you want to exit?", @"Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Logger.Info(@"Closing application Playground.Winforms");
                FileCleanup.StopThread();
                AppShutDownChecker.StopThread();
                Application.Exit();
            }
        }

        private void ViewWelcomeMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_showWelcomeMsg)
            {
                _showWelcomeMsg = false;
                Properties.Settings.Default["ViewLoginMessage"] = false;
                Properties.Settings.Default.Save();
                HandleWelcomeMessage(_showWelcomeMsg);
            }
            else
            {
                _showWelcomeMsg = true;
                Properties.Settings.Default["ViewLoginMessage"] = true;
                Properties.Settings.Default.Save();
                HandleWelcomeMessage(_showWelcomeMsg);
            }
        }

        private void HandleWelcomeMessage(bool value)
        {
            if (value)
            {
                viewWelcomeMessageToolStripMenuItem.Checked = true;
                lblLoginMessage.Visible = true;
            }
            else
            {
                viewWelcomeMessageToolStripMenuItem.Checked = false;
                lblLoginMessage.Visible = false;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ScrollText(ref lblLoginMessage, _loginMsg, 20);
        }

        private void ScrollText(ref Label lbl, string lblText, int maxCharDisplay)
        {
            string lblTextTotal = lblText + lblText;
            if (maxCharDisplay > (lblText.Length - 1))
            {
                maxCharDisplay = (lblText.Length - 1);
            }

            if (lbl.Tag == null)
            {
                lbl.Tag = 0;
            }
            
            int curIndex = Convert.ToInt32(lbl.Tag.ToString());

            lbl.Text = lblTextTotal.Substring(curIndex, maxCharDisplay);
            curIndex++;
            if (curIndex > (lblText.Length - 1)) curIndex = 0;
            lbl.Tag = curIndex;
        }

        #endregion

        #region Misc Menu

        private void EncryptDecryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new EncryptDecrypt();
            form.ShowDialog();
        }

        private void NumericTextBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _msg = string.Empty;

            _msg = @"In KeyPress event of textBox." +
                   "\nif(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')\n{\n" +
                   "e.Handled = true;\n}\n\nif (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)\n{\n" +
                   "e.Handled = true;\n}";

            MessageBoxWithTextBox.Show(_msg, "Handle number only TextBox", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void TopMostMessageBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(new Form() { TopMost = true }, @"Hi! This message box is on top.", @"Message Box on top", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void ExceptionHandlingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var a = 10;
                Console.WriteLine(a/0);
            }
            finally
            {
                Dispose();
            }
        }

        private void RemoveDuplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var list1 = new List<string>
            {
                "PROD",
                "PROD",
                "PROD",
                "TEST",
                "TEST",
                "TEST",
                "DEV",
                "DEV",
                "DEV"
            };


            var sb = new StringBuilder();

            foreach (string item in list1)
            {
                sb.AppendLine(item);
            }

            Helpers.DisplayInfo(this, sb.ToString(), "List<string> before removing duplicates.");

            list1 = HelperTools.RemoveDuplicatesFromCollection(list1);

            sb.Clear();

            foreach (string item in list1)
            {
                sb.AppendLine(item);
            }

            Helpers.DisplayInfo(this, sb.ToString(), "List<string> After removing duplicates.");
        }

        private void EncodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new InputBox();
            var value = form.ShowForm(this, "Value to Encode: ", "Encoding");

            if (!string.IsNullOrEmpty(value))
            {
                var outputString = HelperTools.Encode(value.Trim());
                Helpers.DisplayInfo(this, $"Encoded string is {outputString}");
            }
            else
            {
                Helpers.DisplayInfo(this, "Missing values. Try again.");
            }
        }

        private void DecodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new InputBox();
            var value = form.ShowForm(this, "Value to Decode: ", "Decoding");

            if (!string.IsNullOrEmpty(value))
            {
                var outputString = HelperTools.Decode(value.Trim());
                Helpers.DisplayInfo(this, $"Decoded string is {outputString}");
            }
            else
            {
                Helpers.DisplayInfo(this, "Missing values. Try again.");
            }
        }

        private void SplitLargeStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
           var msg = "This is just a random string that will be split into multiple string of 20 characters long each.";

            if (msg.Length > 15)
            {
                var val = HelperTools.SplitLargeString(msg, 20);

                // ReSharper disable once PossibleMultipleEnumeration
                if (val.Any())
                {
                    int counter = 1;
                    // ReSharper disable once PossibleMultipleEnumeration
                    foreach (var str in val)
                    {
                        Helpers.DisplayInfo(this, "string # " + counter + " :\n" + str, "Splitting large string into smaller string size.");
                        counter++;
                    }
                }
            }
        }

        private void RedTextMessageBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult = Helpers.YesNoRedTextMessageBox("MessageBox with Red Text", "Some message to display!!!!");
        }

        private void basicAsyncAwaitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var asyncAwaits = new BasicAsyncAwaitEx.AsyncAndAwaitEx();
            asyncAwaits.Show();
        }

        #endregion

        #region File Operations
        private void CreateFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var path = Directory.GetCurrentDirectory();
            var fileName = "SampleFile1000Rows_" + DateTime.Now.ToString("MM_dd_yyyy") + ".txt";
            var fileNameWithPath = Path.Combine(path, fileName);

            if (!File.Exists(fileNameWithPath))
            {
                using (StreamWriter sWriter = new StreamWriter(fileNameWithPath))
                {
                    for (int i = 1; i < 1001; i++)
                    {
                        sWriter.WriteLine(i.ToString() + " | " + DateTime.Now.ToShortDateString());
                    }
                }

                Helpers.DisplayInfo(this, $"File:\n{fileNameWithPath}\ncreated successfully in bin folder!");
            }
            else
            {
                Helpers.DisplayInfo(this, $"File:\n{fileNameWithPath}\nalready exist in the bin folder.");

            }
        }

        private void RowInFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var path = Directory.GetCurrentDirectory();
            string[] fileList = Directory.GetFiles(path, "*.txt");

            if (fileList.Length > 0)
            {
                foreach (string file in fileList)
                {
                    var counter = FileOperations.GetNumberOfRecordsInFile(file);
                    Helpers.DisplayInfo(this, $"There are {counter.ToString()} rows in File:\n{file}\n");
                }
            }
            else
            {
                Helpers.DisplayInfo(this, $"No .txt file found in\n{path}\nfolder.");
            }
        }

        private void SplitFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var path = Directory.GetCurrentDirectory();
            string[] fileList = Directory.GetFiles(path, "*.txt");

            if (fileList.Length > 0)
            {
                foreach (string file in fileList)
                {
                    if (FileOperations.SplitLargeFileIntoSmallFiles(file, path, 250))
                    {
                        Helpers.DisplayInfo(this, $"File\n{file}\nsplitted successfully.\nNew Files are created at\n{path}\nEach file contains {250} rows.\n");
                    }
                    else
                    {
                        Helpers.DisplayError(this, "Error occurred in splitting file\n" + file + "\n");
                    }
                }
            }
            else
            {
                Helpers.DisplayInfo(this, $"No .txt file found in\n{path}\nfolder.");
            }
        }

        private void DeleteSplitFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var path = Directory.GetCurrentDirectory();
            var numOfDays = -1;
            var filePattern = "*.spt";

            if (FileOperations.DeleteOlderFiles(path, numOfDays, filePattern))
            {
                Helpers.DisplayInfo(this, $"All file(s) in folder:\n{path}\n With pattern {filePattern} \nOlder than {numOfDays} day(s) deleted successfully.");
            }
            else
            {
                Helpers.DisplayInfo(this, $"No file(s) older than {numOfDays} days\nwith pattern {filePattern} in folder:\n{path}  \nTherefore no file(s) got deleted at this time.");
            }
        }
        #endregion

        #region Examples Menu
        private void OracleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new OracleDb();
            form.ShowDialog();
        }

        private void ReadWriteToTextFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new ReadAndWriteToTxtFile();
            form.ShowForm(this);
        }

        private void LoadExcelFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new LoadExcelFile();
            form.ShowForm(this);
        }

        private void ListViewExampleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new ListViewEx();
            form.ShowForm(this);
        }

        private void WorldWithTreeViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new WorldTreeView();
            form.ShowDialog();
        }
        #endregion

        #region DataGridEx
        private void DataGridToExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new DataGridToExcel();
            form.ShowForm(this);
        }

        private void DataGridComboBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new DataGridComboBox();
            form.ShowForm(this);
        }

        private void DataGridColumnFiltersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new DataGridColumnFilters();
            form.ShowForm(this);
        }
        #endregion

        #region Admin Menu
        private void ManageUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new ManageUsers();
            form.ShowForm(this);
        }

        private void ManageOnlineAccountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new AccountMgr();
            form.ShowDialog();
        }
        private void ViewAccountHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new AccountHistory();
            form.ShowForm(this);
        }
        #endregion

        public delegate DialogResult AppShutDownDialogResultDelegate(string msg);

        public DialogResult ShowMessageBoxFromAppShutDownCheckerThread(string msg)
        {
            if(InvokeRequired)
            {
                return (DialogResult)Invoke(new AppShutDownDialogResultDelegate(ShowMessageBoxFromAppShutDownCheckerThread), msg);
                          
            }

            return MessageBox.Show(new Form { TopMost = true }, msg, "Playground.Winform", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void advancedAsyncAwaitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new AdvancedAsyncAwait();
            form.ShowDialog();
        }
    }
}
