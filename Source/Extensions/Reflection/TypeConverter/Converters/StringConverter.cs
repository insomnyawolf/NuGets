using System;

namespace Extensions.Reflection.Converters
{
    public class StringConverter : TypeConverterBase<string>
    {
        public override bool Convert(string value, out string result, TypeConverterSettings typeConverterSettings = null)
        {
            // if that fails i guess i quit coding
            result = value;
            return true;
        }
    }

    public class GuidConverter : TypeConverterBase<Guid>
    {
        public override bool Convert(string value, out Guid result, TypeConverterSettings typeConverterSettings = null)
        {
            return Guid.TryParse(value, out result);
        }
    }

    public class UriConverter : TypeConverterBase<Uri>
    {
        public override bool Convert(string value, out Uri result, TypeConverterSettings typeConverterSettings = null)
        {
            if (value.StartsWith("<") && value.EndsWith(">"))
            {
                value = value.Substring(1, value.Length - 2); ;
            }

            result = new Uri(value);

            return true;
        }
    }
}
