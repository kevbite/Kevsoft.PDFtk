using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kevsoft.PDFtk.Tests
{
    public class ConcatenatingFilesTests
    {
        [Fact]
        public async Task ShouldReturnPdfWithCorrectPages()
        {
            var pdFtk = new PDFtk();

            var pdfFile1Bytes = await File.ReadAllBytesAsync("TestFiles/TestFileWith2Pages.pdf");
            var pdfFile2Bytes = await File.ReadAllBytesAsync("TestFiles/TestFileWith3Pages.pdf");
            var result = await pdFtk.Concat(new[] {pdfFile1Bytes, pdfFile2Bytes});

            result.Success.Should().BeTrue();
            (await pdFtk.GetNumberOfPages(result.Result))
                .Result.Should().Be(5);
        }
        
        [Fact]
        public async Task ShouldReturnUnsuccessfulAndEmptyResult_ForInvalidPdfFiles()
        {
            var pdFtk = new PDFtk();

            var result = await pdFtk.Concat(new[]
            {
                Guid.NewGuid().ToByteArray(),
                Guid.NewGuid().ToByteArray()
            });
            
            result.Success.Should().BeFalse();
            result.Result.Should().BeEmpty();
        }
    }
}