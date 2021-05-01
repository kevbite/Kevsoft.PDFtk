using System;
using System.Collections.Generic;
using System.Linq;

namespace Kevsoft.PDFtk
{
    internal sealed class DataField : IDataField
    {
        public string? FieldValue { get; private set; }
        public string? FieldValueDefault { get; private set; }
        public string? FieldType { get; private set; }
        public string? FieldName { get; private set; }
        public string? FieldNameAlt { get; private set; }
        public string? FieldFlags { get; private set; }
        public string? FieldJustification { get; private set; }
        public string[] FieldStateOption { get; private set; } = new string[0];
        public string? FieldMaxLength { get; private set; }

        internal static DataField Parse(string[] args)
        {
            var dataField = new DataField();
            foreach (var arg in args)
            {
                var split = arg.Split(":", 2);
                var key = split[0];

                var value = split.Length == 1 ? null : split[1][1..];

                FieldSetMap[key](dataField, value);
            }

            return dataField;
        }

        private static readonly Dictionary<string, Action<DataField, string?>> FieldSetMap
            = new()
            {
                ["FieldValue"] = (field, value) => field.FieldValue = value ?? string.Empty,
                ["FieldValueDefault"] = (field, value) => field.FieldValueDefault = value,
                ["FieldType"] = (field, value) => field.FieldType = value,
                ["FieldName"] = (field, value) => field.FieldName = value,
                ["FieldNameAlt"] = (field, value) => field.FieldNameAlt = value,
                ["FieldFlags"] = (field, value) => field.FieldFlags = value,
                ["FieldJustification"] = (field, value) => field.FieldJustification = value,
                ["FieldStateOption"] = (field, value) =>
                {
                    if (value is { })
                    {
                        field.FieldStateOption = field.FieldStateOption.Concat(new[] {value}).ToArray();
                    }
                },
                ["FieldMaxLength"] = (field, value) => field.FieldMaxLength = value
            };
    }
}