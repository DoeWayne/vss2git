using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VssAnalyze
{
    public partial class Analyzer : Component
    {
        public string VssRepoPath { get; set; }
        public IProgress<string> Output { get; set; }

        private string FindAnalyzeTool()
        {
            var appPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var analyzeExePath = Path.Combine(appPath, "Microsoft Visual SourceSafe\\analyze.exe");
            if (!File.Exists(analyzeExePath))
            {
                throw new FileNotFoundException("Could not find VSS tool ANALYZE.EXE.");
            }
            return analyzeExePath;
        }

        public Task AnalyzeOnly(JobRequest request)
        {
            ValidateInputs();
            string analyzeResultPath = Path.Combine(VssRepoPath, "analyze-results");
            if (!Directory.Exists(analyzeResultPath))
                Directory.CreateDirectory(analyzeResultPath);
            return RunAnalyzeTool(request, "-banalyze-results -v4 -i- data");
        }

        private void ValidateInputs()
        {
            if (!Directory.Exists(VssRepoPath))
                throw new DirectoryNotFoundException("The given VSS repository path does not exist.");
        }

        public Task Repair(JobRequest request)
        {
            ValidateInputs();
            string analyzeResultPath = Path.Combine(VssRepoPath, "analyze-results-fix");
            if (!Directory.Exists(analyzeResultPath))
                Directory.CreateDirectory(analyzeResultPath);
            return RunAnalyzeTool(request, "-banalyze-results-fix -f -d -v4 -i- data");
        }

        public async Task RunAnalyzeTool(JobRequest request, string args)
        {
            var job = new Job();
            await job.Run(request, VssRepoPath, FindAnalyzeTool(), args);
        }

        public string GetLogLinesAfterAnalyze()
        {
            return GetLogLines("analyze-results");
        }

        public string GetLogLinesAfterRepair()
        {
            return GetLogLines("analyze-results-fix");
        }

        private string GetLogLines(string analyzeFolder)
        {
            var logFile = Path.Combine(VssRepoPath, analyzeFolder, "analyze.log");
            if (!File.Exists(logFile))
                return "";
            return File.ReadAllText(logFile);
        }

        public string[] GetBadFilesAfterAnalyze()
        {
            return GetBadFiles("analyze-results");
        }

        public string[] GetBadFilesAfterRepair()
        {
            return GetBadFiles("analyze-results-fix");
        }

        private string[] GetBadFiles(string analyzeFolder)
        {
            var path = Path.Combine(VssRepoPath, analyzeFolder, "analyze.bad");
            if (!File.Exists(path))
                return new string[0];
            return File.ReadAllLines(path);
        }
    }

}
