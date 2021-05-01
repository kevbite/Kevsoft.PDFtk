using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kevsoft.PDFtk.Tests
{
    public class StampingTests
    {
        [Fact]
        public async Task ShouldReturnStampedPdf()
        {
            var pdFtk = new PDFtk();

            var fileBytes = await File.ReadAllBytesAsync("TestFiles/TestFile1.pdf");
            var stampBytes = await File.ReadAllBytesAsync("TestFiles/Stamp.pdf");

            var result = await pdFtk.Stamp(fileBytes, stampBytes);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();
        }
        
        [Fact]
        public async Task ShouldReturnUnsuccessfulAndEmptyResult_ForInvalidPdfFile()
        {
            var pdFtk = new PDFtk();

            var fileBytes = Guid.NewGuid().ToByteArray();
            var stampBytes = await File.ReadAllBytesAsync("TestFiles/Stamp.pdf");
            
            var result = await pdFtk.Stamp(fileBytes, stampBytes);

            result.Success.Should().BeFalse();
            result.Result.Should().BeEmpty();
        }
        
        [Fact]
        public async Task ShouldReturnUnsuccessfulAndEmptyResult_ForInvalidStamp()
        {
            var pdFtk = new PDFtk();

            var fileBytes = await File.ReadAllBytesAsync("TestFiles/TestFile1.pdf");
            var stampBytes = Guid.NewGuid().ToByteArray();
            
            var result = await pdFtk.Stamp(fileBytes, stampBytes);

            result.Success.Should().BeFalse();
            result.Result.Should().BeEmpty();
        }
    }
}