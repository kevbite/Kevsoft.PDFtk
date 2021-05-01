namespace Kevsoft.PDFtk
{
    public interface IDataField
    {
        public string? FieldValue { get; }
        public string? FieldValueDefault { get; }
        public string? FieldType { get; }
        public string? FieldName { get; }
        public string? FieldNameAlt { get; }
        public string? FieldFlags { get; }
        public string? FieldJustification { get; }
        public string[] FieldStateOption { get; }
        public string? FieldMaxLength { get; }
    }
}