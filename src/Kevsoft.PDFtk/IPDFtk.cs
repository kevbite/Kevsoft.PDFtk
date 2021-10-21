using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Kevsoft.PDFtk
{
    /// <summary>
    /// The main <c>IPDFtk</c> interface.
    /// Contains all methods for performing the PDFtk functions.
    /// </summary>
    public interface IPDFtk
    {
        /// <summary>
        /// Return the number of pages for a given PDF.
        /// </summary>
        /// <param name="filePath">The PDF file path.</param>
        /// <returns>A result with the number of pages.</returns>
        /// See <see cref="PDFtkByteArrayExtensions.GetNumberOfPagesAsync(IPDFtk, byte[])"/> to use a PDF input as a byte array. 
        Task<IPDFtkResult<int?>> GetNumberOfPagesAsync(string filePath);
        
        /// <summary>
        /// Concatenate a list of pages into one single file and returns the output PDF as a byte array.
        /// </summary>
        /// <param name="filePath">The PDF file path.</param>
        /// <param name="pages">The pages to concatenate.</param>
        /// <returns>A result with the PDF as a byte array with the concatenated pages.</returns>
        Task<IPDFtkResult<byte[]>> GetPagesAsync(string filePath, params int[] pages);
        
        /// <summary>
        /// Reads the PDF and returns the form field statistics.
        /// </summary>
        /// <param name="filePath">The PDF file path.</param>
        /// <returns>A result with the form field statistics</returns>  
        Task<IPDFtkResult<IDataField[]>> GetDataFieldsAsync(string filePath);

        /// <summary>
        /// Merges multiple PDFs into one single PDF and returns the output PDF as a byte array.
        /// </summary>
        /// <param name="filePaths">An enumeration of the PDF file paths to merge.</param>
        /// <returns>A result with the PDF as a byte array of the merged PDFs.</returns>
        Task<IPDFtkResult<byte[]>> ConcatAsync(IEnumerable<string> filePaths);
        
        /// <summary>
        /// Splits a single PDF in many pages and return an enumeration of bytes representing each page a s single PDF.
        /// </summary>
        /// <param name="filePath">The PDF file path.</param>
        /// <returns>A result with an enumeration of byte arrays.</returns>
        Task<IPDFtkResult<IEnumerable<byte[]>>> SplitAsync(string filePath);
        
        /// <summary>
        /// Applies a stamp to a PDF file.
        /// </summary>
        /// <param name="pdfFilePath">A PDF file path.</param>
        /// <param name="stampPdfFilePath">A PDF stamp file path.</param>
        /// <returns>A result with the stamped PDF file.</returns>
        Task<IPDFtkResult<byte[]>> StampAsync(string pdfFilePath, string stampPdfFilePath);
        
        /// <summary>
        /// Fill a PDF form with given data and returns the output PDF as a byte array.
        /// </summary>
        /// <param name="pdfFilePath">A PDF file path.</param>
        /// <param name="fieldData">A key value pair of form field name to value.</param>
        /// <param name="flatten">If the final PDF should be flattened.</param>
        /// <param name="dropXfa">If the XFA data should be omitted from final PDF.</param>
        /// <returns>A result with the PDF form filled as a byte array.</returns>
        Task<IPDFtkResult<byte[]>> FillFormAsync(string pdfFilePath,
            IReadOnlyDictionary<string, string> fieldData,
            bool flatten = true,
            bool dropXfa = false);
        
        /// <summary>
        /// Replaces a page in a PDF with another PDF
        /// </summary>
        /// <param name="pdfFilePath">A PDF file path input.</param>
        /// <param name="page">The page to replace</param>
        /// <param name="replacementFilePath">A PDF file path to replace the page with.</param>
        /// <returns>A result with the PDF form filled as a byte array.</returns>
        Task<IPDFtkResult<byte[]>> ReplacePage(string pdfFilePath, int page, string replacementFilePath);
        
        /// <summary>
        /// Replaces a range of pages in a PDF with another PDF
        /// </summary>
        /// <param name="pdfFilePath">A PDF file path input.</param>
        /// <param name="startPage">The page to replace</param>
        /// <param name="endPage">The page to replace</param>
        /// <param name="replacementFilePath">A PDF file path to replace the page with.</param>
        /// <returns>A result with the PDF form filled as a byte array.</returns>
        Task<IPDFtkResult<byte[]>> ReplacePages(string pdfFilePath, int startPage, int endPage, string replacementFilePath);

    }
}