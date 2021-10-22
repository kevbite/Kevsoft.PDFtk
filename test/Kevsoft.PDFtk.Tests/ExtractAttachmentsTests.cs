using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kevsoft.PDFtk.Tests
{
    public class ExtractAttachmentsTests
    {
        private readonly IPDFtk _pdFtk = new PDFtk();

        [Fact]
        public async Task ShouldReturnAttachments_ForInputFileAsFilePath()
        {
            var result = await _pdFtk.ExtractAttachments(TestFiles.TestFileWithAttachmentsPath);

            result.Success.Should().BeTrue();
            result.Result.Should().HaveCount(1);
            result.Result.First().Key.Should().Be("utf8test.txt");
            result.Result.First().Key.Should().NotBeEmpty();
        }
    }
}