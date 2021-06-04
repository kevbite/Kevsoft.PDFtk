using System;
using System.IO;

namespace Kevsoft.PDFtk
{
    internal static class PathEnvironmentVariable
    {
        public static string GetFileWithPath(string fileName)
        {
            var pathEnvironmentVariable = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            var paths = pathEnvironmentVariable.Split(Path.PathSeparator);
            foreach (var path in paths)
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }

            throw new PDFtkFileNotFoundExceptionException(paths);
        }
    }
}