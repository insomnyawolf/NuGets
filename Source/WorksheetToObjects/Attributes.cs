using System;

namespace WorksheetToObjects
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class WorkSheetNonRequieredAttribute : Attribute
    {
        public WorkSheetNonRequieredAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class WorkSheetColumnNameAttribute : Attribute
    {
        public string ColumnName { get; }
        public WorkSheetColumnNameAttribute(string ColumnName)
        {
            this.ColumnName = ColumnName;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class WorkSheetNonMappedFieldsAttribute : Attribute
    {
        public WorkSheetNonMappedFieldsAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class WorkSheetNameAttribute : Attribute
    {
        public string SheetName { get; }
        public WorkSheetNameAttribute(string SheetName)
        {
            this.SheetName = SheetName;
        }
    }
}
