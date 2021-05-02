using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kevsoft.PDFtk.Tests
{
    public class GetPagesTests
    {
        private readonly PDFtk _pdFtk = new();

        [Fact]
        public async Task ShouldReturnPagesFromAnotherPdf_ForInputFileAsBytes()
        {
            var pdfFileBytes = await File.ReadAllBytesAsync("TestFiles/TestFile1.pdf");
            var result = await _pdFtk.GetPages(pdfFileBytes, 1,5,6,7,10);
                
            result.Success.Should().BeTrue();
            (await _pdFtk.GetNumberOfPages(result.Result))
                .Result.Should().Be(5);
        }
        
        [Fact]
        public async Task ShouldReturnPagesFromAnotherPdf_ForInputFileAsStream()
        {
            await using var stream = File.OpenRead("TestFiles/TestFile1.pdf");
            var result = await _pdFtk.GetPages(stream, 1,5,6,7,10);
                
            result.Success.Should().BeTrue();
            (await _pdFtk.GetNumberOfPages(result.Result))
                .Result.Should().Be(5);
        }
        
        [Fact]
        public async Task ShouldReturnPagesFromAnotherPdf_ForInputFileFilePath()
        {
            var result = await _pdFtk.GetPages("TestFiles/TestFile1.pdf", 1,5,6,7,10);
                
            result.Success.Should().BeTrue();
            (await _pdFtk.GetNumberOfPages(result.Result))
                .Result.Should().Be(5);
        }
        
        [Fact]
        public async Task ShouldReturnUnsuccessfulAndEmptyResult_ForInvalidPageNumbers()
        {
            var pdfFileBytes = await File.ReadAllBytesAsync("TestFiles/TestFile1.pdf");
            var result = await _pdFtk.GetPages(pdfFileBytes, 15, 16);
            
            result.Success.Should().BeFalse();
            result.Result.Should().BeEmpty();
        }
        
        [Fact]
        public async Task ShouldReturnUnsuccessfulAndEmptyResult_ForInvalidPdfFile()
        {
            var result = await _pdFtk.GetPages(Guid.NewGuid().ToByteArray(), 1,5,6,7,10);
        
            result.Success.Should().BeFalse();
            result.Result.Should().BeEmpty();
        }
    }
}
