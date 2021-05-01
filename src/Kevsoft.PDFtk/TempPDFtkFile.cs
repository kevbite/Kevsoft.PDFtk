using System;
using System.IO;

namespace Kevsoft.PDFtk
{
    public sealed class TempPDFtkFile : IDisposable
    {
        public string TempFileName { get; }

        public TempPDFtkFile()
        {
            TempFileName = Path.GetTempFileName();
        }

        public void Dispose()
        {
            File.Delete(TempFileName);
        }
    }

    public sealed class TempPDFtkDirectory : IDisposable
    {
        private readonly DirectoryInfo _directoryInfo;
        public string TempDirectoryFullName => _directoryInfo.FullName;

        public TempPDFtkDirectory()
        {
            _directoryInfo = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
        }

        public void Dispose()
        {
            _directoryInfo.Delete(true);
        }
    }
}