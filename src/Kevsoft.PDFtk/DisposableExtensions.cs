using System;
using System.Collections.Generic;

namespace Kevsoft.PDFtk
{
    internal static class DisposableExtensions
    {
        public static void Dispose(this IEnumerable<IDisposable> collection)
        {
            foreach (IDisposable item in collection)
            {
                if (item != null)
                {
                    try
                    {
                        item.Dispose();
                    }
                    catch (Exception)
                    {
                        // log exception and continue
                    }
                }
            }
        }
    }
}