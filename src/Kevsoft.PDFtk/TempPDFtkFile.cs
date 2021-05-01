using System;
using System.IO;
using System.Threading.Tasks;

namespace Kevsoft.PDFtk
{
    public sealed class TempPDFtkFile : IDisposable
    {
        public string TempFileName { get; }

        private TempPDFtkFile()
        {
            TempFileName = Path.GetTempFileName();
        }

        public static async Task<TempPDFtkFile> Create(byte[]? pdfFileBytes = null)
        {
            var inputFile = new TempPDFtkFile();
            if (pdfFileBytes is not null)
            {
                await File.WriteAllBytesAsync(inputFile.TempFileName, pdfFileBytes);
            }

            return inputFile;
        }

        public void Dispose()
        {
            File.Delete(TempFileName);
        }
    }
}