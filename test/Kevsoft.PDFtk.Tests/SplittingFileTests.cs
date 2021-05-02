using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kevsoft.PDFtk.Tests
{
    public class SplittingFileTests
    {
        private readonly PDFtk _pdFtk = new();

        [Fact]
        public async Task ShouldReturnSinglePages_ForInputFileAsBytes()
        {
            var fileByes = await File.ReadAllBytesAsync(TestFiles.TestFile1Path);
            
            var result = await _pdFtk.Split(fileByes);

            result.Success.Should().BeTrue();
            result.Result.Should().HaveCount(10);
        }
        
        [Fact]
        public async Task ShouldReturnSinglePages_ForInputFileAsStream()
        {
            var stream = File.OpenRead(TestFiles.TestFile1Path);
            
            var result = await _pdFtk.Split(stream);

            result.Success.Should().BeTrue();
            result.Result.Should().HaveCount(10);
        }
        
        [Fact]
        public async Task ShouldReturnSinglePages_ForInputFileAsFilePath()
        {
            var result = await _pdFtk.Split(TestFiles.TestFile1Path);

            result.Success.Should().BeTrue();
            result.Result.Should().HaveCount(10);
        }
        
        [Fact]
        public async Task ShouldReturnUnsuccessfulAndEmptyResult_ForInvalidPdfFile()
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