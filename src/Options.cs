using CommandLine;
namespace CheckUrls
{
    public class Options
    {
        [Option('f', "file", Required = true, HelpText = "Set input file with urls need check status")]
        public string FileName { get; set; }

        [Option('o', "out", Required = true, HelpText = "Set out file with result")]
        public string OutName { get; set; }

        [Option('v', "verbose", Default = false, Required = false, HelpText = "Set output to verbose messages")]
        public bool Verbose { get; set; }
    }
}
