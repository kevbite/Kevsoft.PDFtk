using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kevsoft.PDFtk.Tests
{
    public class FillFormTests
    {
        [Fact]
        public async Task ShouldFillPdfForm()
        {
            var pdFtk = new PDFtk();

            var fieldData = new Dictionary<string, string>
            {
                ["Given Name Text Box"] = Guid.NewGuid().ToString(),
                ["Language 3 Check Box"] = "Yes",
            };

            var fileBytes = await File.ReadAllBytesAsync("TestFiles/Form.pdf");

            var result = await pdFtk.FillForm(fileBytes, fieldData, false, false);

            result.Success.Should().BeTrue();
            var dumpDataFields = await pdFtk.DumpDataFields(result.Result);
            dumpDataFields.Result.Where(x => fieldData.Keys.Contains(x.FieldName))
                .ToDictionary(x => x.FieldName!, field => field.FieldValue)
                .Should()
                .BeEquivalentTo(fieldData);
        }

        [Fact]
        public async Task ShouldReturnUnsuccessfulAndEmptyResult_ForInvalidPdfFile()
        {
            var pdFtk = new PDFtk();

            var fileBytes = Guid.NewGuid().ToByteArray();

            var result = await pdFtk.FillForm(fileBytes, new Dictionary<string, string>(), false, false);

            result.Success.Should().BeFalse();
            result.Result.Should().BeEmpty();
        }

    }
}