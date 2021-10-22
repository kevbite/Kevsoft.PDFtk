using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kevsoft.PDFtk
{
    /// <summary>
    /// Extensions on top of IPDFtk for dealing with streams of PDFs
    /// </summary>
    public static class PDFtkStreamExtensions
    {
        /// <summary>
        /// Return the number of pages for a given PDF.
        /// </summary>
        /// <param name="pdftk">The IPDFtk object.</param>
        /// <param name="pdfFile">A stream containing the PDF file input.</param>
        /// <returns>A result with the number of pages.</returns>
        /// See <see cref="PDFtkByteArrayExtensions.GetNumberOfPagesAsync(IPDFtk, byte[])"/> to use a PDF input as a byte array or <see cref="IPDFtk.GetNumberOfPagesAsync(string)"/> to use a PDF file path. 
        public static async Task<IPDFtkResult<int?>> GetNumberOfPagesAsync(this IPDFtk pdftk, Stream pdfFile)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);

            return await pdftk.GetNumberOfPagesAsync(inputFile.TempFileName);
        }
        
        /// <summary>
        /// Concatenate a list of pages into one single file and returns the output PDF as a byte array.
        /// </summary>
        /// <param name="pdftk">The IPDFtk object.</param>
        /// <param name="pdfFile">A stream of the PDF file input.</param>
        /// <param name="pages">The pages to concatenate.</param>
        /// <returns>A result with the PDF as a byte array with the concatenated pages.</returns>
        public static async Task<IPDFtkResult<byte[]>> GetPagesAsync(this IPDFtk pdftk, Stream pdfFile, params int[] pages)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);

            return await pdftk.GetPagesAsync(inputFile.TempFileName, pages);
        }

        /// <summary>
        /// Reads the PDF and returns the form field statistics.
        /// </summary>
        /// <param name="pdftk">The IPDFtk object.</param>
        /// <param name="pdfFile">A stream of the PDF file input.</param>
        /// <returns>A result with the form field statistics</returns>
        public static async Task<IPDFtkResult<IDataField[]>> GetDataFieldsAsync(this IPDFtk pdftk, Stream pdfFile)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);

            return await pdftk.GetDataFieldsAsync(inputFile.TempFileName);
        }

        /// <summary>
        /// Merges multiple PDFs into one single PDF and returns the output PDF as a byte array.
        /// </summary>
        /// <param name="pdftk">The IPDFtk object.</param>
        /// <param name="pdfFiles">An enumeration of streams of the PDF files to merge.</param>
        /// <returns>A result with the PDF as a byte array of the merged PDFs.</returns>
        public static async Task<IPDFtkResult<byte[]>> ConcatAsync(this IPDFtk pdftk, IEnumerable<Stream> pdfFiles)
        {
            var inputFiles = await Task.WhenAll(
                pdfFiles.Select(async file => await TempPDFtkFile.FromAsync(file))
                    .ToList());

            try
            {
                return await pdftk.ConcatAsync(inputFiles.Select(x => x.TempFileName));
            }
            finally
            {
                inputFiles.Dispose();
            }
        }
        
        /// <summary>
        /// Splits a single PDF in many pages and return an enumeration of bytes representing each page a s single PDF.
        /// </summary>
        /// <param name="pdftk">The IPDFtk object.</param>
        /// <param name="pdfFile">A stream of the PDF file input.</param>
        /// <returns>A result with an enumeration of key value pair where the key is the filename and the value is a byte arrays.</returns>
        public static async Task<IPDFtkResult<IEnumerable<KeyValuePair<string, byte[]>>>> SplitAsync(this IPDFtk pdftk, Stream pdfFile)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);

            return await pdftk.SplitAsync(inputFile.TempFileName);
        }
        
        /// <summary>
        /// Applies a stamp to a PDF file.
        /// </summary>
        /// <param name="pdftk">The IPDFtk object.</param>
        /// <param name="pdfFile">A stream of the PDF file input.</param>
        /// <param name="stampPdfFile">A stream of the PDF stamp to apply.</param>
        /// <returns>A result with the stamped PDF file.</returns>
        public static async Task<IPDFtkResult<byte[]>> StampAsync(this IPDFtk pdftk, Stream pdfFile, Stream stampPdfFile)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);
            using var stampFile = await TempPDFtkFile.FromAsync(stampPdfFile);

            return await pdftk.StampAsync(inputFile.TempFileName, stampFile.TempFileName);
        }
        
        /// <summary>
        /// Fill a PDF form with given data and returns the output PDF as a byte array.
        /// </summary>
        /// <param name="pdftk">The IPDFtk object.</param>
        /// <param name="pdfFile">A stream of the PDF file input.</param>
        /// <param name="fieldData">A key value pair of form field name to value.</param>
        /// <param name="flatten">If the final PDF should be flattened.</param>
        /// <param name="dropXfa">If the XFA data should be omitted from final PDF.</param>
        /// <returns>A result with the PDF form filled as a byte array.</returns>
        public static async Task<IPDFtkResult<byte[]>> FillFormAsync(this IPDFtk pdftk, Stream pdfFile,
            IReadOnlyDictionary<string, string> fieldData,
            bool flatten,
            bool dropXfa)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);

            return await pdftk.FillFormAsync(inputFile.TempFileName,
                fieldData,
                flatten,
                dropXfa);
        }
        
        /// <summary>
        /// Replaces a page in a PDF with another PDF
        /// </summary>
        /// <param name="pdftk">The IPDFtk object.</param>
        /// <param name="pdfFile">A stream of the PDF file input.</param>
        /// <param name="page">The page to replace</param>
        /// <param name="replacementPdfFile">A stream of the PDF file to replace the page with.</param>
        /// <returns>A result with the PDF form filled as a byte array.</returns>
        public static async Task<IPDFtkResult<byte[]>> ReplacePage(this IPDFtk pdftk, Stream pdfFile, int page, Stream replacementPdfFile)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);
            using var replacementFile = await TempPDFtkFile.FromAsync(replacementPdfFile);

            return await pdftk.ReplacePage(inputFile.TempFileName, page, replacementFile.TempFileName);
        }
        
        /// <summary>
        /// Replaces a range of pages in a PDF with another PDF
        /// </summary>
        /// <param name="pdftk">The IPDFtk object.</param>
        /// <param name="pdfFile">A stream of the PDF file input.</param>
        /// <param name="startPage">The page to replace</param>
        /// <param name="endPage">The page to replace</param>
        /// <param name="replacementPdfFile">A stream of the PDF file to replace the page with.</param>
        /// <returns>A result with the PDF form filled as a byte array.</returns>
        public static async Task<IPDFtkResult<byte[]>> ReplacePage(this IPDFtk pdftk, Stream pdfFile, int startPage, int endPage, Stream replacementPdfFile)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);
            using var replacementFile = await TempPDFtkFile.FromAsync(replacementPdfFile);

            return await pdftk.ReplacePages(inputFile.TempFileName, startPage, endPage, replacementFile.TempFileName);
        }

        /// <summary>
        /// Extracts attachments from a PDF file.
        /// </summary>
        /// <param name="pdftk">The IPDFtk object.</param>
        /// <param name="pdfFile">A stream of the PDF file input.</param>
        /// <returns>A result with the attachments.</returns>
        public static async Task<IPDFtkResult<IEnumerable<KeyValuePair<string, byte[]>>>> ExtractAttachments(this IPDFtk pdftk, Stream pdfFile)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);

            return await pdftk.ExtractAttachments(inputFile.TempFileName);
        }
    }
}