using System;

namespace Kevsoft.PDFtk
{
    /// <summary>
    /// Exception raised when the pdftk executable could not be found.
    /// </summary>
    [Serializable]
    public class PDFtkFileNotFoundExceptionException : Exception
    {
        internal PDFtkFileNotFoundExceptionException(string[] searchedPaths)
            : base(CreateMessage(searchedPaths))
        {
        }

        private static string CreateMessage(string[] searchedPaths)
        {
            var message = "Could not find the pdftk executable, we tried the following paths:";
            message += Environment.NewLine;
            message += string.Join(Environment.NewLine, searchedPaths);
            
            return message;
        }
    }
}