using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kevsoft.PDFtk
{
    public sealed class PDFtk
    {
        private readonly XfdfGenerator _xfdfGenerator;
        private readonly PDFtkProcess _pdftkProcess;

        public PDFtk()
        {
            _xfdfGenerator = new XfdfGenerator();
            _pdftkProcess = new PDFtkProcess();
        }

        public async Task<IPDFtkResult<int?>> GetNumberOfPages(byte[] pdfFileBytes)
        {
            using var inputFile = await CreateTempPdFtkFile(pdfFileBytes);

            var executeProcessResult = await _pdftkProcess.Execute(inputFile.TempFileName, "dump_data");

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

            var executeProcessResult = await _pdftkProcess.Execute(inputFile.TempFileName, "cat",
                string.Join(" ", pageRanges),
                "output", outputFile.TempFileName);

            return await ResolveSingleFileExecutionResult(executeProcessResult, outputFile);
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
                else if (page == runEnd + 1)
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

            var executeProcessResult = await _pdftkProcess.Execute(inputFile.TempFileName, "dump_data_fields");

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

            try
            {
                var executeProcessResult =
                    await _pdftkProcess.Execute(inputFileNames, "cat", "output", outputFile.TempFileName);

                return await ResolveSingleFileExecutionResult(executeProcessResult, outputFile);
            }
            finally
            {
                inputFiles.Dispose();
            }
        }

        public async Task<IPDFtkResult<IEnumerable<byte[]>>> Split(byte[] file)
        {
            using var inputFile = await CreateTempPdFtkFile(file);

            using var outputDirectory = new TempPDFtkDirectory();

            var outputFilePattern = Path.Combine(outputDirectory.TempDirectoryFullName, "page_%02d.pdf");
            var executeProcessResult =
                await _pdftkProcess.Execute(inputFile.TempFileName, "burst", "output", outputFilePattern);

            var outputFileBytes = new List<byte[]>();
            if (executeProcessResult.Success)
            {
                var outputFiles = Directory.GetFiles(outputDirectory.TempDirectoryFullName, "*.pdf");
                foreach (var outputFile in outputFiles)
                {
                    var bytes = await File.ReadAllBytesAsync(outputFile);
                    outputFileBytes.Add(bytes);
                }
            }

            return new PDFtkResult<IEnumerable<byte[]>>(executeProcessResult, outputFileBytes);
        }

        public async Task<IPDFtkResult<byte[]>> Stamp(byte[] pdfFile, byte[] stampPdfFile)
        {
            using var inputFile = await CreateTempPdFtkFile(pdfFile);
            using var stampFile = await CreateTempPdFtkFile(stampPdfFile);

            using var outputFile = await CreateTempPdFtkFile();


            var executeProcessResult = await _pdftkProcess.Execute(inputFile.TempFileName,
                "multistamp", stampFile.TempFileName,
                "output", outputFile.TempFileName);

            return await ResolveSingleFileExecutionResult(executeProcessResult, outputFile);
        }

        public async Task<IPDFtkResult<byte[]>> FillForm(byte[] pdfFile,
            IReadOnlyDictionary<string, string> fieldData,
            bool flatten,
            bool dropXfa)
        {
            using var inputFile = await CreateTempPdFtkFile(pdfFile);
            using var outputFile = await CreateTempPdFtkFile();
            using var xfdfFile = await _xfdfGenerator.CreateXfdfFile(fieldData);

            var args = new List<string>(new[]
            {
                inputFile.TempFileName,
                "fill_form",
                xfdfFile.TempFileName,
                "output",
                outputFile.TempFileName
            });
            if (flatten)
            {
                args.Add("flatten");
            }

            if (dropXfa)
            {
                args.Add("drop_xfa");
            }

            var executeProcessResult = await _pdftkProcess.Execute(args.ToArray());

            return await ResolveSingleFileExecutionResult(executeProcessResult, outputFile);
        }
        
        private static async Task<IPDFtkResult<byte[]>> ResolveSingleFileExecutionResult(ExecutionResult executeProcessResult,
            TempPDFtkFile outputFile)
        {
            var bytes = Array.Empty<byte>();
            if (executeProcessResult.Success)
            {
                bytes = await File.ReadAllBytesAsync(outputFile.TempFileName);
            }

            return new PDFtkResult<byte[]>(executeProcessResult, bytes);
        }
    }
}