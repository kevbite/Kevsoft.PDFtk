using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kevsoft.PDFtk.Tests
{
    public class GetPagesTests
    {
        [Fact]
        public async Task ShouldReturnPagesFromAnotherPdf_ForInputFileAsBytes()
        {
            var pdFtk = new PDFtk();

            var pdfFileBytes = await File.ReadAllBytesAsync("TestFiles/TestFile1.pdf");
            var result = await pdFtk.GetPages(pdfFileBytes, 1,5,6,7,10);
                
            result.Success.Should().BeTrue();
            (await pdFtk.GetNumberOfPages(result.Result))
                .Result.Should().Be(5);
        }
        
        [Fact]
        public async Task ShouldReturnPagesFromAnotherPdf_ForInputFileAsStream()
        {
            var pdFtk = new PDFtk();

            await using var stream = File.OpenRead("TestFiles/TestFile1.pdf");
            var result = await pdFtk.GetPages(stream, 1,5,6,7,10);
                
            result.Success.Should().BeTrue();
            (await pdFtk.GetNumberOfPages(result.Result))
                .Result.Should().Be(5);
        }
        
        [Fact]
        public async Task ShouldReturnPagesFromAnotherPdf_ForInputFileFilePath()
        {
            var pdFtk = new PDFtk();
;
            var result = await pdFtk.GetPages("TestFiles/TestFile1.pdf", 1,5,6,7,10);
                
            result.Success.Should().BeTrue();
            (await pdFtk.GetNumberOfPages(result.Result))
                .Result.Should().Be(5);
        }
        
        [Fact]
        public async Task ShouldReturnUnsuccessfulAndEmptyResult_ForInvalidPageNumbers()
        {
            var pdFtk = new PDFtk();

            var pdfFileBytes = await File.ReadAllBytesAsync("TestFiles/TestFile1.pdf");
            var result = await pdFtk.GetPages(pdfFileBytes, 15, 16);
            
            result.Success.Should().BeFalse();
            result.Result.Should().BeEmpty();
        }
        
        [Fact]
        public async Task ShouldReturnUnsuccessfulAndEmptyResult_ForInvalidPdfFile()
        {
            var pdFtk = new PDFtk();
        
            var result = await pdFtk.GetPages(Guid.NewGuid().ToByteArray(), 1,5,6,7,10);
        
            result.Success.Should().BeFalse();
            result.Result.Should().BeEmpty();
        }
    }
}
