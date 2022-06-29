//#define renewData
using System;
using System.IO;
using System.Text;
using System.Threading;
using CsvToObjects;
using Extensions.Reflection;
using InMemoryDatabase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InMemoryDatabaseTest
{
    [TestClass]
    [DoNotParallelize]
    public class InMemoryDatabaseCommon
    {
        public string FilePath { get; set; }
        public CsvToolsConfig Config { get; set; }

        public InMemoryDatabase<TestDataStructure> InMemoryDatabase { get; set; }
        public InMemoryDatabase<TestDataStructure> InMemoryDatabaseCompressed { get; set; }

        [TestInitialize]
        [DoNotParallelize]
        public void SetupInit()
        {
            var rawPath = FilePath + ".json";
            var compressedPath = FilePath + ".jsonc";
#if renewData
            File.Delete(rawPath);
            File.Delete(compressedPath);
#endif
            // https://www.stats.govt.nz/large-datasets/csv-files-for-download/
            FilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data8277");

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

            InMemoryDatabase = new InMemoryDatabase<TestDataStructure>(rawPath);
            
            InMemoryDatabaseCompressed = new InMemoryDatabase<TestDataStructure>(compressedPath, true);
#if renewData
            CreateNewData();
#endif
        }


        public void CreateNewData()
        {
            var sucess = CsvTools.Deserialize<TestDataStructure>(File.OpenRead(FilePath + ".csv"), out var processed, Config);

            // if false "Something Failed"
            Assert.IsTrue(sucess);

            for (int i = 0; i < processed.Count; i++)
            {
                var current = processed[i];

                InMemoryDatabase.Add(current);
                InMemoryDatabaseCompressed.Add(current);
            }

            InMemoryDatabase.Save();
            InMemoryDatabaseCompressed.Save();
        }
    }

    [Serializable]
    public class TestDataStructure
    {
        // Year,Age,Ethnic,Sex,Area,count
        public int Year { get; set; }
        public int Age { get; set; }
        public int Ethnic { get; set; }
        public int Sex { get; set; }
        public string? Area { get; set; }
        public string? count { get; set; }
    }
}