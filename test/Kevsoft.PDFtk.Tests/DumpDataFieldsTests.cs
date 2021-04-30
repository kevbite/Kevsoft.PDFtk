using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kevsoft.PDFtk.Tests
{
    public class DumpDataFieldsTests
    {
        [Fact]
        public async Task ShouldReturnSuccessAndAllDataFields()
        {
            var pdFtk = new PDFtk();

            var pdfFileBytes = await File.ReadAllBytesAsync("TestFiles/Form.pdf");
            var result = await pdFtk.DumpDataFields(pdfFileBytes);

            result.Success.Should().BeTrue();
            result.Result.Should().BeEquivalentTo(
                new[]
                {
                    new TestDataField
                    {
                        FieldFlags = "0",
                        FieldNameAlt = "First name",
                        FieldName = "Given Name Text Box",
                        FieldType = "Text",
                        FieldJustification = "Left",
                        FieldMaxLength = "40",
                        FieldValue = ""
                    },
                    new TestDataField
                    {
                        FieldFlags = "0",
                        FieldNameAlt = "Last name",
                        FieldName = "Family Name Text Box",
                        FieldType = "Text",
                        FieldJustification = "Left",
                        FieldMaxLength = "40",
                        FieldValue = ""
                    },
                    new TestDataField
                    {
                        FieldFlags = "0",
                        FieldNameAlt = "House and floor",
                        FieldName = "House nr Text Box",
                        FieldType = "Text",
                        FieldJustification = "Left",
                        FieldMaxLength = "20",
                        FieldValue = ""
                    },
                    new TestDataField
                    {
                        FieldFlags = "0",
                        FieldName = "Address 2 Text Box",
                        FieldType = "Text",
                        FieldJustification = "Left",
                        FieldMaxLength = "40",
                        FieldValue = ""
                    },
                    new TestDataField
                    {
                        FieldFlags = "0",
                        FieldName = "Postcode Text Box",
                        FieldType = "Text",
                        FieldJustification = "Left",
                        FieldMaxLength = "20",
                        FieldValue = ""
                    },
                    new TestDataField
                    {
                        FieldStateOption = new[]
                        {
                            "Austria", "Belgium", "Britain", "Bulgaria", "Croatia",
                            "Cyprus", "Czech-Republic", "Denmark", "Estonia", "Finland", "France",
                            "Germany", "Greece", "Hungary", "Ireland", "Italy", "Latvia",
                            "Lithuania", "Luxembourg", "Malta", "Netherlands", "Poland", "Portugal",
                            "Romania", "Slovakia", "Slovenia", "Spain", "Sweden"
                        },
                        FieldFlags = "393216",
                        FieldNameAlt = "Use selection or write country name",
                        FieldName = "Country Combo Box",
                        FieldType = "Choice",
                        FieldJustification = "Left",
                        FieldValue = ""
                    },
                    new TestDataField
                    {
                        FieldFlags = "0",
                        FieldNameAlt = "Value from 40 to 250 cm",
                        FieldName = "Height Formatted Field",
                        FieldType = "Text",
                        FieldJustification = "Left",
                        FieldMaxLength = "20",
                        FieldValueDefault = "150",
                        FieldValue = "150"
                    },
                    new TestDataField
                    {
                        FieldFlags = "0",
                        FieldName = "City Text Box",
                        FieldType = "Text",
                        FieldJustification = "Left",
                        FieldMaxLength = "40",
                        FieldValue = ""
                    },
                    new TestDataField
                    {
                        FieldStateOption = new[] {"Off", "Yes"},
                        FieldFlags = "0",
                        FieldNameAlt = "Car driving license",
                        FieldName = "Driving License Check Box",
                        FieldType = "Button",
                        FieldJustification = "Left",
                        FieldValue = "Off"
                    },
                    new TestDataField
                    {
                        FieldStateOption = new[]
                            {"Black", "Blue", "Brown", "Green", "Grey", "Orange", "Red", "Violet", "White", "Yellow"},
                        FieldFlags = "131072",
                        FieldNameAlt = "Select from colour spectrum",
                        FieldName = "Favourite Colour List Box",
                        FieldType = "Choice",
                        FieldJustification = "Left",
                        FieldValueDefault = "Red",
                        FieldValue = "Red"
                    },
                    new TestDataField
                    {
                        FieldStateOption = new[] {"Off", "Yes"},
                        FieldFlags = "0",
                        FieldName = "Language 1 Check Box",
                        FieldType = "Button",
                        FieldJustification = "Left",
                        FieldValue = "Off"
                    },
                    new TestDataField
                    {
                        FieldStateOption = new[] {"Off", "Yes"},
                        FieldFlags = "0",
                        FieldName = "Language 2 Check Box",
                        FieldType = "Button",
                        FieldJustification = "Left",
                        FieldValue = "Yes"
                    },
                    new TestDataField
                    {
                        FieldStateOption = new[] {"Off", "Yes"},
                        FieldFlags = "0",
                        FieldName = "Language 3 Check Box",
                        FieldType = "Button",
                        FieldJustification = "Left",
                        FieldValue = "Off"
                    },
                    new TestDataField
                    {
                        FieldStateOption = new[] {"Off", "Yes"},
                        FieldFlags = "0",
                        FieldName = "Language 4 Check Box",
                        FieldType = "Button",
                        FieldJustification = "Left",
                        FieldValue = "Off"
                    },
                    new TestDataField
                    {
                        FieldStateOption = new[] {"Off", "Yes"},
                        FieldFlags = "0",
                        FieldName = "Language 5 Check Box",
                        FieldType = "Button",
                        FieldJustification = "Left",
                        FieldValue = "Off"
                    },
                    new TestDataField
                    {
                        FieldStateOption = new[] {"Man", "Woman"},
                        FieldFlags = "131072",
                        FieldNameAlt = "Select from list",
                        FieldName = "Gender List Box",
                        FieldType = "Choice",
                        FieldJustification = "Left",
                        FieldValue = "Man",
                        FieldValueDefault = "Man"
                    },
                    new TestDataField
                    {
                        FieldFlags = "0",
                        FieldName = "Address 1 Text Box",
                        FieldType = "Text",
                        FieldJustification = "Left",
                        FieldMaxLength = "40",
                        FieldValue = ""
                    }
                }, options => options.WithStrictOrdering()
            );
        }

        [Fact]
        public async Task ShouldReturnUnsuccessfulAndNullResult()
        {
            var pdFtk = new PDFtk();

            var result = await pdFtk.DumpDataFields(Guid.NewGuid().ToByteArray());

            result.Success.Should().BeFalse();
            result.Result.Should().BeEmpty();
        }
        
        public sealed class TestDataField
        {
            public string? FieldValue { get; set; }
            public string? FieldValueDefault { get; set; }
            public string? FieldType { get; set; }
            public string? FieldName { get; set; }
            public string? FieldNameAlt { get; set; }
            public string? FieldFlags { get; set; }
            public string? FieldJustification { get; set; }
            public string[] FieldStateOption { get; set; } = Array.Empty<string>();
            public string? FieldMaxLength { get; set; }
        }
    }
}