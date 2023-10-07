using System.Text;

namespace CliArgumentBuilder
{
    public abstract class CommandLineArgumentBuilder
    {
        public Dictionary<string, string?> Arguments { get; } = new Dictionary<string, string?>();

        public virtual void Add(string key)
        {
            Arguments.Add(key, null);
        }

        // Only the value is escaped
        public virtual void AddWithValueEscaped(string key, object value)
        {
            Arguments.Add(key, $@"""{value}""");
        }

        public virtual void AddWithValueRaw(string key, object value)
        {
            Arguments.Add(key, value.ToString());
        }

        public virtual void Remove(string key)
        {
            Arguments.Remove(key);
        }

        public virtual string GetCommandLineArguments()
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

        public override string ToString()
        {
            return GetCommandLineArguments();   
        }
    }
}
