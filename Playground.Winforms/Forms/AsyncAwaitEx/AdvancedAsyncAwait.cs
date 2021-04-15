using Playground.Core.AdoNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Playground.Winforms.Forms.AsyncAwaitEx
{
    public partial class AdvancedAsyncAwait : Form
    {
        private ExtendedBackgroundWorker _extendedBackgroundWorker;
        private readonly DataTable _employeeData;

        public AdvancedAsyncAwait()
        {
            InitializeComponent();

            _employeeData = DAL.Seraph.ExecuteQuery(@"
                    SELECT A.*, B.DEPARTMENT_NAME
                    FROM DEMO_EMPLOYEE A
                        INNER JOIN DEMO_DEPARTMENT B
                            ON A.DEPARTMENT_ID = B.DEPARTMENT_ID 
                    ORDER BY A.ID");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            progressBar1.Minimum = 0;
            progressBar1.Maximum = _employeeData.Rows.Count;

            _extendedBackgroundWorker = new ExtendedBackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            _extendedBackgroundWorker.DoWork += ExtendedBackgroundWorker_DoWork;
            _extendedBackgroundWorker.RunWorkerCompleted += ExtendedBackgroundWorker_RunWorkerCompleted;
            _extendedBackgroundWorker.RunWorkerAsync(1);
        }

        private void ExtendedBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show(@"Operation cancelled by user.", @"Thread Aborted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
            else if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
            else
            {
                MessageBox.Show(e.Result.ToString());
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }

            label1.Text = "";
        }

        private void ExtendedBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            btnStart.BeginInvoke(new Action(() =>
            {
                btnStart.Enabled = false;
                btnStop.Enabled = true;
            }));

            ExtendedBackgroundWorker helperBw = sender as ExtendedBackgroundWorker;
            int arg = (int)e.Argument;
            e.Result = LoadDataAsync(helperBw, arg);
            if (helperBw != null && helperBw.CancellationPending)
            {
                e.Cancel = true;
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
        }

        private string LoadDataAsync(ExtendedBackgroundWorker helperBw, int arg)
        {
            var retVal = @"Loading employee data thread completed successfully!";
            Console.WriteLine(arg);

            try
            {
                var index = 0;
                foreach (DataRow r in _employeeData.Rows)
                {
                    var empFullName = $"{r["FIRST_NAME"]} {r["LAST_NAME"]}";
                    var jobTitle = r["JOB_TITLE"].ToString();
                    var salary = r["SALARY"].ToString();

                    label1.BeginInvoke(new Action(() =>
                    {
                        label1.Text = $"Now processing data for: {empFullName}";
                    }));

                    Thread.Sleep(3000);        
                    
                    var msg = $"{empFullName} - {salary} - {jobTitle} {Environment.NewLine}";

                    richTextBox1.BeginInvoke(new Action(() =>
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        richTextBox1.AppendText(msg);
                        richTextBox1.AppendText("------------------------------------\n");
                    }));

                    progressBar1.BeginInvoke(new Action(() =>
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        progressBar1.Value = index;
                    }));

                    index++;
                }
            }
            catch (Exception ex)
            {
                retVal = ex.Message;
            }

            return retVal;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(@"Are you sure, you want to abort the current thread?", @"Confirm Abort", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _extendedBackgroundWorker.StopImmediately();
                richTextBox1.AppendText("*** Cancelled by User ***");
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
