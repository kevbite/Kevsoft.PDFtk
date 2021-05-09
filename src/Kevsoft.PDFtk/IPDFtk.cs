using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Kevsoft.PDFtk
{
    public interface IPDFtk
    {
        Task<IPDFtkResult<int?>> GetNumberOfPagesAsync(byte[] pdfFileBytes);
        Task<IPDFtkResult<int?>> GetNumberOfPagesAsync(Stream pdfFileStream);
        Task<IPDFtkResult<int?>> GetNumberOfPagesAsync(string filePath);
        Task<IPDFtkResult<byte[]>> GetPagesAsync(byte[] pdfFileBytes, params int[] pages);
        Task<IPDFtkResult<byte[]>> GetPagesAsync(Stream pdfFileBytes, params int[] pages);
        Task<IPDFtkResult<byte[]>> GetPagesAsync(string inputFile, params int[] pages);
        Task<IPDFtkResult<IDataField[]>> GetDataFieldsAsync(byte[] pdfFileBytes);
        Task<IPDFtkResult<IDataField[]>> GetDataFieldsAsync(Stream pdfFile);
        Task<IPDFtkResult<IDataField[]>> GetDataFieldsAsync(string filePath);
        Task<IPDFtkResult<byte[]>> ConcatAsync(IEnumerable<byte[]> pdfFiles);
        Task<IPDFtkResult<byte[]>> ConcatAsync(IEnumerable<Stream> pdfStreams);
        Task<IPDFtkResult<byte[]>> ConcatAsync(IEnumerable<string> filePaths);
        Task<IPDFtkResult<IEnumerable<byte[]>>> SplitAsync(byte[] pdfFile);
        Task<IPDFtkResult<IEnumerable<byte[]>>> SplitAsync(Stream stream);
        Task<IPDFtkResult<IEnumerable<byte[]>>> SplitAsync(string filePath);
        Task<IPDFtkResult<byte[]>> StampAsync(byte[] pdfFile, byte[] stampPdfFile);
        Task<IPDFtkResult<byte[]>> StampAsync(Stream pdfFileStream, Stream stampPdfFileStream);
        Task<IPDFtkResult<byte[]>> StampAsync(string pdfFilePath, string stampPdfFilePath);

        Task<IPDFtkResult<byte[]>> FillFormAsync(byte[] pdfFile,
            IReadOnlyDictionary<string, string> fieldData,
            bool flatten,
            bool dropXfa);

        Task<IPDFtkResult<byte[]>> FillFormAsync(Stream stream,
            IReadOnlyDictionary<string, string> fieldData,
            bool flatten,
            bool dropXfa);

        Task<IPDFtkResult<byte[]>> FillFormAsync(string pdfFilePath,
            IReadOnlyDictionary<string, string> fieldData,
            bool flatten,
            bool dropXfa);
    }
}