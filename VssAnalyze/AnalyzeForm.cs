using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VssAnalyze
{
    public partial class AnalyzeForm : Form
    {
        public string VssRepoPath { get { return analyzer1.VssRepoPath; } set { analyzer1.VssRepoPath = value; } }

        private CancellationTokenSource CancellationTokenSource;

        public AnalyzeForm()
        {
            InitializeComponent();
        }

        private async void analyzeButton_Click(object sender, EventArgs e)
        {
            ClearOutput();
            await Run(() => analyzer1.AnalyzeOnly(GetRunRequest()));
            outputTextBox.Text = analyzer1.GetLogLinesAfterAnalyze();
            badFilesListBox.Items.AddRange(analyzer1.GetBadFilesAfterAnalyze());
        }

        private async void repairButton_Click(object sender, EventArgs e)
        {
            ClearOutput();
            await Run(() => analyzer1.Repair(GetRunRequest()));
            outputTextBox.Text = analyzer1.GetLogLinesAfterRepair();
            badFilesListBox.Items.AddRange(analyzer1.GetBadFilesAfterRepair());
        }

        void ClearOutput()
        {
            outputTextBox.Clear();
            badFilesListBox.Items.Clear();
        }

        private async Task Run(Func<Task> taskFunc)
        {
            SetJobRunning(true);
            CancellationTokenSource = new CancellationTokenSource();
            try
            {
                await taskFunc();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetJobRunning(false);
            }
        }

        private void SetJobRunning(bool value)
        {
            progressBar1.Visible = value;
            stopButton.Enabled = value;
        }

        private JobRequest GetRunRequest()
        {
            return new JobRequest
            {
                CancellationToken = CancellationTokenSource.Token,
            };
        }
        
        private void stopButton_Click(object sender, EventArgs e)
        {
            StopJob();
        }

        private void StopJob()
        {
            CancellationTokenSource?.Cancel();
        }

        private void AnalyzeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopJob();
        }
    }
}
