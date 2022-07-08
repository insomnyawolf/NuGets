using System;
using System.Text.Json;

namespace Extensions
{
    public static class ExtensionsString
    {
        public static bool EndsWithAny(this string current, string[] matches, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            for (int i = 0; i < matches.Length; i++)
            {
                if (current.EndsWith(matches[i], comparisonType))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool StartsWithAny(this string current, string[] matches, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            for (int i = 0; i < matches.Length; i++)
            {
                if (current.StartsWith(matches[i], comparisonType))
                {
                    return true;
                }
            }
            return false;
        }

        public static string RemovePrefix(this string current, string prefix)
        {
            if (!current.StartsWith(prefix))
            {
                return current;
            }
            return current[prefix.Length..];
        }

        public static string RemoveSuffix(this string current, string suffix)
        {
            if (!current.EndsWith(suffix))
            {
                return current;
            }
            return current[..suffix.Length];
        }

        public static bool StartsWith(this string current, string pattern, out string removedPattern)
        {
            if (!current.StartsWith(pattern))
            {
                removedPattern = null;
                return false;
            }

            removedPattern = current[pattern.Length..].Trim();
            return true;
        }

        public static string ToJsonString(this object data)
        {
            return JsonSerializer.Serialize(data, JsonSerializerOptionsCache);
        }

        private static JsonSerializerOptions JsonSerializerOptionsCache = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };
    }
}
