using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MoeBooruApi
{
    public class MoeBooruRequester
    {
        private HttpClient HttpClient;

        public MoeBooruRequester() : this(new HttpClient())
        {

        }

        public MoeBooruRequester(HttpClient HttpClient)
        {
            this.HttpClient = HttpClient;
        }

        public async Task<List<MoeBooruAPIResponse>> GetResponse(HttpRequestMessage MoeboruRequest)
        {
            var response = await HttpClient.SendAsync(MoeboruRequest);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.ReasonPhrase, null, response.StatusCode);
            }

            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            return MoeBooruAPIResponse.FromJson(content);
        }
    }
}
