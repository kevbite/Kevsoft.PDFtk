using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kevsoft.PDFtk.Tests
{
    public class CompressionTests
    {
        private readonly PDFtk _pdFtk = new();

        private static readonly Regex PageContentsReferenceRegex = new(
            @"/Type\s*/Page\b(?!s).*?/Contents\s+(?<objectId>\d+)\s+\d+\s+R",
            RegexOptions.Singleline);

        [Fact]
        public async Task ShouldCompressPdfFromFilePath()
        {
            var decompressed = await _pdFtk.DecompressAsync(TestFiles.TestFile1Path);
            decompressed.Success.Should().BeTrue();

            var result = await _pdFtk.CompressAsync(decompressed.Result);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();
            (await _pdFtk.GetNumberOfPagesAsync(result.Result)).Result.Should().Be(10);
            HasFlateCompressedPageStreams(result.Result).Should().BeTrue();
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
            HasFlateCompressedPageStreams(result.Result).Should().BeFalse();
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

        private static bool HasFlateCompressedPageStreams(byte[] pdf)
        {
            var pdfText = Encoding.Latin1.GetString(pdf);
            var pageContentObjectIds = PageContentsReferenceRegex.Matches(pdfText)
                .Cast<Match>()
                .Select(match => match.Groups["objectId"].Value)
                .Distinct()
                .ToArray();

            pageContentObjectIds.Should().NotBeEmpty("the fixture contains page content streams");

            return pageContentObjectIds.All(objectId => Regex.IsMatch(
                pdfText,
                $@"(?m)^{objectId}\s+\d+\s+obj\s*<<.*?/Filter\s*/FlateDecode.*?>>\s*stream",
                RegexOptions.Singleline));
        }
    }
}
