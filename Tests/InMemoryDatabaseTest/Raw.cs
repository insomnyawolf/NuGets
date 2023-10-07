#define renewData
using System;
using System.IO;
using System.Text;
using System.Threading;
using CsvToObjects;
using Extensions;
using InMemoryDatabase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InMemoryDatabaseTest
{
    [TestClass]
    [DoNotParallelize]
    public class InMemoryDatabaseRaw
    {
        public string FilePath { get; set; }
        public CsvToolsConfig Config { get; set; }

        public InMemoryDatabase<TestDataStructure> InMemoryDatabase { get; set; }

        [TestInitialize]
        [DoNotParallelize]
        public void SetupInit()
        {
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
                    IFormatProvider = Thread.CurrentThread.CurrentCulture,
                    NumberStyles = System.Globalization.NumberStyles.Any,
                    DateTimeStyles = System.Globalization.DateTimeStyles.AllowWhiteSpaces,
                    TimeSpanStyles = System.Globalization.TimeSpanStyles.None
                },
            };

            InMemoryDatabase = new InMemoryDatabase<TestDataStructure>(FilePath + ".json");
        }

        [TestMethod]
        public void Count()
        {
            // 380ms
            Console.WriteLine(InMemoryDatabase.CountWhere((_) => true));
        }

        [TestMethod]
        public void CountMulti()
        {
            // 156ms
            //Console.WriteLine(InMemoryDatabase.CountWhereMultiThread((_) => true));
        }
    }
}