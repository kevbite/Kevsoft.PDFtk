namespace Kevsoft.PDFtk
{
    public interface IPDFtkResult<out TResult>
    {
        string StandardOutput  { get; }
        string StandardError { get; }
        TResult Result { get; }
        int ExitCode { get; }
        bool Success { get; }
    }
}