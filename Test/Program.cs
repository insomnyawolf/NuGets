//#define LoadNewData
using CsvToObjects;
using ReflectionExtensions;
using MoeBooruApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MathHelpers;
using InMemoryDatabase;

namespace Test
{
    public static class Program
    {
        // Region,Country,Item Type,Sales Channel,Order Priority,Order Date,Order ID,Ship Date,Units Sold,Unit Price,Unit Cost,Total Revenue,Total Cost,Total Profit
        [Serializable]
        class DataStructure
        {
            [CsvName("Region")]
            public string Region { get; set; }

            [CsvName("Country")]
            public string Country { get; set; }

            [CsvName("Item Type")]
            public string ItemType { get; set; }

            [CsvName("Sales Channel")]
            public string SalesChannel { get; set; }

            [CsvName("Order Priority")]
            public string OrderPriority { get; set; }

            [CsvName("Order Date")]
            public DateTime OrderDate { get; set; }

            [CsvName("Order ID")]

            public ulong OrderID { get; set; }

            [CsvName("Ship Date")]
            public DateTime ShipDate { get; set; }

            [CsvName("Units Sold")]
            public ulong UnitsSold { get; set; }

            [CsvName("Unit Price")]
            public decimal UnitPrice { get; set; }

            [CsvName("Unit Cost")]
            public decimal UnitCost { get; set; }

            [CsvName("Total Revenue")]
            public decimal TotalRevenue { get; set; }

            [CsvName("Total Cost")]
            public decimal TotalCost { get; set; }

            [CsvName("Total Profit")]
            public decimal TotalProfit { get; set; }
        }

        static async Task Main(string[] args)
        {
#if LoadNewData
            var CsvConfig = new CsvToolsConfig()
            {
                SplitPattern = ',',
                EncodingOrigin = Encoding.UTF8,
                TypeConversionConfig = new TypeConversionConfig()
                {
                    AcceptLossyConversion = false,
                    DateTimeFormat = "M/d/yyyy",
                    TimeSpanFormat = "c",
                    CultureInfo = Thread.CurrentThread.CurrentCulture,
                    NumberStyles = System.Globalization.NumberStyles.Any,
                    DateTimeStyles = System.Globalization.DateTimeStyles.AllowWhiteSpaces,
                    TimeSpanStyles = System.Globalization.TimeSpanStyles.None
                },
            };

            var sucess = CsvTools.Deserialize<DataStructure>(File.OpenRead(@"C:\Users\iw\Downloads\TestCsv\5m Sales Records.csv"), out var processed, CsvConfig);
            if (!sucess)
            {
                Console.WriteLine("Something Failed");
            }
#endif

            var test = new InMemoryDatabase<DataStructure>(@"C:\Users\iw\Downloads\TestCsv\DatabaseTest.json");

#if LoadNewData
            test.Add(processed);
#endif

            var frutas = test.Find(item => 
                item.OrderPriority == "C" && 
                item.ItemType == "Fruits" && 
                item.SalesChannel == "Offline" && 
                item.Region == "Europe" && 
                item.UnitsSold > 2317 &&
                item.Country == "Belgium"
                );

            //test.Save();

            Console.WriteLine("");

            //RollingAverage potato = new RollingAverage(5);

            //while (true)
            //{
            //    var read = Console.ReadLine();

            //    var number = Convert.ToDouble(read);

            //    var average = potato.Next(number);

            //    Console.WriteLine($"NewAverage => '{average}'");
            //}

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

            //var listUsuarios = new List<Usuario>()
            //{
            //    new Usuario()
            //    {
            //        NombrePeroRaroParaEnseñarElEjemplo = "Pepito",
            //        Edad = 18,
            //        DatoInutil = "asdasdasd"
            //    },
            //    new Usuario()
            //    {
            //        NombrePeroRaroParaEnseñarElEjemplo = "Juan",
            //        Edad = 20,
            //        DatoInutil = "fwefwefwef"
            //    },
            //};

            //Migrate(listUsuarios, item => new MigrateQuery()
            //{
            //    UserName = item.NombrePeroRaroParaEnseñarElEjemplo,
            //    Edad = item.Edad,
            //});
        }

        public static void Migrate<T>(List<T> usuarios, Func<T, MigrateQuery> mapeo)
        {
            foreach (var item in usuarios)
            {
                var usuarioMigracion = mapeo(item);

                Console.WriteLine($"Migrado correctamente el usuario {usuarioMigracion.UserName} {usuarioMigracion.Edad}");
            }

        }

        public class Usuario
        {
            public string NombrePeroRaroParaEnseñarElEjemplo { get; set; }
            public int Edad { get; set; }
            public string DatoInutil { get; set; }
        }

        public class MigrateQuery
        {
            public string UserName { get; set; }
            public int Edad { get; set; }
        }
    }
}
