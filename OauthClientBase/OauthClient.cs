using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace OauthClientBase
{
    public class OauthCredentials
    {
        public string ClientId { get; set; }
        public string Secret { get; set; }
    }

    public abstract class OauthClient
    {
        protected abstract string ApiUrl { get; }
        protected abstract string AuthUrl { get; }

        protected HttpClient HttpClient { get; }

        private readonly OauthCredentials OauthCredentials;
        private string Token { get; set; }
        private static bool JustRefreshed = false;

        public OauthClient(OauthCredentials OauthCredentials, HttpClient HttpClient = null)
        {
            if (OauthCredentials is null)
            {
                throw new ArgumentNullException(nameof(OauthCredentials));
            }

            if (OauthCredentials is null)
            {
                this.HttpClient = new HttpClient();
            }

            if (string.IsNullOrEmpty(OauthCredentials.ClientId) && string.IsNullOrEmpty(OauthCredentials.Secret))
            {
                throw new InvalidDataException($"You must provide valid autentintication data in {nameof(OauthCredentials.ClientId)} and {nameof(OauthCredentials.Secret)}");
            }

            this.OauthCredentials = OauthCredentials;

            // By reusing the clients, you try to avoid socket exhaustion problems and cache problems
            // false here prevents disposing the handler, which should live for the duration of the program and be shared by all requests that use the same handler properties

            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpClient.DefaultRequestHeaders.Add("Client-ID", OauthCredentials.ClientId);
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("OauthClientC#");

            // Get the acess token for the first time
            RefreshCredentials().GetAwaiter().GetResult();
        }

        protected async Task<T> GetAsync<T>(string endpoint, Dictionary<string, object>? values = null)
        {
            // Format query parameters
            var requestUrl = $"{ApiUrl}{endpoint}";

            if (values?.Count > 0)
            {
                requestUrl += '?';

                foreach (var item in values)
                {
                    requestUrl += $"{item.Key}={HttpUtility.UrlEncode(item.Value?.ToString() ?? "")}";
                }
            }

        // Gotos are usually a bad idea, plz, dont overuse them
        retry:

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            // good idea to set to something reasonable

            var response = await HttpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized && !JustRefreshed && await RefreshCredentials())
                {
                    JustRefreshed = true;
                    // Gotos are usually a bad idea, plz, dont overuse them
                    goto retry;
                }

                Console.WriteLine($"ApiRequestError => {response.StatusCode} => {response.ReasonPhrase}");

                response.Dispose();

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException($"Failed to get valid credentials");
                }

                throw new Exception($"Api request failed\nQueryData: {requestUrl}\nError: {response.StatusCode} => {response.ReasonPhrase}");
            }

            JustRefreshed = false;

            //var data = await response.Content.ReadAsStreamAsync();
            var data = await response.Content.ReadAsStringAsync();

            var returnValue = JsonSerializer.Deserialize<T>(data);
            response.Dispose();

            return returnValue;
        }

        // Get's a new token if the currentone is not valid
        private async Task<bool> RefreshCredentials()
        {
            var content = new StringContent($"{{\"client_id\":{OauthCredentials.ClientId},\"client_secret\":\"{OauthCredentials.Secret}\",\"grant_type\":\"client_credentials\",\"scope\":\"public\"}}", System.Text.Encoding.UTF8, "application/json");
            using var response = await HttpClient.PostAsync(AuthUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var auth = JsonSerializer.Deserialize<AuthResponse>(await response.Content.ReadAsStreamAsync());
            Token = auth.access_token;

            return true;
        }

        // Destructor
        bool IsFinalized = false;

        ~OauthClient()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            if (!IsFinalized)
            {
                HttpClient.Dispose();
                IsFinalized = true;
                GC.SuppressFinalize(this);
            }
        }

#pragma warning disable IDE1006 // Estilos de nombres
        // Done it that way to preserve the api structure for now
#warning needs refactoring
        public class AuthResponse
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
        }
    }
}