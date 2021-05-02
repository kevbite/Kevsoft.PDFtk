using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kevsoft.PDFtk.Tests
{
    public class NumberOfPagesTests
    {
        private readonly PDFtk _pdFtk = new();

        [Fact]
        public async Task ShouldReturnSuccessAndCorrectTotalNumberOfPages_ForInputFileAsBytes()
        {
            var pdfFileBytes = await File.ReadAllBytesAsync("TestFiles/TestFile1.pdf");

            var result = await _pdFtk.GetNumberOfPages(pdfFileBytes);
                
            result.Success.Should().BeTrue();
            result.Result.Should().Be(10);
        }
        
        [Fact]
        public async Task ShouldReturnSuccessAndCorrectTotalNumberOfPages_ForInputFileFilePath()
        {
            var result = await _pdFtk.GetNumberOfPages("TestFiles/TestFile1.pdf");
                
            result.Success.Should().BeTrue();
            result.Result.Should().Be(10);
        }
        
        [Fact]
        public async Task ShouldReturnSuccessAndCorrectTotalNumberOfPages_ForStream()
        {
            var stream = File.OpenRead("TestFiles/TestFile1.pdf");
            var result = await _pdFtk.GetNumberOfPages(stream);
                
            result.Success.Should().BeTrue();
            result.Result.Should().Be(10);
        }
        
        [Fact]
        public async Task ShouldReturnUnsuccessfulAndNullResult()
        {
            var result = await _pdFtk.GetNumberOfPages(Guid.NewGuid().ToByteArray());

            result.Success.Should().BeFalse();
            result.Result.Should().BeNull();
        }
    }
}
