﻿using System;

namespace Extensions.String
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
    }
}
