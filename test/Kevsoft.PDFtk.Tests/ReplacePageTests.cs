using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kevsoft.PDFtk.Tests
{
    public class ReplacePageTests
    {
        private readonly PDFtk _pdFtk = new();

        private const int FirstPage = 1;
        private const int LastPage = 10;
        private const int MiddlePage = 5;

        [Theory]
        [InlineData(FirstPage)]
        [InlineData(LastPage)]
        [InlineData(MiddlePage)]
        public async Task ShouldReturnPdfWithReplacedPage_ForInputFilesAsBytes(int page)
        {
            var fileBytes = await File.ReadAllBytesAsync(TestFiles.TestFile1Path);
            var replacementPdfBytes = await File.ReadAllBytesAsync(TestFiles.TestFileWith2PagesPath);

            var result = await _pdFtk.ReplacePage(fileBytes, page, replacementPdfBytes);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();

            (await _pdFtk.GetNumberOfPagesAsync(result.Result)).Result.Should().Be(11);
        }

        [Theory]
        [InlineData(FirstPage)]
        [InlineData(LastPage)]
        [InlineData(MiddlePage)]
        public async Task ShouldReturnPdfWithReplacedPage_ForInputFilesAsStreams(int page)
        {
            await using var inputFileStream = File.OpenRead(TestFiles.TestFile1Path);
            await using var stampFileStream = File.OpenRead(TestFiles.TestFileWith2PagesPath);

            var result = await _pdFtk.ReplacePage(inputFileStream, page, stampFileStream);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();
            (await _pdFtk.GetNumberOfPagesAsync(result.Result)).Result.Should().Be(11);
        }

        [Theory]
        [InlineData(FirstPage)]
        [InlineData(LastPage)]
        [InlineData(MiddlePage)]
        public async Task ShouldReturnPdfWithReplacedPage_ForInputFilesAsFilePaths(int page)
        {
            var result = await _pdFtk.ReplacePage(TestFiles.TestFile1Path, page, TestFiles.TestFileWith2PagesPath);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();
            (await _pdFtk.GetNumberOfPagesAsync(result.Result)).Result.Should().Be(11);
        }

        [Fact]
        public async Task ShouldReturnPdfWithReplacedPage_ForInvalidPdfFile()
        {
            var fileBytes = Guid.NewGuid().ToByteArray();
            var replacementFileBytes = await File.ReadAllBytesAsync(TestFiles.StampFilePath);

            var result = await _pdFtk.ReplacePage(fileBytes, 1, replacementFileBytes);

            result.Success.Should().BeFalse();
            result.Result.Should().BeEmpty();
        }

        [Fact]
        public async Task ShouldReturnPdfWithReplacedPage_ForReplacementPdfFile()
        {
            var fileBytes = await File.ReadAllBytesAsync(TestFiles.TestFile1Path);
            var replacementFileBytes = Guid.NewGuid().ToByteArray();

            var result = await _pdFtk.ReplacePage(fileBytes, 1, replacementFileBytes);

            result.Success.Should().BeFalse();
            result.Result.Should().BeEmpty();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(11)]
        public async Task ShouldThrowExceptionWhenPageIsOutOfBounds(int page)
        {
            var fileBytes = await File.ReadAllBytesAsync(TestFiles.TestFile1Path);
            var replacementPdfBytes = await File.ReadAllBytesAsync(TestFiles.TestFileWith2PagesPath);

            Func<Task> act = (async () => await _pdFtk.ReplacePage(fileBytes, page, replacementPdfBytes));

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Theory]
        [InlineData(FirstPage, 3)]
        [InlineData(8, LastPage)]
        [InlineData(4, 6)]
        [InlineData(4, 8)]
        public async Task ShouldReturnPdfWithReplacedRangeOfPages(int start, int end)
        {
            const int totalPagesInserted = 2;
            
            var result = await _pdFtk.ReplaceRangeOfPages(TestFiles.TestFile1Path, start,end, TestFiles.TestFileWith2PagesPath);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();
            (await _pdFtk.GetNumberOfPagesAsync(result.Result)).Result.Should().Be(LastPage - (end - start + 1) + totalPagesInserted);
        }
    }
}