using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Extensions
{
    public static class Url
    {
        private static readonly Regex UrlRegex = new Regex("(https?:\\/\\/)?([\\w\\-])+\\.{1}([a-zA-Z]{2,63})([\\/\\w-]*)*\\/?\\??([^#\\n\\r]*)?#?([^\\n\\r]*)", RegexOptions.Compiled);

        public static List<string> FindUrls(this string input)
        {
            var results = new List<string>();

            var matches = UrlRegex.Matches(input);

            for (int index = 0; index < matches.Count; index++)
            {
                var match = matches[index];

                results.Add(match.Value);
            }

            return results;
        }
    }
}
