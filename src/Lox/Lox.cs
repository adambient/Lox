using System.Text;

namespace Lox
{
    internal class Lox
    {
        async static Task<int> Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: jlox [script]"); // TODO - dotnet equiv
                return 64;
            }
            else if (args.Length == 1)
            {
                await RunFileAsync(args[0], default);
            }
            else
            {
                RunPrompt();
            }

            if (hadError)
            {
                return 65;
            }

            return 0;
        }

        static async Task RunFileAsync(string path, CancellationToken cancellationToken) =>
            Run(await File.ReadAllTextAsync(path, Encoding.Default, cancellationToken));

        static void RunPrompt()
        {
            while(true)
            {
                Console.Out.Write("> ");
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }

                Run(line);
            }
        }

        static void Run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            // for now just print the tokens
            foreach (var token in tokens)
            {
                Console.Out.WriteLine(token);
            }
        }

        public static void Error(int line, string message) =>
            Report(line, string.Empty, message);

        static bool hadError;
        static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error{where}:{message}");
            hadError = true;
        }
    }
}
