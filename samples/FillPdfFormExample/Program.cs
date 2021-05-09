using System;
using System.IO;
using System.Threading.Tasks;

namespace FillPdfFormExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var pdfFile = await File.ReadAllBytesAsync("Form.pdf")
        }
    }
}