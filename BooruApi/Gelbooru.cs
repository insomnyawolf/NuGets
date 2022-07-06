namespace BooruApi
{
    // https://gelbooru.com/index.php?page=wiki&s=list
    public class GelbooruApi : ApiBase<GelbooruPostQueryHelper>
    {
        public override string BaseUrl => "https://gelbooru.com/index.php";

        public override string PostApi => "?page=dapi&q=index&json=1&s=post";

        public override string PostPage => "?page=post&s=view&id=";

        public override string AutoComplete => "?page=autocomplete2&type=tag_query&term=";
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

    // do rating even work?
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