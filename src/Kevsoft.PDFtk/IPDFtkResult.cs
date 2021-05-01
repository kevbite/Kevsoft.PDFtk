namespace Kevsoft.PDFtk
{
    public interface IPDFtkResult<out TResult>
    {
        public string StandardOutput  { get; }
        public string StandardError { get; }
        public TResult Result { get; }
        public int ExitCode { get; }
        public bool Success { get; }
    }
}