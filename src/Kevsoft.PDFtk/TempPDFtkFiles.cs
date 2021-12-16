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
            foreach (var kvp in attachments)
            {
                var fileName = kvp.Key;
                var content = kvp.Value;

#if NETSTANDARD2_0
                File.WriteAllBytes(Path.Combine(tempPdFtkFiles._directory.TempDirectoryFullName, fileName),
                    content);
                await Task.CompletedTask;
#else
                   await File.WriteAllBytesAsync(Path.Combine(tempPdFtkFiles._directory.TempDirectoryFullName, fileName),
                    content);
#endif
            }

            return tempPdFtkFiles;
        }

        public static async Task<TempPDFtkFiles> FromAsync(IEnumerable<KeyValuePair<string, Stream>> attachments)
        {
            var tempPdFtkFiles = new TempPDFtkFiles();
            foreach (var kvp in attachments)
            {
                var fileName = kvp.Key;
                var stream = kvp.Value;

#if NETSTANDARD2_0
                using var openWrite =
                    File.OpenWrite(Path.Combine(tempPdFtkFiles._directory.TempDirectoryFullName, fileName));

#else
                    await using var openWrite =
                    File.OpenWrite(Path.Combine(tempPdFtkFiles._directory.TempDirectoryFullName, fileName));
#endif

                await stream.CopyToAsync(openWrite);
            }

            return tempPdFtkFiles;
        }
    }
}