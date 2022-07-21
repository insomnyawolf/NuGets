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
    public class InMemoryDatabaseCompressed
    {
        public string FilePath { get; set; }
        public CsvToolsConfig Config { get; set; }

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

            //InMemoryDatabaseCompressedObj = new InMemoryDatabase<TestDataStructure>(FilePath + ".jsonc", true);
        }
    }
}