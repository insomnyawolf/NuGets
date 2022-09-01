using OauthClientBase;
using System;

namespace OsuApiHelper
{
    public class OsuApiConfig : OauthCredentials
    {
        public string Comment { get; set; } = "To obtain your key go to => https://osu.ppy.sh/home/account/edit#new-oauth-application";
    }

    // https://osu.ppy.sh/docs/index.html
    public partial class OsuApi : OauthClient, IDisposable
    {
        protected override string ApiUrl { get; } = "https://osu.ppy.sh/api/v2/";
        protected override string AuthUrl { get; } = "https://osu.ppy.sh/oauth/token";

        public OsuApi(OsuApiConfig OsuApiConfig) : base(OsuApiConfig)
        {

        }
    }
}