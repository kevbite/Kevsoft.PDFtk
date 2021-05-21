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
        /// <param name="pdfFile">A byte array of the PDF file input.</param>
        /// <returns>A result with the number of pages.</returns>
        /// See <see cref="GetNumberOfPagesAsync(Stream)"/> to use a PDF input as a stream or <see cref="GetNumberOfPagesAsync(string)"/> to use a PDF file path. 
        Task<IPDFtkResult<int?>> GetNumberOfPagesAsync(byte[] pdfFile);

        /// <summary>
        /// Return the number of pages for a given PDF.
        /// </summary>
        /// <param name="pdfFile">A stream containing the PDF file input.</param>
        /// <returns>A result with the number of pages.</returns>
        /// See <see cref="GetNumberOfPagesAsync(byte[])"/> to use a PDF input as a byte array or <see cref="GetNumberOfPagesAsync(string)"/> to use a PDF file path. 
        Task<IPDFtkResult<int?>> GetNumberOfPagesAsync(Stream pdfFile);
 
        /// <summary>
        /// Return the number of pages for a given PDF.
        /// </summary>
        /// <param name="filePath">The PDF file path.</param>
        /// <returns>A result with the number of pages.</returns>
        /// See <see cref="GetNumberOfPagesAsync(byte[])"/> to use a PDF input as a byte array. 
        Task<IPDFtkResult<int?>> GetNumberOfPagesAsync(string filePath);
        
        /// <summary>
        /// Concatenate a list of pages into one single file and returns the output PDF as a byte array.
        /// </summary>
        /// <param name="pdfFile">A byte array of the PDF file input.</param>
        /// <param name="pages">The pages to concatenate.</param>
        /// <returns>A result with the PDF as a byte array with the concatenated pages.</returns>
        Task<IPDFtkResult<byte[]>> GetPagesAsync(byte[] pdfFile, params int[] pages);
        
        /// <summary>
        /// Concatenate a list of pages into one single file and returns the output PDF as a byte array.
        /// </summary>
        /// <param name="pdfFile">A stream of the PDF file input.</param>
        /// <param name="pages">The pages to concatenate.</param>
        /// <returns>A result with the PDF as a byte array with the concatenated pages.</returns>
        Task<IPDFtkResult<byte[]>> GetPagesAsync(Stream pdfFile, params int[] pages);
        
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
        /// <param name="pdfFile">A byte array of the PDF file input.</param>
        /// <returns>A result with the form field statistics</returns>
        Task<IPDFtkResult<IDataField[]>> GetDataFieldsAsync(byte[] pdfFile);
        
        /// <summary>
        /// Reads the PDF and returns the form field statistics.
        /// </summary>
        /// <param name="pdfFile">A stream of the PDF file input.</param>
        /// <returns>A result with the form field statistics</returns>
        Task<IPDFtkResult<IDataField[]>> GetDataFieldsAsync(Stream pdfFile);
        
        /// <summary>
        /// Reads the PDF and returns the form field statistics.
        /// </summary>
        /// <param name="filePath">The PDF file path.</param>
        /// <returns>A result with the form field statistics</returns>  
        Task<IPDFtkResult<IDataField[]>> GetDataFieldsAsync(string filePath);
        
        /// <summary>
        /// Merges multiple PDFs into one single PDF and returns the output PDF as a byte array.
        /// </summary>
        /// <param name="pdfFiles">An enumeration of bytes of the PDF files to merge.</param>
        /// <returns>A result with the PDF as a byte array of the merged PDFs.</returns>
        Task<IPDFtkResult<byte[]>> ConcatAsync(IEnumerable<byte[]> pdfFiles);
        
        /// <summary>
        /// Merges multiple PDFs into one single PDF and returns the output PDF as a byte array.
        /// </summary>
        /// <param name="pdfFiles">An enumeration of streams of the PDF files to merge.</param>
        /// <returns>A result with the PDF as a byte array of the merged PDFs.</returns>
        Task<IPDFtkResult<byte[]>> ConcatAsync(IEnumerable<Stream> pdfFiles);
        
        /// <summary>
        /// Merges multiple PDFs into one single PDF and returns the output PDF as a byte array.
        /// </summary>
        /// <param name="filePaths">An enumeration of the PDF file paths to merge.</param>
        /// <returns>A result with the PDF as a byte array of the merged PDFs.</returns>
        Task<IPDFtkResult<byte[]>> ConcatAsync(IEnumerable<string> filePaths);
        
        /// <summary>
        /// Splits a single PDF in many pages and return an enumeration of bytes representing each page a s single PDF.
        /// </summary>
        /// <param name="pdfFile">A byte array of the PDF file input.</param>
        /// <returns>A result with an enumeration of byte arrays.</returns>
        Task<IPDFtkResult<IEnumerable<byte[]>>> SplitAsync(byte[] pdfFile);
        
        /// <summary>
        /// Splits a single PDF in many pages and return an enumeration of bytes representing each page a s single PDF.
        /// </summary>
        /// <param name="pdfFile">A stream of the PDF file input.</param>
        /// <returns>A result with an enumeration of byte arrays.</returns>
        Task<IPDFtkResult<IEnumerable<byte[]>>> SplitAsync(Stream pdfFile);
        
        /// <summary>
        /// Splits a single PDF in many pages and return an enumeration of bytes representing each page a s single PDF.
        /// </summary>
        /// <param name="filePath">The PDF file path.</param>
        /// <returns>A result with an enumeration of byte arrays.</returns>
        Task<IPDFtkResult<IEnumerable<byte[]>>> SplitAsync(string filePath);
        
        /// <summary>
        /// Applies a stamp to a PDF file.
        /// </summary>
        /// <param name="pdfFile">A byte array of the PDF file input.</param>
        /// <param name="stampPdfFile">A byte array of the PDF stamp to apply.</param>
        /// <returns>A result with the stamped PDF file.</returns>
        Task<IPDFtkResult<byte[]>> StampAsync(byte[] pdfFile, byte[] stampPdfFile);
        
        /// <summary>
        /// Applies a stamp to a PDF file.
        /// </summary>
        /// <param name="pdfFile">A stream of the PDF file input.</param>
        /// <param name="stampPdfFile">A stream of the PDF stamp to apply.</param>
        /// <returns>A result with the stamped PDF file.</returns>
        Task<IPDFtkResult<byte[]>> StampAsync(Stream pdfFile, Stream stampPdfFile);
        
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
        /// <param name="pdfFile">A byte array of the PDF file input.</param>
        /// <param name="fieldData">A key value pair of form field name to value.</param>
        /// <param name="flatten">If the final PDF should be flattened.</param>
        /// <param name="dropXfa">If the XFA data should be omitted from final PDF.</param>
        /// <returns>A result with the PDF form filled as a byte array.</returns>
        Task<IPDFtkResult<byte[]>> FillFormAsync(byte[] pdfFile,
            IReadOnlyDictionary<string, string> fieldData,
            bool flatten = true,
            bool dropXfa = false);

        /// <summary>
        /// Fill a PDF form with given data and returns the output PDF as a byte array.
        /// </summary>
        /// <param name="pdfFile">A stream of the PDF file input.</param>
        /// <param name="fieldData">A key value pair of form field name to value.</param>
        /// <param name="flatten">If the final PDF should be flattened.</param>
        /// <param name="dropXfa">If the XFA data should be omitted from final PDF.</param>
        /// <returns>A result with the PDF form filled as a byte array.</returns>
        Task<IPDFtkResult<byte[]>> FillFormAsync(Stream pdfFile,
            IReadOnlyDictionary<string, string> fieldData,
            bool flatten = true,
            bool dropXfa = false);

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
    }
}