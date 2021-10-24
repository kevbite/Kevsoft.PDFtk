using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Kevsoft.PDFtk
{
    internal sealed class TempPDFtkFiles : IDisposable
    {
        private readonly TempPDFtkDirectory _directory;

        private TempPDFtkFiles()
        {
            _directory = TempPDFtkDirectory.Create();
        }

        public IEnumerable<string> FileNames => Directory.EnumerateFiles(_directory.TempDirectoryFullName);

        public void Dispose()
        {
            _directory.Dispose();
        }

        public static async Task<TempPDFtkFiles> FromAsync(IEnumerable<KeyValuePair<string, byte[]>> attachments)
        {
            var tempPdFtkFiles = new TempPDFtkFiles();
            foreach (var (fileName, content) in attachments)
            {
                await File.WriteAllBytesAsync(Path.Combine(tempPdFtkFiles._directory.TempDirectoryFullName, fileName),
                    content);
            }

            return tempPdFtkFiles;
        }

        public static async Task<TempPDFtkFiles> FromAsync(IEnumerable<KeyValuePair<string, Stream>> attachments)
        {
            var tempPdFtkFiles = new TempPDFtkFiles();
            foreach (var (fileName, stream) in attachments)
            {
                await using var openWrite =
                    File.OpenWrite(Path.Combine(tempPdFtkFiles._directory.TempDirectoryFullName, fileName));

                await stream.CopyToAsync(openWrite);
            }

            return tempPdFtkFiles;
        }
    }
}