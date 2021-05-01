namespace Kevsoft.PDFtk
{
    public sealed class PDFtkResult<TResult> : IPDFtkResult<TResult>
    {
        private readonly PDFtk.ExecuteProcessResult _executeProcessResult;

        internal PDFtkResult(PDFtk.ExecuteProcessResult executeProcessResult, TResult result)
        {
            _executeProcessResult = executeProcessResult;
            Result = result;
        }

        public string StandardOutput => _executeProcessResult.StandardOutput;
        public string StandardError => _executeProcessResult.StandardError;
        public TResult Result { get; }
        public int ExitCode => _executeProcessResult.ExitCode;
        public bool Success => ExitCode == 0;
    }
}