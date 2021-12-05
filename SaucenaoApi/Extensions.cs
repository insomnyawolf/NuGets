using System.Linq;

namespace SaucenaoApi
{
    public static class SaucenaoResponseExtensions
    {
        public static Result GetClosestMatch(this SaucenaoResponse result)
        {
            if (result.Results.Count == 0)
            {
                return null;
            }

            var maxSimilarity = result.Results.Max(item => item.Header.Similarity);

            return result.Results.Find(item => item.Header.Similarity == maxSimilarity);
        }
    }
}
