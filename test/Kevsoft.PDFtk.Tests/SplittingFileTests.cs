using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kevsoft.PDFtk.Tests
{
    public class SplittingFileTests
    {
        [Fact]
        public async Task ShouldReturnSinglePages()
        {
            var pdFtk = new PDFtk();

            var fileByes = await File.ReadAllBytesAsync("TestFiles/TestFile1.pdf");
            
            var result = await pdFtk.Split(fileByes);

            result.Success.Should().BeTrue();
            result.Result.Should().HaveCount(10);
        }
        
        [Fact]
        public async Task ShouldReturnUnsuccessfulAndEmptyResult_ForInvalidPdfFile()
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