using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kevsoft.PDFtk.Tests
{
    public class NumberOfPagesTests
    {
        [Fact]
        public async Task ShouldReturnSuccessAndCorrectTotalNumberOfPages()
        {
            var pdFtk = new PDFtk();

            var pdfFileBytes = await File.ReadAllBytesAsync("TestFiles/TestFile1.pdf");
            var result = await pdFtk.GetNumberOfPages(pdfFileBytes);
                
            result.Success.Should().BeTrue();
            result.Result.Should().Be(10);
        }
        
        [Fact]
        public async Task ShouldReturnUnsuccessfulAndNullResult()
        {
            var pdFtk = new PDFtk();

            var result = await pdFtk.GetNumberOfPages(Guid.NewGuid().ToByteArray());

            result.Success.Should().BeFalse();
            result.Result.Should().BeNull();
        }
    }
}
