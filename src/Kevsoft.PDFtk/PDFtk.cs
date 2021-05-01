using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Kevsoft.PDFtk
{
    public sealed class PDFtk
    {
        public async Task<IPDFtkResult<int?>> GetNumberOfPages(byte[] pdfFileBytes)
        {
            using var inputFile = await CreateTempPdFtkFile(pdfFileBytes);

            var executeProcessResult = await ExecuteProcess(inputFile.TempFileName, "dump_data");

            int? pages = null;

            if (executeProcessResult.ExitCode == 0)
            {
                var key = "NumberOfPages: ";
                var line = executeProcessResult.StandardOutput
                    .Split(Environment.NewLine)
                    .Single(x => x.StartsWith(key));

                pages = int.Parse(line.Substring(key.Length));
            }

            return new PDFtkResult<int?>(executeProcessResult, pages);
        }

        internal class ExecuteProcessResult
        {
            public ExecuteProcessResult(int exitCode, string standardOutput, string standardError)
            {
                ExitCode = exitCode;
                StandardOutput = standardOutput;
                StandardError = standardError;
            }

            public string StandardOutput { get; }
            public string StandardError { get; }
            public int ExitCode { get; }

            public bool Success => ExitCode == 0;
        }

        private static async Task<ExecuteProcessResult> ExecuteProcess(params string[] args)
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

            var stringBuilder = new StringBuilder();
            while (!process.StandardOutput.EndOfStream)
            {
                stringBuilder.AppendLine(await process.StandardOutput.ReadLineAsync());
            }

            var standardErrorstringBuilder = new StringBuilder();
            while (!process.StandardError.EndOfStream)
            {
                standardErrorstringBuilder.AppendLine(await process.StandardError.ReadLineAsync());
            }

            await process.WaitForExitAsync();


            return new ExecuteProcessResult(process.ExitCode, stringBuilder.ToString(),
                standardErrorstringBuilder.ToString());
        }

        private static async Task<TempPDFtkFile> CreateTempPdFtkFile(byte[]? pdfFileBytes = null)
        {
            var inputFile = new TempPDFtkFile();
            if (pdfFileBytes is not null)
            {
                await File.WriteAllBytesAsync(inputFile.TempFileName, pdfFileBytes);
            }

            return inputFile;
        }
        
        public async Task<IPDFtkResult<byte[]>> GetPages(byte[] pdfFileBytes, params int[] pages)
        {
            using var inputFile = await CreateTempPdFtkFile(pdfFileBytes);
            using var outputFile = await CreateTempPdFtkFile();

            var pageRanges = GetPageRangeArgs(pages);

            var executeProcessResult = await ExecuteProcess(inputFile.TempFileName, "cat", string.Join(" ", pageRanges),
                "output", outputFile.TempFileName);

            var bytes = new byte[0];
            if (executeProcessResult.Success)
            {
                bytes = await File.ReadAllBytesAsync(outputFile.TempFileName);
            }

            return new PDFtkResult<byte[]>(executeProcessResult, bytes);
        }

        private static IEnumerable<string> GetPageRangeArgs(int[] pages)
        {
            var runStart = -1;
            var runEnd = -1;
            
            string RangeString() 
                => runStart != runEnd ? $"{runStart}-{runEnd}" : $"{runStart}";
            
            foreach (var page in pages)
            {
                if (runStart == -1)
                {
                    runStart = page;
                    runEnd = page;
                }
                else if(page == runEnd +1)
                {
                    runEnd = page;
                }
                else
                {
                    yield return RangeString();

                    runStart = page;
                    runEnd = page;
                }
            }
            yield return RangeString();
        }

        public async Task<IPDFtkResult<IDataField[]>> DumpDataFields(byte[] pdfFileBytes)
        {
            using var inputFile = await CreateTempPdFtkFile(pdfFileBytes);
            using var outputFile = await CreateTempPdFtkFile(pdfFileBytes);

            var executeProcessResult = await ExecuteProcess(inputFile.TempFileName, "dump_data_fields");

            var dataFields = Array.Empty<DataField>();
            if (executeProcessResult.Success)
            {
                dataFields = executeProcessResult.StandardOutput
                    .Split("---" + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
                    .Select(DataField.Parse)
                    .ToArray();
            }

            return new PDFtkResult<DataField[]>(executeProcessResult, dataFields);
        }
        
        public async Task<IPDFtkResult<byte[]>> Concat(IEnumerable<byte[]> files)
        {
            var inputFiles = await Task.WhenAll(
                files.Select(async file => await CreateTempPdFtkFile(file))
                    .ToList());

            using var outputFile = await CreateTempPdFtkFile();

            var inputFileNames = string.Join(" ", inputFiles.Select(x => x.TempFileName));

            var executeProcessResult = await ExecuteProcess(inputFileNames, "cat", "output", outputFile.TempFileName);

            var bytes = Array.Empty<byte>();
            if (executeProcessResult.Success)
            {
                bytes = await File.ReadAllBytesAsync(outputFile.TempFileName);
            }

            return new PDFtkResult<byte[]>(executeProcessResult, bytes);
        }
    }
}