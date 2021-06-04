using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Kevsoft.PDFtk
{
    internal sealed class PDFtkProcess
    {
        private readonly PDFtkOptions _options;

        public PDFtkProcess(PDFtkOptions options)
            => _options = options;

        internal async Task<ExecutionResult> ExecuteAsync(params string[] args)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _options.PDFtkPath,
                    Arguments = string.Join(" ", args),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();

            var standardOutput = await GetAllStreamAsync(process.StandardOutput);
            var standardError = await GetAllStreamAsync(process.StandardError);

#if NETSTANDARD2_1
            process.WaitForExit();
#else
            await process.WaitForExitAsync();
#endif
            
            return new ExecutionResult(process.ExitCode, standardOutput, standardError);
        }

        private static async Task<string> GetAllStreamAsync(StreamReader stream)
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