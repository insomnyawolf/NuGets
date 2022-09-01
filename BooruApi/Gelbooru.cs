using System.Collections.Generic;
using System.Net.Http;

namespace BooruApi
{
    // https://gelbooru.com/index.php?page=wiki&s=list
    public class GelbooruApi : ApiBase<GelbooruPostQueryHelper>
    {
        public override string BaseUrl => "https://gelbooru.com/index.php";

        public string CommonApiUrl => BaseUrl + "?page=dapi&q=index&json=1";

        public override string PostEndpoint => CommonApiUrl + "&s=post";

        public override string TagEndpoint => CommonApiUrl + "&s=tag";

        public override string PostPage => BaseUrl + "?page=post&s=view&id=";

        public override string AutoComplete => BaseUrl + "?page=autocomplete2&type=tag_query&term=";

        public GelbooruApi(HttpClient? HttpClient = null) : base (HttpClient) { }
    }

    public class GelbooruPostQueryHelper : PostRequestHelper
    {
        new public virtual IEnumerable<RatingGelbooru> Rating { get; set; }

        public override string ToString()
        {
            var query = base.ToString();

            if (Rating != null)
            {
                query += string.Join(string.Empty, Rating);
            }

            return query;
        }
    }

    // do exclude rating even work?
    public class RatingGelbooru : Rating
    {
        new public virtual GelbooruPostRating PostRating { get; set; }

        public override string ToString()
        {
            var rating = SearchType switch
            {
                SearchType.Include => "+",
                SearchType.Exclude => "-",
            };

            rating += "rating:";

            rating += PostRating switch
            {
                GelbooruPostRating.General => "general",
                GelbooruPostRating.Sensitive => "sensitive",
                GelbooruPostRating.Questionable => "questionable",
                GelbooruPostRating.Explicit => "explicit",
            };

            return rating;
        }
    }

    public enum GelbooruPostRating
    {
        General,
        Sensitive,
        Questionable,
        Explicit,
    }
}