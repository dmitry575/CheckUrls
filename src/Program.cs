using CommandLine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;


namespace CheckUrls
{
    class Program
    {
        private static bool _hasError = false;
        static async Task Main(string[] args)
        {
            Console.WriteLine($"Start program checking urls");
            Console.WriteLine($"Version: {Assembly.GetExecutingAssembly().GetName().Version}");
            Console.WriteLine("");

            var options = new Options();
            var arguments = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(x => options = x)
                .WithNotParsed(HandleParseError);

            if (!_hasError)
            {

                var checkUrls = new CheckService(options);
                await checkUrls.CheckAsync();

                checkUrls.PrintResult();
            }
        }

        static void HandleParseError(IEnumerable<Error> errors)
        {

            foreach (var error in errors)
            {
                _hasError = true;
            }
        }

    }
}
