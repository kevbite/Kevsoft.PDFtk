namespace Kevsoft.PDFtk
{
    /// <summary>
    /// Statistics of a single data field within the PDF. 
    /// </summary>
    public interface IDataField
    {
        /// <summary>
        /// The field value.
        /// </summary>
        string? FieldValue { get; }
        
        /// <summary>
        /// The default field value.
        /// </summary>
        string? FieldValueDefault { get; }
        
        /// <summary>
        /// The field type.
        /// </summary>
        string? FieldType { get; }
        
        /// <summary>
        /// The field name.
        /// </summary>
        string? FieldName { get; }
        
        /// <summary>
        /// The field alternative description.
        /// </summary>
        string? FieldNameAlt { get; }
        
        /// <summary>
        /// The field flags.
        /// </summary>
        string? FieldFlags { get; }
        
        /// <summary>
        /// The field justification.
        /// </summary>
        string? FieldJustification { get; }
        
        /// <summary>
        /// The field possible values.
        /// </summary>
        string[] FieldStateOption { get; }
        
        /// <summary>
        /// The field maximum length.
        /// </summary>
        string? FieldMaxLength { get; }
    }
}