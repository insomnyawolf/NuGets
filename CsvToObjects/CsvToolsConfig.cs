using ExtensionsReflection;
using System.Text;

namespace CsvToObjects
{
    public class CsvToolsConfig
    {
        /// <summary>
        /// Charactet that will be used to split each field in the input array
        /// </summary>
        public char SplitPattern { get; set; } = ';';
        public Encoding EncodingOrigin { get; set; } = null;
        public TypeConversionConfig TypeConversionConfig { get; set; } = new();
    }
}
