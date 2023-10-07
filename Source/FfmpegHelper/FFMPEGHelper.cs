using CliArgumentBuilder;
using System.Text;

namespace FfmpegHelperHelper
{
    public class FFMPEGHelper : CommandLineArgumentBuilder
    {
        public string Output { get; set; }
        public void SetInput(string value) => AddWithValueEscaped("-i", value);
        public void SetOutput(string value) { Output = value; }

        public override string GetCommandLineArguments()
        {
            return base.GetCommandLineArguments() + ' ' + Output;
        }

        public string Run()
        {
            return base.GetCommandLineArguments() + ' ' + Output;
        }
    }
}