using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kevsoft.PDFtk.Tests
{
    public class ConcatenatingFilesTests
    {
        private readonly PDFtk _pdFtk = new();

        [Fact]
        public async Task ShouldReturnPdfWithCorrectPages_ForInputFilesAsBytes()
        {
            var pdfFile1Bytes = await File.ReadAllBytesAsync("TestFiles/TestFileWith2Pages.pdf");
            var pdfFile2Bytes = await File.ReadAllBytesAsync("TestFiles/TestFileWith3Pages.pdf");
            var result = await _pdFtk.Concat(new[] {pdfFile1Bytes, pdfFile2Bytes});

            result.Success.Should().BeTrue();
            (await _pdFtk.GetNumberOfPages(result.Result))
                .Result.Should().Be(5);
        }
        
        [Fact]
        public async Task ShouldReturnPdfWithCorrectPages_ForInputFilesAsStreams()
        {
            await using var pdfFile1Stream = File.OpenRead("TestFiles/TestFileWith2Pages.pdf");
            await using var pdfFile2Stream = File.OpenRead("TestFiles/TestFileWith3Pages.pdf");
            var result = await _pdFtk.Concat(new[] {pdfFile1Stream, pdfFile2Stream});

            result.Success.Should().BeTrue();
            (await _pdFtk.GetNumberOfPages(result.Result))
                .Result.Should().Be(5);
        }
        
        [Fact]
        public async Task ShouldReturnPdfWithCorrectPages_ForInputFilesAsFilePaths()
        {
            var filePaths = new[] {"TestFiles/TestFileWith2Pages.pdf", "TestFiles/TestFileWith3Pages.pdf"};
            var result = await _pdFtk.Concat(filePaths);

            result.Success.Should().BeTrue();
            (await _pdFtk.GetNumberOfPages(result.Result))
                .Result.Should().Be(5);
        }
        
        [Fact]
        public async Task ShouldReturnUnsuccessfulAndEmptyResult_ForInvalidPdfFiles()
        {
            var result = await _pdFtk.Concat(new[]
            {
                Guid.NewGuid().ToByteArray(),
                Guid.NewGuid().ToByteArray()
            });
            
            result.Success.Should().BeFalse();
            result.Result.Should().BeEmpty();
        }
    }
}