using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Kevsoft.PDFtk
{
    internal sealed class PDFtkProcess
    {
        internal async Task<ExecutionResult> Execute(params string[] args)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "pdftk",
                    Arguments = string.Join(" ", args),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();

            var standardOutput = await GetAllStream(process.StandardOutput);
            var standardError = await GetAllStream(process.StandardError);

            await process.WaitForExitAsync();
            
            return new ExecutionResult(process.ExitCode, standardOutput, standardError);
        }

        private static async Task<string> GetAllStream(StreamReader stream)
        {
            var stringBuilder = new StringBuilder();
            while (!stream.EndOfStream)
            {
                stringBuilder.AppendLine(await stream.ReadLineAsync());
            }

            return stringBuilder.ToString();
        }
    }
}