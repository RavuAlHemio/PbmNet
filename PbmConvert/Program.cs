using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using RavuAlHemio.PbmNet;

namespace PbmConvert
{
    class Program
    {
        static int Main(string[] args)
        {
            ParserResult<Options> genericResult = Parser.Default.ParseArguments<Options>(args);
            var result = genericResult as Parsed<Options>;
            if (result == null)
            {
                return 1;
            }

            NetpbmImage8 image8;
            var reader = new NetpbmReader();
            var writer = new NetpbmWriter();
            var factory = new ImageFactories.Image8Factory();
            using (var inputStream = new FileStream(result.Value.InputFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                image8 = (NetpbmImage8)reader.ReadImage(inputStream, factory);
            }

            var supportedTypes = writer.SupportedTypesForImage(image8);
            if (!supportedTypes.Contains(result.Value.ToType))
            {
                Console.Error.WriteLine("The chosen image cannot be converted to the chosen type. Supported types for the image are:");
                foreach (var supportedType in new SortedSet<ImageType>(supportedTypes))
                {
                    Console.Error.WriteLine(supportedType);
                }
                Environment.Exit(1);
            }

            using (var outputStream = new FileStream(result.Value.OutputFilename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                writer.WriteImage(image8, outputStream, result.Value.ToType);
            }

            return 0;
        }
    }
}
