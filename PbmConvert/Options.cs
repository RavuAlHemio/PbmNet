using CommandLine;
using CommandLine.Text;
using RavuAlHemio.PbmNet;

namespace PbmConvert
{
    class Options
    {
        [Option('t', "to-type", Required = true, HelpText = "The Netpbm image type into which to convert the given image.")]
        public ImageType ToType { get; set; }

        [Option('r', "read", Required = true, HelpText = "The filename of the Netpbm image to read.")]
        public string InputFilename { get; set; }

        [Option('o', "output", Required = true, HelpText = "The name of the file into which the converted Netpbm image will be stored.")]
        public string OutputFilename { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
