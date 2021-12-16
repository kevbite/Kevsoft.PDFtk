using System;
using System.IO;
using System.Threading.Tasks;

namespace Kevsoft.PDFtk
{
    internal sealed class TempPDFtkFile : IDisposable
    {
        public string TempFileName { get; }

        private TempPDFtkFile()
        {
            TempFileName = Path.GetTempFileName();
        }

        public static async Task<TempPDFtkFile> FromAsync(byte[] pdfFileBytes)
        {
            var tempPdFtkFile = new TempPDFtkFile();
            if (pdfFileBytes is not null)
            {
#if NETSTANDARD2_0
                File.WriteAllBytes(tempPdFtkFile.TempFileName, pdfFileBytes);
                await Task.CompletedTask;
#else
                await File.WriteAllBytesAsync(tempPdFtkFile.TempFileName, pdfFileBytes);
#endif
            }

            return tempPdFtkFile;
        }

        public static TempPDFtkFile Create()
        {
            return new();
        }

        public static async Task<TempPDFtkFile> FromAsync(Stream stream)
        {
            var tempPdFtkFile = new TempPDFtkFile();

#if NETSTANDARD2_0
            using var openWrite = File.OpenWrite(tempPdFtkFile.TempFileName);
#else
            await using var openWrite = File.OpenWrite(tempPdFtkFile.TempFileName);
#endif
            await stream.CopyToAsync(openWrite);

            return tempPdFtkFile;
        }

        public void Dispose()
        {
            File.Delete(TempFileName);
        }
    }
}