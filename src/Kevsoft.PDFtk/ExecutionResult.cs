namespace Kevsoft.PDFtk
{
    internal class ExecutionResult
    {
        public ExecutionResult(int exitCode, string standardOutput, string standardError)
        {
            ExitCode = exitCode;
            StandardOutput = standardOutput;
            StandardError = standardError;
        }

        public string StandardOutput { get; }
        public string StandardError { get; }
        public int ExitCode { get; }

        public bool Success => ExitCode == 0;
    }
}