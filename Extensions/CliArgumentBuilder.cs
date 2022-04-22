using System.Collections.Generic;
using System.Text;

namespace Extensions.CliArgumentBuilder
{
    public abstract class ExtensionsCliArgumentBuilder
    {
        public Dictionary<string, string?> Arguments { get; } = new();

        public virtual void AddEscaped(string key)
        {
            Arguments.Add(key, null);
        }

        // Only the value is escaped
        public virtual void AddEscaped(string key, string value)
        {
            Arguments.Add(key, $@"""{value}""");
        }

        // Only the value is escaped
        public virtual void AddEscaped(string key, object value)
        {
            Arguments.Add(key, $@"""{value.ToString()}""");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var arg in Arguments)
            {
                sb.Append(arg.Key);
                sb.Append(' ');

                if (!string.IsNullOrEmpty(arg.Value))
                {
                    sb.Append(arg.Value);
                    sb.Append(' ');
                }
            }

            return sb.ToString();
        }
    }
}
