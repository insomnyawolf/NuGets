using OauthClientBase;

namespace OsuApiHelper
{
    public class OsuApiConfig : OauthCredentials
    {
        public string Comment { get; set; } = "To obtain your key go to => https://osu.ppy.sh/home/account/edit#new-oauth-application";
    }

    // https://osu.ppy.sh/docs/index.html
    public class OsuApi : OauthClient, IDisposable
    {
        protected override string ApiUrl { get; } = "https://osu.ppy.sh/api/v2/";
        protected override string AuthUrl { get; } = "https://osu.ppy.sh/oauth/token";

        public OsuApi(OsuApiConfig OsuApiConfig) : base(OsuApiConfig)
        {

        }
        
        public async Task<Beatmap> BeatmapLookup(string checksum = null, string filename = null, string id = null)
        {
            var par = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(checksum))
            {
                par.Add("checksum", checksum);
            }
            if (!string.IsNullOrEmpty(filename))
            {
                par.Add("filename", filename);
            }
            if (!string.IsNullOrEmpty(id))
            {
                par.Add("id", id);
            }

            if (par.Count < 1)
            {
                throw new Exception("You must at least provide one parameter");
            }

            var data = await GetAsync<Beatmap>("beatmaps/lookup", par);

            return data;
        }
    }
}