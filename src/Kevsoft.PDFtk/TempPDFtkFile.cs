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
}