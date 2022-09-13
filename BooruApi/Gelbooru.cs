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

        public GelbooruApi(HttpClient? HttpClient = null) : base(HttpClient) { }
    }

    public class GelbooruPostQueryHelper : PostRequestHelper
    {
        new public virtual RatingGelbooru? Rating { get; set; }

        public override string ToString()
        {
            var query = base.ToString();

            if (Rating != null)
            {
                query += Rating;
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
            switch (PostRating)
            {
                case GelbooruPostRating.Explicit:
                    return "+rating:explicit";
                case GelbooruPostRating.Questionable:
                    return "+rating:questionable";
                case GelbooruPostRating.Sensitive:
                    return "+rating:sensitive";
                case GelbooruPostRating.General:
                    return "+rating:general";
                default:
                    return "";
            }
        }
    }

    public enum GelbooruPostRating
    {
        Any,
        General,
        Sensitive,
        Questionable,
        Explicit,
    }
}