using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace Kevsoft.PDFtk
{
    internal sealed class XfdfGenerator
    {
        internal async Task<TempPDFtkFile> CreateXfdfFile(IReadOnlyDictionary<string, string> fieldData)
        {
            var inputFile = new TempPDFtkFile();

            var xmlWriterSettings = new XmlWriterSettings()
            {
                Async = true
            };

            await using var xmlWriter =
                XmlWriter.Create(inputFile.TempFileName, xmlWriterSettings);

            await WriteXmlDocument(xmlWriter, fieldData);

            await xmlWriter.FlushAsync();

            return inputFile;
        }

        private static async Task WriteXmlDocument(XmlWriter xmlWriter, IReadOnlyDictionary<string, string> fieldData)
        {
            await xmlWriter.WriteStartDocumentAsync();

            await xmlWriter.WriteStartElementAsync(null, "xfdf", "http://ns.adobe.com/xfdf/");
            await xmlWriter.WriteAttributeStringAsync("xml", "space", null, "preserve");

            await WriteFieldsElement(xmlWriter, fieldData);

            await xmlWriter.WriteEndElementAsync();

            await xmlWriter.WriteEndDocumentAsync();
        }

        private static async Task WriteFieldsElement(XmlWriter xmlWriter, IReadOnlyDictionary<string, string> fieldData)
        {
            await xmlWriter.WriteStartElementAsync(null, "fields", null);

            foreach (var (key, value) in fieldData)
            {
                await WriteField(xmlWriter, key, value);
            }

            await xmlWriter.WriteEndElementAsync();
        }

        private static async Task WriteField(XmlWriter xmlWriter, string key, string value)
        {
            await xmlWriter.WriteStartElementAsync(null, "field", null);
            await xmlWriter.WriteAttributeStringAsync(null, "name", null, key);

            await xmlWriter.WriteStartElementAsync(null, "value", null);
            await xmlWriter.WriteStringAsync(value);

            await xmlWriter.WriteEndElementAsync();

            await xmlWriter.WriteEndElementAsync();
        }
    }
}