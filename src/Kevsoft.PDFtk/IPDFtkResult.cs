namespace Kevsoft.PDFtk
{
    /// <summary>
    /// The result of a PDFtk execution.
    /// </summary>
    /// <typeparam name="TResult">The enclosed result type.</typeparam>
    public interface IPDFtkResult<out TResult>
    {
        /// <summary>
        /// The standard output captured when PDFtk was executed.
        /// </summary>
        string StandardOutput  { get; }
        /// <summary>
        /// The standard error captured when PDFtk was executed.
        /// </summary>
        string StandardError { get; }
        /// <summary>
        /// The result of the execution.
        /// </summary>
        TResult Result { get; }
        /// <summary>
        /// The exit code of the execution of PDFtk.
        /// </summary>
        int ExitCode { get; }
        /// <summary>
        /// Success flag of the execution of PDFtk.
        /// </summary>
        bool Success { get; }
    }
}