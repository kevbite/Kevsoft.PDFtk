using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kevsoft.PDFtk.Tests
{
    public class ReplacePagesTests
    {
        private readonly PDFtk _pdFtk = new();

        private const int FirstPage = 1;
        private const int LastPage = 10;

        [Theory]
        [InlineData(FirstPage, 3)]
        [InlineData(8, LastPage)]
        [InlineData(4, 6)]
        [InlineData(4, 8)]
        public async Task ShouldReturnPdfWithReplacedRangeOfPages(int start, int end)
        {
            const int totalPagesInserted = 2;
            
            var result = await _pdFtk.ReplacePages(TestFiles.TestFile1Path, start,end, TestFiles.TestFileWith2PagesPath);

            result.Success.Should().BeTrue();
            result.Result.Should().NotBeEmpty();
            (await _pdFtk.GetNumberOfPagesAsync(result.Result)).Result.Should().Be(LastPage - (end - start + 1) + totalPagesInserted);
        }
    }
}