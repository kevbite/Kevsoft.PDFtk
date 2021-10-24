using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kevsoft.PDFtk.Tests
{
    public class AttachFilesTests : IAsyncLifetime
    {
        private readonly PDFtk _pdFtk = new();

        private readonly IReadOnlyDictionary<string, string>
            _attachments = new Dictionary<string, string>
            {
                [Path.GetTempFileName()] = "Hello World",
                [Path.GetTempFileName()] =
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque aliquet sagittis felis eget pharetra.",
            };

        public async Task InitializeAsync()
        {
            foreach (var (fileName, content) in _attachments)
            {
                await File.WriteAllTextAsync(fileName, content);
            }
        }

        [Fact]
        public async Task ShouldReturnPdfWithAttachments()
        {
            var result =
                await _pdFtk.AttachFiles(TestFiles.TestFileWith3PagesPath, _attachments.Keys);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();

            await AssertPdfFileAttachments(result, _attachments);
        }


        [Fact]
        public async Task ShouldReturnPdfWithAttachments_ForInputFilesAsBytes()
        {
            var input = await File.ReadAllBytesAsync(TestFiles.TestFileWith3PagesPath);
            var attachments = new Dictionary<string, byte[]>
            {
                ["test-file1.txt"] = Encoding.ASCII.GetBytes("Hello"),
                ["test-file2.txt"] = Encoding.ASCII.GetBytes("World")
            };

            var result = await _pdFtk.AttachFiles(input, attachments);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();

            var extractAttachments = await _pdFtk.ExtractAttachments(result.Result);
            extractAttachments.Result.Count.Should().Be(attachments.Count);

            extractAttachments.Result.Should().BeEquivalentTo(attachments);
        }
        
        
        [Fact]
        public async Task ShouldReturnPdfWithAttachments_ForInputFilesAsStreams()
        {
            await using var input = File.OpenRead(TestFiles.TestFileWith3PagesPath);
            var attachments = new Dictionary<string, byte[]>
            {
                ["test-file1.txt"] = Encoding.ASCII.GetBytes("Hello"),
                ["test-file2.txt"] = Encoding.ASCII.GetBytes("World")
            };

            var result = await _pdFtk.AttachFiles(input, attachments
                .Select(kvp => KeyValuePair.Create<string, Stream>(kvp.Key, new MemoryStream(kvp.Value))));

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();

            var extractAttachments = await _pdFtk.ExtractAttachments(result.Result);
            extractAttachments.Result.Count.Should().Be(attachments.Count);

            extractAttachments.Result.Should().BeEquivalentTo(attachments);
        }

        [Fact]
        public async Task ShouldReturnPdfWithAttachments_ForGivenPage()
        {
            var result = await _pdFtk.AttachFiles(TestFiles.TestFileWith3PagesPath,
                _attachments.Keys, 2);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();

            var page2Result = await _pdFtk.GetPagesAsync(result.Result, 2);
            await AssertPdfFileAttachments(page2Result, _attachments);
        }

        [Fact]
        public async Task ShouldReturnPdfWithNoAttachments_ForGivenPage()
        {
            var result = await _pdFtk.AttachFiles(TestFiles.TestFileWith3PagesPath,
                _attachments.Keys, 1);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();

            var page2Result = await _pdFtk.GetPagesAsync(result.Result, 2);
            await AssertPdfFileAttachments(page2Result, new Dictionary<string, string>());
        }

        private async Task AssertPdfFileAttachments(IPDFtkResult<byte[]> result,
            IReadOnlyDictionary<string, string> attachments)
        {
            var extractAttachments = await _pdFtk.ExtractAttachments(result.Result);
            extractAttachments.Result.Count.Should().Be(attachments.Count);

            extractAttachments.Result.Should().BeEquivalentTo(
                attachments.ToDictionary(kvp => Path.GetFileName(kvp.Key),
                    kvp => Encoding.ASCII.GetBytes(kvp.Value)
                ));
        }

        public Task DisposeAsync()
        {
            foreach (var (fileName, _) in _attachments)
            {
                File.Delete(fileName);
            }

            return Task.CompletedTask;
        }
    }
}