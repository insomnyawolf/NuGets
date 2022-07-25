using OauthClientBase;

namespace OsuApiHelper
{
    public partial class OsuApi : OauthClient, IDisposable
    {
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

        public async Task<Beatmap> BeatmapsetDownload(string beatmapsetId)
        {
            var data = await GetAsync<Beatmap>($"beatmapsets/{beatmapsetId}/lookup");

            return data;
        }
    }
}