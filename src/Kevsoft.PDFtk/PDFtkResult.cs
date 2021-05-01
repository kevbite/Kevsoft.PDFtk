namespace Kevsoft.PDFtk
{
    public sealed class PDFtkResult<TResult> : IPDFtkResult<TResult>
    {
        private readonly ExecutionResult _executionResult;

        internal PDFtkResult(ExecutionResult executionResult, TResult result)
        {
            _executionResult = executionResult;
            Result = result;
        }

        public string StandardOutput => _executionResult.StandardOutput;
        public string StandardError => _executionResult.StandardError;
        public TResult Result { get; }
        public int ExitCode => _executionResult.ExitCode;
        public bool Success => ExitCode == 0;
    }
}