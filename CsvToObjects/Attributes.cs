using System;
namespace CsvToObjects
{
    /// <summary>
    /// Used to binda column drom the original csv into the target object
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class CsvNameAttribute : Attribute
    {
        public CsvNameAttribute(string v)
        {
            this.FieldName = v;
        }

        public string FieldName { get; }
    }

    /// <summary>
    /// Used to ignore X first rows from the original csv
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class CsvIgnoreTopAttribute : Attribute
    {
        public CsvIgnoreTopAttribute(uint v)
        {
            this.IgnoreTop = v;
        }

        public uint IgnoreTop { get; }
    }

    /// <summary>
    /// Used to ignore Proprty on Serialize/DeSerialize
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class CsvIgnoreAttribute : Attribute { }

    /// <summary>
    /// Used to ignore Proprty on Serialize/DeSerialize
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class CsvRequieredColumnAttribute : Attribute { }
}
