using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kevsoft.PDFtk.Tests
{
    public class CompressionTests
    {
        private readonly PDFtk _pdFtk = new();

        [Fact]
        public async Task ShouldCompressPdfFromFilePath()
        {
            var result = await _pdFtk.CompressAsync(TestFiles.TestFile1Path);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();
            (await _pdFtk.GetNumberOfPagesAsync(result.Result)).Result.Should().Be(10);
        }

        [Fact]
        public async Task ShouldCompressPdfFromByteArray()
        {
            var input = await File.ReadAllBytesAsync(TestFiles.TestFile1Path);

            var result = await _pdFtk.CompressAsync(input);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();
            (await _pdFtk.GetNumberOfPagesAsync(result.Result)).Result.Should().Be(10);
        }

        [Fact]
        public async Task ShouldCompressPdfFromStream()
        {
            await using var input = File.OpenRead(TestFiles.TestFile1Path);

            var result = await _pdFtk.CompressAsync(input);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();
            (await _pdFtk.GetNumberOfPagesAsync(result.Result)).Result.Should().Be(10);
        }

        [Fact]
        public async Task ShouldDecompressPdfFromFilePath()
        {
            var result = await _pdFtk.DecompressAsync(TestFiles.TestFile1Path);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();
            (await _pdFtk.GetNumberOfPagesAsync(result.Result)).Result.Should().Be(10);
        }

        [Fact]
        public async Task ShouldDecompressPdfFromByteArray()
        {
            var input = await File.ReadAllBytesAsync(TestFiles.TestFile1Path);

            var result = await _pdFtk.DecompressAsync(input);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();
            (await _pdFtk.GetNumberOfPagesAsync(result.Result)).Result.Should().Be(10);
        }

        [Fact]
        public async Task ShouldDecompressPdfFromStream()
        {
            await using var input = File.OpenRead(TestFiles.TestFile1Path);

            var result = await _pdFtk.DecompressAsync(input);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();
            (await _pdFtk.GetNumberOfPagesAsync(result.Result)).Result.Should().Be(10);
        }
    }
}