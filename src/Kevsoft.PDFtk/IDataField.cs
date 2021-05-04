namespace Kevsoft.PDFtk
{
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
}