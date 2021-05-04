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
var pdFtk = new PDFtk();

var fieldData = new Dictionary<string, string>()
{
   ["Given Name Text Box"] = "Kevin",
   ["Language 3 Check Box"] = "Yes"
};

var result = await _pdFtk.FillFormAsync(
   pdfFile: await File.ReadAllBytesAsync("myForm.pdf"),
   fieldData: FieldData,
   flatten: false,
   dropXfa: false
);

if(result.Success)
{
   // Do something with result.Result (bytes[])
}
```

### Get Field Information

Read PDF and output form field statistics.

```csharp
var pdFtk = new PDFtk();

var result = await pdFtk.GetDataFieldsAsync(File.ReadAllBytesAsync("Form.pdf"));

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
var pdFtk = new PDFtk();

var result = await pdFtk.ConcatAsync(new[]
{
    File.ReadAllBytesAsync("Pdf1.pdf"),
    File.ReadAllBytesAsync("Pdf2.pdf")
});

if(result.Success)
{
   // Do something with result.Result (bytes[])
}
```

### Concatenate a List of Pages

Concatenate a list of page ranges into one single file and returns the PDF bytes.

```csharp
var pdFtk = new PDFtk();

var pages = new []{ 1, 5, 6, 7, 10 };
var result = await pdFtk.GetPagesAsync(File.ReadAllBytesAsync("Form.pdf"), pages);

if(result.Success)
{
   // Do something with result.Result (bytes[])
}
```

### Splitting a PDF into Many PDFs

Split a single PDF in many pages and return a list of PDF bytes.

```csharp
var pdFtk = new PDFtk();
            
var result = await pdFtk.SplitAsync(File.ReadAllBytesAsync("Form.pdf"));

if(result.Success)
{
   foreach (var pdfPage in result.Result)
   {
       // Do something with each pdfPage (byte[]).
   }
}
```

### Getting Total Pages

Return the number of pages for a PDF bytes.

```csharp
var pdFtk = new PDFtk();
            
var result = await pdFtk.GetNumberOfPagesAsync(File.ReadAllBytesAsync("Form.pdf"));

if(result.Success)
{
   // result.Result is the number of pages.
}
```

### Stamping a PDF

Applies a stamp to the PDF file.

```csharp
var pdFtk = new PDFtk();

var result = await pdFtk.StampAsync(
    File.ReadAllBytesAsync("MyDocument.pdf"),
    File.ReadAllBytesAsync("Stamp.pdf")
);

if(result.Success)
{
   // Do something with result.Result (bytes[])
}
```

### Replace a Page

Replace a page in a PDF.

*Not Implemented yet.*

### Compression/Decompression

These are only useful when you want to edit PDF code in a text
editor like vim or emacs.  Remove PDF page stream compression by
applying the uncompress filter. 

*Not Implemented yet.*

## Contributing

1. Issue
1. Fork
1. Hack!
1. Pull Request

This source includes a [VSCode Remote Containers](https://code.visualstudio.com/docs/remote/containers) to help you get setup fast for development, it comes preinstalled with the prerequisites.

