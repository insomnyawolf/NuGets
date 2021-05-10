using CsvToObjects;
using ExtensionsReflection;
using MoeBooruApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Test
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            //var p = new MoeBooruRequester();

            //var request = new MoeBooruAPIRequest()
            //{
            //    MaxResults = 1000,
            //    Random = true,
            //    Rating = MoeBooruRating.Any,
            //    Tags = new List<string>()
            //    {
            //        "catgirl"
            //    }
            //};

            //var cachedRequest = request.GetRequest();

            //var response = await p.GetResponse(cachedRequest);

            //var type = typeof(bool?);

            //object p = type.ConvertToCompatibleType("test");
            var file = File.OpenRead(@"C:\Users\iw\Desktop\Temp\report.csv");

            var config = new CsvToolsConfig()
            {
                SplitPattern = ',',
                TypeConversionConfig = new TypeConversionConfig()
                {
                    AcceptLossyConversion = false,
                    DateTimeFormat = "dd/MM/yyyy HH:mm:ss",
                    TimeSpanFormat = "c",
                }
            };

            var sucess = CsvTools.Deserialize<ScoreEntry>(file, out var processed, config);
            if (!sucess)
            {
                Console.WriteLine("Something Failed");
            }

            var serialized = CsvTools.Serialize(processed, config);

            Console.WriteLine(serialized);
        }

        [Serializable]
        public class ScoreEntry
        {
            [CsvName("Score")]
            public int Score { get; set; }
            [CsvName("time stamp")]
            public DateTime Timestamp { get; set; }
        }
    }
}
