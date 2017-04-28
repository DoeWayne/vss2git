using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace VssAnalyze
{
    internal class Job
    {
        private TaskCompletionSource<int> RunTask = new TaskCompletionSource<int>();
        private Process Process;
        private JobRequest Request;

        public Task Run(JobRequest request, string workingDir, string appPath, string args)
        {
            Request = request;
            var startInfo = new ProcessStartInfo
            {
                WorkingDirectory = workingDir,
                FileName = appPath,
                Arguments = args
            };
            Process = Process.Start(startInfo);
            Process.Exited += Process_Exited;
            Process.EnableRaisingEvents = true;
            Request.CancellationToken.Register(() => Process?.Kill());
            return RunTask.Task;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            HandleProcessExit();
        }

        private void HandleProcessExit()
        {
            RunTask.SetResult(Process.ExitCode);
            Process.Exited -= Process_Exited;
            Process.Dispose();
            Process = null;
        }

    }

    public class JobRequest
    {
        public CancellationToken CancellationToken { get; set; }
    }
}
