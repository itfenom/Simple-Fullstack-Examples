using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Playground.Winforms.Forms.Examples
{
    public partial class ReadAndWriteToTxtFile : BaseForm
    {
        // ReSharper disable once NotAccessedField.Local
        private DialogResult _iAnswer;
        private readonly string _debugFolderPath = Directory.GetCurrentDirectory();
        private int _runningTotal;
        private string _filePath = string.Empty;
        private const int TargetValue = 500;

        public ReadAndWriteToTxtFile()
        {
            InitializeComponent();
        }

        public void ShowForm(Form oParent)
        {
            _iAnswer = ShowDialog(oParent);
            Close();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ReadAndWriteToTxtFile_Load(object sender, EventArgs e)
        {
            _filePath = _debugFolderPath + @"\" + "sourceFile.nfo";
            _runningTotal = GetCurrentValues();
        }

        private int GetCurrentValues()
        {
            var retVal = 0;
            string currentLine;
            string[] currentValue;
            int Logs;
            var list = new List<int>();
            try
            {
                using (var reader = new StreamReader(_filePath))
                {
                    while ((currentLine = reader.ReadLine()) != null)
                    {
                        currentValue = currentLine.Split(',');
                        Logs = Convert.ToInt32(currentValue[1]);
                        list.Add(Logs);
                    }
                }
                retVal = list.Sum();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace, @"Information", MessageBoxButtons.OK);
            }
            return retVal;
        }

        private void InsertValues(int num)
        {
            try
            {
                if (_runningTotal < TargetValue)
                {
                    string log = DateTime.Now + " , " + num;
                    if (!File.Exists(_filePath))
                    {
                        using (var fs = new FileStream(_filePath, FileMode.Create, FileAccess.Write))
                        {
                            using (var writer = new StreamWriter(fs))
                            {
                                writer.WriteLine(log);
                                rTxtBox.AppendText("Values inserted successfully: " + log);
                                rTxtBox.AppendText("\n");
                                txtValue.Clear();
                                txtValue.Focus();
                            }
                        }
                    }
                    else
                    {
                        using (var fs = new FileStream(_filePath, FileMode.Append, FileAccess.Write))
                        {
                            using (var writer = new StreamWriter(fs))
                            {
                                writer.WriteLine(log);
                                rTxtBox.AppendText("Values inserted successfully: " + log);
                                rTxtBox.AppendText("\n");
                                txtValue.Clear();
                                txtValue.Focus();
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show(@"You have achieved your goal of '" + TargetValue + @"' ", @"Information", MessageBoxButtons.OK);
                    txtValue.Clear();
                    txtValue.Focus();
                    GetTotals();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace, @"Information", MessageBoxButtons.OK);
            }
        }

        private void GetTotals()
        {
            using (var reader = new StreamReader(_filePath))
            {
                string line;
                string[] currentLog;
                int pos = 0;
                int total = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    currentLog = line.Split(',');
                    var log = int.Parse(currentLog[1]);
                    total += log;
                    pos++;
                }
                var remaining = TargetValue - total;
                rTxtBox.AppendText("\n");
                rTxtBox.AppendText("Total Entries: " + total + ".\n");
                rTxtBox.AppendText("Count: " + pos + ".\n");
                rTxtBox.AppendText("Remaining: " + remaining + ".\n");
            }
        }

        private void BtnInsert_Click(object sender, EventArgs e)
        {
            string logValue = txtValue.Text.Trim();
            int num;
            bool isNum = int.TryParse(logValue, out num);
            if (isNum)
            {
                InsertValues(num);
            }
            else
            {
                MessageBox.Show(@"Invalid values entered. Try again!");
                txtValue.Clear();
                txtValue.Focus();
            }
        }

        private void BtnShowAll_Click(object sender, EventArgs e)
        {
            using (var reader = new StreamReader(_filePath))
            {
                string line;
                rTxtBox.AppendText("\n=== All Entries ===\n");
                while ((line = reader.ReadLine()) != null)
                {
                    rTxtBox.AppendText(line + "\n");
                }
            }
        }

        private void BtnTotal_Click(object sender, EventArgs e)
        {
            GetTotals();
        }
    }
}
