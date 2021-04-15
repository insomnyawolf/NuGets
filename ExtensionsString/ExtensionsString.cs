using System;

namespace ExtensionsString
{
    public static class ExtensionsString
    {
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

        public static bool EndsWithAny(this string current, string[] matches, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            foreach (string item in matches)
            {
                if (current.EndsWith(item, comparisonType))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool StartsWithAny(this string current, string[] matches, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            foreach (string item in matches)
            {
                if (current.StartsWith(item, comparisonType))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
