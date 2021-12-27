# Kevsoft.PDFtk [![Continuous Integration Workflow](https://github.com/kevbite/Kevsoft.PDFtk/actions/workflows/continuous-integration-workflow.yml/badge.svg)](https://github.com/kevbite/Kevsoft.PDFtk/actions/workflows/continuous-integration-workflow.yml) [![install from nuget](http://img.shields.io/nuget/v/Kevsoft.PDFtk.svg?style=flat-square)](https://www.nuget.org/packages/Kevsoft.PDFtk) [![downloads](http://img.shields.io/nuget/dt/Kevsoft.PDFtk.svg?style=flat-square)](https://www.nuget.org/packages/Kevsoft.PDFtk)

.NET Library to drive the awesome [pdftk](https://www.pdflabs.com/tools/pdftk-the-pdf-toolkit/) binary.

Inspired by [pypdftk](https://github.com/revolunet/pypdftk) from [revolunet](https://github.com/revolunet).

## Getting Started

### Prerequisites

You'll need to have [PDFtk Server](https://www.pdflabs.com/tools/pdftk-server/) installed and the bin directory included within your [PATH](https://en.wikipedia.org/wiki/PATH_(variable)) environment variable.

#### Windows

If you're on windows this can be installed directly from [chocolatey](https://chocolatey.org/).

```powershell
choco install pdftk-server
```

Alternatively you can [download](https://www.pdflabs.com/tools/pdftk-server/#download) the exe from PDFtk from their main site and install it manually.

#### Linux

This is very dependant on your Linux distribution, but you can install this from the package manager of choice.

##### Ubuntu

```bash
apt-get install pdftk
```

#### macOS

I don't have a mac anymore, but it seems you can install this with [brew](http://brew.sh/).

```bash
brew tap spl/pdftk
brew install pdftk
```

### Installing Package

**Kevsoft.PDFtk** can be installed directly via the package manager console by executing the following commandlet:

```powershell
Install-Package Kevsoft.PDFtk
```

alternative you can use the dotnet CLI.

```bash
dotnet add package Kevsoft.PDFtk
```

## Usage

### Filling a PDF Form.

Fill a PDF with given data and returns the PDF bytes.

```csharp
var pdftk = new PDFtk();

var fieldData = new Dictionary<string, string>()
{
   ["Given Name Text Box"] = "Kevin",
   ["Language 3 Check Box"] = "Yes"
};

var result = await pdftk.FillFormAsync(
   pdfFile: await File.ReadAllBytesAsync("myForm.pdf"),
   fieldData: FieldData,
   flatten: false,
   dropXfa: dropfalse
);

if(result.Success)
{
   // Do something with result.Result (bytes[])
}
```

### Get Field Information

Read PDF and output form field statistics.

```csharp
var pdftk = new PDFtk();

var pdfBytes = await File.ReadAllBytesAsync("Form.pdf");
var result = await pdftk.GetDataFieldsAsync(pdfBytes);

if(result.Success)
{
   // Do something with result.Result (IDataField[])
}
```
`IDataField` interface:
```csharp
public interface IDataField
{
    string? FieldValue { get; }
    string? FieldValueDefault { get; }
    string? FieldType { get; }
    string? FieldName { get; }
    string? FieldNameAlt { get; }
    string? FieldFlags { get; }
    string? FieldJustification { get; }
    string[] FieldStateOption { get; }
    string? FieldMaxLength { get; }
}
```

### Concatenate Multiple PDFs

Merge multiple PDFs into one single PDF and returns the PDF bytes.

```csharp
var pdftk = new PDFtk();

var result = await pdftk.ConcatAsync(new[]
{
    await File.ReadAllBytesAsync("Pdf1.pdf"),
    await File.ReadAllBytesAsync("Pdf2.pdf")
});

if(result.Success)
{
   // Do something with result.Result (bytes[])
}
```

### Concatenate a List of Pages

Concatenate a list of page ranges into one single file and returns the PDF bytes.

```csharp
var pdftk = new PDFtk();

var pages = new []{ 1, 5, 6, 7, 10 };
var pdfBytes = await File.ReadAllBytesAsync("Form.pdf");
var result = await pdftk.GetPagesAsync(pdfBytes, pages);

if(result.Success)
{
   // Do something with result.Result (bytes[])
}
```

### Splitting a PDF into Many PDFs

Split a single PDF in many pages and return a list of PDF bytes.

```csharp
var pdftk = new PDFtk();

var pdfBytes = await File.ReadAllBytesAsync("Form.pdf");
var result = await pdftk.SplitAsync(pdfBytes);

if(result.Success)
{
   foreach (var (fileName, bytes) in result.Result)
   {
       // Do something with each pdfPage.
   }
}
```

### Getting Total Pages

Return the number of pages for a PDF bytes.

```csharp
var pdftk = new PDFtk();

var pdfBytes = await File.ReadAllBytesAsync("Form.pdf");
var result = await pdftk.GetNumberOfPagesAsync(pdfBytes);

if(result.Success)
{
   // result.Result is the number of pages.
}
```

### Stamping a PDF

Applies a stamp to the PDF file.

```csharp
var pdftk = new PDFtk();

var result = await pdftk.StampAsync(
    await File.ReadAllBytesAsync("MyDocument.pdf"),
    await File.ReadAllBytesAsync("Stamp.pdf")
);

if(result.Success)
{
   // Do something with result.Result (bytes[])
}
```

### Replace a Page

Replace a page in a PDF with another PDF.

```csharp
var pdftk = new PDFtk();

var result = await pdftk.ReplacePage(
    await File.ReadAllBytesAsync("MyDocument.pdf"),
    page: 3,
    await File.ReadAllBytesAsync("replacement.pdf")
);

if(result.Success)
{
   // Do something with result.Result (bytes[])
}
```
### Replace Pages

Replace multiple pages within a PDF with another PDF.

```csharp
var pdftk = new PDFtk();

var result = await pdftk.ReplacePage(
    await File.ReadAllBytesAsync("MyDocument.pdf"),
    startPage: 3,
    endPage: 5,
    await File.ReadAllBytesAsync("replacement.pdf")
);

if(result.Success)
{
   // Do something with result.Result (bytes[])
}
```

### Attach files

Attaches files to a PDF, if `page` argument is supplied then files are attached to a given page, if `page` argument is not specified then files are attached at document level.

```csharp
var pdftk = new PDFtk();

var result = await _pdFtk.AttachFiles(
   await File.ReadAllBytesAsync("MyDocument.pdf"),
   new Dictionary<string, byte[]>
   {
         ["test-file1.txt"] = Encoding.ASCII.GetBytes("Hello"),
         ["test-file2.txt"] = Encoding.ASCII.GetBytes("World")
   },
   page: 10 // Optional page to attach files
   );

if(result.Success)
{
   // Do something with result.Result (bytes[])
}
```

### Extract Attachments

Extracts attachments from a PDF file.

```csharp
var pdftk = new PDFtk();

var result = await _pdFtk.ExtractAttachments(
   await File.ReadAllBytesAsync("MyDocument.pdf"));

if(result.Success)
{
   foreach (var (fileName, bytes) in result.Result)
   {
       // Do something with each attachment.
   }
}
```

### Compression/Decompression

These are only useful when you want to edit PDF code in a text
editor like vim or emacs.  Remove PDF page stream compression by
applying the uncompress filter. 

*Not Implemented yet.*

## Samples

The [samples](samples/) folder containers examples of how you could use the Kevsoft.PDFtk Library.

### WebApplicationFillForm

This is a small [razor pages website](samples/WebApplicationFillForm) that has a form where you can fill in a PDF and generate an output for the browser to download.

## Contributing

1. Issue
1. Fork
1. Hack!
1. Pull Request

This source includes a [VSCode Remote Containers](https://code.visualstudio.com/docs/remote/containers) to help you get setup fast for development, it comes preinstalled with the prerequisites.

