using System;
using System.IO;

namespace Kevsoft.PDFtk
{
    public sealed class TempPDFtkDirectory : IDisposable
    {
        private readonly DirectoryInfo _directoryInfo;
        public string TempDirectoryFullName => _directoryInfo.FullName;

        private TempPDFtkDirectory()
        {
            _directoryInfo = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
        }

        public void Dispose()
        {
            _directoryInfo.Delete(true);
        }

        public static TempPDFtkDirectory Create()
        {
            return new TempPDFtkDirectory();
        }
    }
}