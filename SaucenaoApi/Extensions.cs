using System.Collections.Generic;
using System.Linq;

namespace SaucenaoSearch
{
    public static class SaucenaoResponseExtensions
    {
        public static SaucenaoResponse GetClosestMatch(this IList<SaucenaoResponse> result)
        {
            if (result.Count == 0)
            {
                return null;
            }

            var closestMatch = result[0];

            for (int index = 1; index < result.Count; index++)
            {
                var current = result[index];
                if (current.Similarity > closestMatch.Similarity)
                {
                    closestMatch = current;
                }
            }

            return closestMatch;
        }
    }
}
