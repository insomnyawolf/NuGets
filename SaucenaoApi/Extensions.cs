using System.Collections.Generic;
using System.Linq;

namespace SaucenaoApi
{
    public static class SaucenaoResponseExtensions
    {
        public static SaucenaoResponse GetClosestMatch(this List<SaucenaoResponse> result)
        {
            if (result.Count == 0)
            {
                return null;
            }

            var maxSimilarity = result.Max(item => item.Similarity);

            return result.Find(item => item.Similarity == maxSimilarity);
        }
    }
}
