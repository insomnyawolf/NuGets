using MoeBooruApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            var p = new MoeBooruRequester();

            var request = new MoeBooruAPIRequest()
            {
                MaxResults = 1000,
                Random = true,
                Rating = MoeBooruRating.Any,
                Tags = new List<string>()
                {
                    "catgirl"
                }
            };

            var cachedRequest = request.GetRequest();

            var response = await p.GetResponse(cachedRequest);
        }
    }
}
