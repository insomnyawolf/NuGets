using CsvToObjects;
using Extensions.Reflection;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace CsvToObjectsTest
{
    public class Tests
    {
        public string FilePath { get; set; }
        public CsvToolsConfig Config { get; set; }

        [SetUp]
        public void Setup()
        {
            FilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "Data7602DescendingYearOrder.csv");

            Config = new CsvToolsConfig()
            {
                SplitPattern = ',',
                EncodingOrigin = Encoding.UTF8,
                TypeConversionConfig = new TypeConversionConfig()
                {
                    AcceptLossyConversion = false,
                    DateTimeFormat = "dd/MM/yyyy HH:mm:ss",
                    TimeSpanFormat = "c",
                    CultureInfo = Thread.CurrentThread.CurrentCulture,
                    NumberStyles = System.Globalization.NumberStyles.Any,
                    DateTimeStyles = System.Globalization.DateTimeStyles.AllowWhiteSpaces,
                    TimeSpanStyles = System.Globalization.TimeSpanStyles.None
                },
            };
        }

        [Test]
        public void Test1()
        {
            var sucess = CsvTools.Deserialize<Entity>(File.OpenRead(FilePath), out var processed, Config);
            if (!sucess)
            {
                Console.WriteLine("Something Failed");
            }

            var serialized = CsvTools.Serialize(processed, Config);

            using var serializedReader = new StringReader(serialized);

            using var originReader = new StreamReader(File.OpenRead(FilePath));

            while (true)
            {
                var csvLine = serializedReader.ReadLine();
                var fileLine = originReader.ReadLine();

                if (csvLine != fileLine)
                {
                    Assert.Fail("Different Result Than Expected", csvLine, fileLine);
                    return;
                }

                if (csvLine is null)
                {
                    break;
                }
            }

            Assert.Pass();
        }

        [Serializable]
        public class Entity
        {
            //anzsic06,Area,year,geo_count,ec_count
            [CsvName("anzsic06")]
            public string Anzsic { get; set; }

            [CsvName("Area")]
            public string Area { get; set; }

            [CsvName("year")]
            public int Year { get; set; }

            [CsvName("geo_count")]
            public int GeoCount { get; set; }

            [CsvName("ec_count")]
            public int EcCount { get; set; }
        }
    }
}