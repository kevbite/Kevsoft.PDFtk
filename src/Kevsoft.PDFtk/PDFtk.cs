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
            : this(PDFtkOptions.Default())
        {
        }

        /// <inheritdoc cref="PDFtk()"/>
        /// <param name="options">The options to use.</param>
        public PDFtk(PDFtkOptions options)
        {
            _xfdfGenerator = new XfdfGenerator();
            _pdftkProcess = new PDFtkProcess(options);
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
        public async Task<IPDFtkResult<byte[]>> ConcatAsync(IEnumerable<string> filePaths)
        {
            using var outputFile = TempPDFtkFile.Create();

            var inputFileNames = string.Join(" ", filePaths);

            var executeProcessResult =
                await _pdftkProcess.ExecuteAsync(inputFileNames, "cat", "output", outputFile.TempFileName);

            return await ResolveSingleFileExecutionResultAsync(executeProcessResult, outputFile);
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
        public async Task<IPDFtkResult<byte[]>> StampAsync(string pdfFilePath, string stampPdfFilePath)
        {
            using var outputFile = TempPDFtkFile.Create();


            var executeProcessResult = await _pdftkProcess.ExecuteAsync(pdfFilePath,
                "multistamp", stampPdfFilePath,
                "output", outputFile.TempFileName);

            return await ResolveSingleFileExecutionResultAsync(executeProcessResult, outputFile);
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


        /// <inheritdoc/>
        public async Task<IPDFtkResult<byte[]>> ReplacePage(string pdfFilePath, int page, string replacementFilePath)
        {
            return await ReplaceRangeOfPages(pdfFilePath, page, page, replacementFilePath);
        }

        /// <inheritdoc/>
        public async Task<IPDFtkResult<byte[]>> ReplaceRangeOfPages(string pdfFilePath, int start, int end, string replacementFilepath)
        {
            var range = new Range(start, end);
            var numberOfPagesAsync = await GetNumberOfPagesAsync(pdfFilePath);
            if (!numberOfPagesAsync.Success)
                return new PDFtkResult<byte[]>(numberOfPagesAsync.ExecutionResult, Array.Empty<byte>());

            var totalPages = numberOfPagesAsync.Result;
            if (!range.IsValid || !range.IsInBounds(totalPages))
                throw new ArgumentException($"Invalid range of pages to replace, min page is 1 and maximum is {totalPages}");

            var bounds = (firstPage: range.HasFirst(), lastPage: range.HasLast(totalPages)) switch
            {
                (firstPage: true, lastPage: false) => new[] { "B", $"A{range.End + 1}-end" },
                (firstPage: false, lastPage: true) => new[] { $"A1-{range.Start - 1}", "B" },
                _ => new[] { $"A1-{range.Start - 1}", "B", $"A{range.End + 1}-end" },
            };

            using var outputFile = TempPDFtkFile.Create();
            var args = new List<string>(8)
            {
                $"A={pdfFilePath}",
                $"B={replacementFilepath}",
                "cat"
            };
            args.AddRange(bounds);
            args.Add("output");
            args.Add(outputFile.TempFileName);
            var executeProcessResult = await _pdftkProcess.ExecuteAsync(args.ToArray());

            return await ResolveSingleFileExecutionResultAsync(executeProcessResult, outputFile);
        }

        private class Range
        {
            public int Start { get; }
            public int End { get; }
            public bool IsValid => Start <= End && Start > 0 && End > 0;
            public Range(int start, int end) => (Start, End) = (start, end);
            public bool IsInBounds(int? upper) => IsInBounds(1, upper);
            public bool IsInBounds(int? lower, int? upper) => Start >= lower && End <= upper;
            public bool HasFirst() => HasFirst(1);
            public bool HasFirst(int? first) => Start == first;
            public bool HasLast(int? last) => End == last;
        }
    }
}