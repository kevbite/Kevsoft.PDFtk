using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kevsoft.PDFtk.Tests
{
    public class StampingTests
    {
        private readonly PDFtk _pdFtk = new();

        [Fact]
        public async Task ShouldReturnStampedPdf_ForInputFilesAsBytes()
        {
            var fileBytes = await File.ReadAllBytesAsync(TestFiles.TestFile1Path);
            var stampBytes = await File.ReadAllBytesAsync(TestFiles.StampFilePath);

            var result = await _pdFtk.Stamp(fileBytes, stampBytes);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();
        }
        
        [Fact]
        public async Task ShouldReturnStampedPdf_ForInputFilesAsStreams()
        {
            var inputFileStream = File.OpenRead(TestFiles.TestFile1Path);
            var stampFileStream = File.OpenRead(TestFiles.StampFilePath);

            var result = await _pdFtk.Stamp(inputFileStream, stampFileStream);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();
        }
        
        [Fact]
        public async Task ShouldReturnStampedPdf_ForInputFilesAsFilePaths()
        {
            var result = await _pdFtk.Stamp(TestFiles.TestFile1Path, TestFiles.StampFilePath);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();
        }
        
        [Fact]
        public async Task ShouldReturnUnsuccessfulAndEmptyResult_ForInvalidPdfFile()
        {
            var fileBytes = Guid.NewGuid().ToByteArray();
            var stampBytes = await File.ReadAllBytesAsync(TestFiles.StampFilePath);
            
            var result = await _pdFtk.Stamp(fileBytes, stampBytes);

            result.Success.Should().BeFalse();
            result.Result.Should().BeEmpty();
        }
        
        [Fact]
        public async Task ShouldReturnUnsuccessfulAndEmptyResult_ForInvalidStamp()
        {
            var fileBytes = await File.ReadAllBytesAsync(TestFiles.TestFile1Path);
            var stampBytes = Guid.NewGuid().ToByteArray();
            
            var result = await _pdFtk.Stamp(fileBytes, stampBytes);

            result.Success.Should().BeFalse();
            result.Result.Should().BeEmpty();
        }
    }
}