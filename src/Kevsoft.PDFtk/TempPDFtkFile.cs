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

        public static async Task<TempPDFtkFile> FromBytes(byte[] pdfFileBytes)
        {
            var tempPdFtkFile = new TempPDFtkFile();
            if (pdfFileBytes is not null)
            {
                await File.WriteAllBytesAsync(tempPdFtkFile.TempFileName, pdfFileBytes);
            }

            return tempPdFtkFile;
        }
        
        public static TempPDFtkFile Create()
        {
            return new();
        }

        public static async Task<TempPDFtkFile> FromStream(Stream stream)
        {
            var tempPdFtkFile = new TempPDFtkFile();

            await using var openWrite = File.OpenWrite(tempPdFtkFile.TempFileName);

            await stream.CopyToAsync(openWrite);

            return tempPdFtkFile;
        }

        public void Dispose()
        {
            File.Delete(TempFileName);
        }
    }
}