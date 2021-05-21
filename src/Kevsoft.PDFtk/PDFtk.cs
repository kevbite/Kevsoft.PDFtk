using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kevsoft.PDFtk
{
    /// <inheritdoc/>
    public sealed class PDFtk : IPDFtk
    {
        private readonly XfdfGenerator _xfdfGenerator;
        private readonly PDFtkProcess _pdftkProcess;

        /// <summary>
        /// Initializes a new instance of the PDFtk class.
        /// </summary>
        public PDFtk()
        {
            _xfdfGenerator = new XfdfGenerator();
            _pdftkProcess = new PDFtkProcess();
        }

        /// <inheritdoc/>
        public async Task<IPDFtkResult<int?>> GetNumberOfPagesAsync(byte[] pdfFile)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);

            return await GetNumberOfPagesAsync(inputFile.TempFileName);
        }

        /// <inheritdoc/>
        public async Task<IPDFtkResult<int?>> GetNumberOfPagesAsync(Stream pdfFile)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);

            return await GetNumberOfPagesAsync(inputFile.TempFileName);
        }

        /// <inheritdoc/>
        public async Task<IPDFtkResult<int?>> GetNumberOfPagesAsync(string filePath)
        {
            var executeProcessResult = await _pdftkProcess.ExecuteAsync(filePath, "dump_data");

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

        /// <inheritdoc/>
        public async Task<IPDFtkResult<byte[]>> GetPagesAsync(byte[] pdfFile, params int[] pages)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);
            
            return await GetPagesAsync(inputFile.TempFileName, pages);
        }
        
        
        /// <inheritdoc/>
        public async Task<IPDFtkResult<byte[]>> GetPagesAsync(Stream pdfFile, params int[] pages)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);
            
            return await GetPagesAsync(inputFile.TempFileName, pages);
        }

        /// <inheritdoc/>
        public async Task<IPDFtkResult<byte[]>> GetPagesAsync(string inputFile, params int[] pages)
        {
            using var outputFile = TempPDFtkFile.Create();

            var pageRanges = GetPageRangeArgs(pages);

            var executeProcessResult = await _pdftkProcess.ExecuteAsync(inputFile, "cat",
                string.Join(" ", pageRanges),
                "output", outputFile.TempFileName);

            return await ResolveSingleFileExecutionResultAsync(executeProcessResult, outputFile);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public async Task<IPDFtkResult<IDataField[]>> GetDataFieldsAsync(byte[] pdfFile)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);

            return await GetDataFieldsAsync(inputFile.TempFileName);
        }
        
        /// <inheritdoc/>
        public async Task<IPDFtkResult<IDataField[]>> GetDataFieldsAsync(Stream pdfFile)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);

            return await GetDataFieldsAsync(inputFile.TempFileName);
        }

        /// <inheritdoc/>
        public async Task<IPDFtkResult<IDataField[]>> GetDataFieldsAsync(string filePath)
        {
            var executeProcessResult = await _pdftkProcess.ExecuteAsync(filePath, "dump_data_fields");

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

        /// <inheritdoc/>
        public async Task<IPDFtkResult<byte[]>> ConcatAsync(IEnumerable<byte[]> pdfFiles)
        {
            var inputFiles = await Task.WhenAll(
                pdfFiles.Select(async file => await TempPDFtkFile.FromAsync(file))
                    .ToList());
            
            try
            {
                return await ConcatAsync(inputFiles.Select(x => x.TempFileName));
            }
            finally
            {
                inputFiles.Dispose();
            }
        }
        
        /// <inheritdoc/>
        public async Task<IPDFtkResult<byte[]>> ConcatAsync(IEnumerable<Stream> pdfFiles)
        {
            var inputFiles = await Task.WhenAll(
                pdfFiles.Select(async file => await TempPDFtkFile.FromAsync(file))
                    .ToList());
            
            try
            {
                return await ConcatAsync(inputFiles.Select(x => x.TempFileName));
            }
            finally
            {
                inputFiles.Dispose();
            }
        }

        /// <inheritdoc/>
        public async Task<IPDFtkResult<byte[]>> ConcatAsync(IEnumerable<string> filePaths)
        {
            using var outputFile = TempPDFtkFile.Create();

            var inputFileNames = string.Join(" ", filePaths);

            var executeProcessResult =
                await _pdftkProcess.ExecuteAsync(inputFileNames, "cat", "output", outputFile.TempFileName);

            return await ResolveSingleFileExecutionResultAsync(executeProcessResult, outputFile);
        }

        /// <inheritdoc/>
        public async Task<IPDFtkResult<IEnumerable<byte[]>>> SplitAsync(byte[] pdfFile)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);

            return await SplitAsync(inputFile.TempFileName);
        }
        
        /// <inheritdoc/>
        public async Task<IPDFtkResult<IEnumerable<byte[]>>> SplitAsync(Stream pdfFile)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);

            return await SplitAsync(inputFile.TempFileName);
        }

        /// <inheritdoc/>
        public async Task<IPDFtkResult<IEnumerable<byte[]>>> SplitAsync(string filePath)
        {
            using var outputDirectory = TempPDFtkDirectory.Create();

            var outputFilePattern = Path.Combine(outputDirectory.TempDirectoryFullName, "page_%02d.pdf");
            var executeProcessResult =
                await _pdftkProcess.ExecuteAsync(filePath, "burst", "output", outputFilePattern);

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

        /// <inheritdoc/>
        public async Task<IPDFtkResult<byte[]>> StampAsync(byte[] pdfFile, byte[] stampPdfFile)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);
            using var stampFile = await TempPDFtkFile.FromAsync(stampPdfFile);

            return await StampAsync(inputFile.TempFileName, stampFile.TempFileName);
        }
        
        /// <inheritdoc/>
        public async Task<IPDFtkResult<byte[]>> StampAsync(Stream pdfFile, Stream stampPdfFile)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);
            using var stampFile = await TempPDFtkFile.FromAsync(stampPdfFile);

            return await StampAsync(inputFile.TempFileName, stampFile.TempFileName);
        }

        /// <inheritdoc/>
        public async Task<IPDFtkResult<byte[]>> StampAsync(string pdfFilePath, string stampPdfFilePath)
        {
            using var outputFile = TempPDFtkFile.Create();


            var executeProcessResult = await _pdftkProcess.ExecuteAsync(pdfFilePath,
                "multistamp", stampPdfFilePath,
                "output", outputFile.TempFileName);

            return await ResolveSingleFileExecutionResultAsync(executeProcessResult, outputFile);
        }

        /// <inheritdoc/>
        public async Task<IPDFtkResult<byte[]>> FillFormAsync(byte[] pdfFile,
            IReadOnlyDictionary<string, string> fieldData,
            bool flatten,
            bool dropXfa)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);

            return await FillFormAsync(inputFile.TempFileName,
                fieldData,
                flatten,
                dropXfa);
        }
        
        /// <inheritdoc/>
        public async Task<IPDFtkResult<byte[]>> FillFormAsync(Stream pdfFile,
            IReadOnlyDictionary<string, string> fieldData,
            bool flatten,
            bool dropXfa)
        {
            using var inputFile = await TempPDFtkFile.FromAsync(pdfFile);

            return await FillFormAsync(inputFile.TempFileName,
                fieldData,
                flatten,
                dropXfa);
        }

        /// <inheritdoc/>
        public async Task<IPDFtkResult<byte[]>> FillFormAsync(string pdfFilePath,
            IReadOnlyDictionary<string, string> fieldData,
            bool flatten,
            bool dropXfa)
        {
            using var outputFile = TempPDFtkFile.Create();
            using var xfdfFile = await _xfdfGenerator.CreateXfdfFile(fieldData);

            var args = new List<string>(new[]
            {
                pdfFilePath,
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

            var executeProcessResult = await _pdftkProcess.ExecuteAsync(args.ToArray());

            return await ResolveSingleFileExecutionResultAsync(executeProcessResult, outputFile);
        }

        private static async Task<IPDFtkResult<byte[]>> ResolveSingleFileExecutionResultAsync(
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