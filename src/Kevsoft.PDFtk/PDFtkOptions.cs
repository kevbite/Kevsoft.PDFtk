using System.Runtime.InteropServices;

namespace Kevsoft.PDFtk
{
    /// <summary>
    /// PDFtk Options
    /// </summary>
    public sealed class PDFtkOptions
    {
        /// <summary>
        /// Initializes a new instance of the PDFtkOptions class.
        /// </summary>
        /// <param name="pdftkPath">The full path to the pdftk executable.</param>
        public PDFtkOptions(string pdftkPath)
        {
            PDFtkPath = pdftkPath;
        }

        /// <summary>
        /// The full path to the pdftk executable.
        /// </summary>
        public string PDFtkPath { get; }

        /// <summary>
        /// Creates the defaults for the PDFtk options.
        /// </summary>
        /// <returns></returns>
        public static PDFtkOptions Default()
        {
            var fileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "pdftk.exe" : "pdftk";
            
            var pdftkPath = PathEnvironmentVariable.GetFileWithPath(fileName);

            return new PDFtkOptions(pdftkPath);
        }
    }
}