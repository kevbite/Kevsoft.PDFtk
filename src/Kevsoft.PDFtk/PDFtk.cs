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
            using var inputFile = await TempPDFtkFile.FromBytes(pdfFileBytes);

            return await GetNumberOfPages(inputFile.TempFileName);
        }

        public async Task<IPDFtkResult<int?>> GetNumberOfPages(Stream pdfFileStream)
        {
            using var inputFile = await TempPDFtkFile.FromStream(pdfFileStream);

            return await GetNumberOfPages(inputFile.TempFileName);
        }

        public async Task<IPDFtkResult<int?>> GetNumberOfPages(string filePath)
        {
            var executeProcessResult = await _pdftkProcess.Execute(filePath, "dump_data");

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

        public async Task<IPDFtkResult<byte[]>> GetPages(byte[] pdfFileBytes, params int[] pages)
        {
            using var inputFile = await TempPDFtkFile.FromBytes(pdfFileBytes);
            
            return await GetPages(inputFile.TempFileName, pages);
        }
        
        
        public async Task<IPDFtkResult<byte[]>> GetPages(Stream pdfFileBytes, params int[] pages)
        {
            using var inputFile = await TempPDFtkFile.FromStream(pdfFileBytes);
            
            return await GetPages(inputFile.TempFileName, pages);
        }

        public async Task<IPDFtkResult<byte[]>> GetPages(string inputFile, params int[] pages)
        {
            using var outputFile = TempPDFtkFile.Create();

            var pageRanges = GetPageRangeArgs(pages);

            var executeProcessResult = await _pdftkProcess.Execute(inputFile, "cat",
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
            using var inputFile = await TempPDFtkFile.FromBytes(pdfFileBytes);

            return await DumpDataFields(inputFile.TempFileName);
        }
        
        public async Task<IPDFtkResult<IDataField[]>> DumpDataFields(Stream pdfFile)
        {
            using var inputFile = await TempPDFtkFile.FromStream(pdfFile);

            return await DumpDataFields(inputFile.TempFileName);
        }

        public async Task<IPDFtkResult<IDataField[]>> DumpDataFields(string filePath)
        {
            var executeProcessResult = await _pdftkProcess.Execute(filePath, "dump_data_fields");

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
                files.Select(async file => await TempPDFtkFile.FromBytes(file))
                    .ToList());

            using var outputFile = TempPDFtkFile.Create();

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

        public async Task<IPDFtkResult<IEnumerable<byte[]>>> Split(byte[] pdfFile)
        {
            using var inputFile = await TempPDFtkFile.FromBytes(pdfFile);

            return await Split(inputFile.TempFileName);
        }
        
        public async Task<IPDFtkResult<IEnumerable<byte[]>>> Split(Stream stream)
        {
            using var inputFile = await TempPDFtkFile.FromStream(stream);

            return await Split(inputFile.TempFileName);
        }

        public async Task<IPDFtkResult<IEnumerable<byte[]>>> Split(string filePath)
        {
            using var outputDirectory = TempPDFtkDirectory.Create();

            var outputFilePattern = Path.Combine(outputDirectory.TempDirectoryFullName, "page_%02d.pdf");
            var executeProcessResult =
                await _pdftkProcess.Execute(filePath, "burst", "output", outputFilePattern);

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
            using var inputFile = await TempPDFtkFile.FromBytes(pdfFile);
            using var stampFile = await TempPDFtkFile.FromBytes(stampPdfFile);

            using var outputFile = TempPDFtkFile.Create();


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
            using var inputFile = await TempPDFtkFile.FromBytes(pdfFile);
            using var outputFile = TempPDFtkFile.Create();
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

        private static async Task<IPDFtkResult<byte[]>> ResolveSingleFileExecutionResult(
            ExecutionResult executeProcessResult,
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