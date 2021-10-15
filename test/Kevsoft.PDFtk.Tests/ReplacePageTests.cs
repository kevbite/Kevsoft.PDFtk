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
            var stampBytes = await File.ReadAllBytesAsync(TestFiles.StampFilePath);

            var result = await _pdFtk.ReplacePage(fileBytes, 1, stampBytes);

            result.Success.Should().BeFalse();
            result.Result.Should().BeEmpty();
        }

        [Fact]
        public async Task ShouldReturnPdfWithReplacedPage_ForReplacementPdfFile()
        {
            var fileBytes = await File.ReadAllBytesAsync(TestFiles.TestFile1Path);
            var stampBytes = Guid.NewGuid().ToByteArray();

            var result = await _pdFtk.ReplacePage(fileBytes, 1, stampBytes);

            result.Success.Should().BeFalse();
            result.Result.Should().BeEmpty();
        }
    }
}