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
            var pdfFileBytes = await File.ReadAllBytesAsync(TestFiles.TestFile1Path);

            var result = await _pdFtk.GetNumberOfPagesAsync(pdfFileBytes);

            result.Success.Should().BeTrue();
            result.Result.Should().Be(10);
        }

        [Fact]
        public async Task ShouldReturnSuccessAndCorrectTotalNumberOfPages_ForInputFileFilePath()
        {
            var result = await _pdFtk.GetNumberOfPagesAsync(TestFiles.TestFile1Path);

            result.Success.Should().BeTrue();
            result.Result.Should().Be(10);
        }

        [Fact]
        public async Task ShouldReturnSuccessAndCorrectTotalNumberOfPages_ForInputFileFilePathWithSpaces()
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            try
            {
                Directory.CreateDirectory(path);

                var pdfFilePath = Path.Combine(path, "file with spaces.pdf");
                File.Copy(TestFiles.TestFile1Path, pdfFilePath);

                var result = await _pdFtk.GetNumberOfPagesAsync(pdfFilePath);

                result.Success.Should().BeTrue();
                result.Result.Should().Be(10);
            }
            finally
            {
                Directory.Delete(path, true);
            }
        }

        [Fact]
        public async Task ShouldReturnSuccessAndCorrectTotalNumberOfPages_ForStream()
        {
            await using var stream = File.OpenRead(TestFiles.TestFile1Path);
            var result = await _pdFtk.GetNumberOfPagesAsync(stream);

            result.Success.Should().BeTrue();
            result.Result.Should().Be(10);
        }

        [Fact]
        public async Task ShouldReturnUnsuccessfulAndNullResult()
        {
            var result = await _pdFtk.GetNumberOfPagesAsync(Guid.NewGuid().ToByteArray());

            result.Success.Should().BeFalse();
            result.Result.Should().BeNull();
        }
    }
}