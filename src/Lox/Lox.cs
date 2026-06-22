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
            var parser = new Parser(tokens);
            var expression = parser.Parse();

            // stop if there was a syntax error
            if (hadError)
            {
                return;
            }

            Console.Out.WriteLine(new AstPrinter().Print(expression));
        }

        public static void Error(int line, string message) =>
            Report(line, string.Empty, message);

        public static void Error(Token token, string message)
        {
            if (token.Type == TokenTypeEnum.EOF)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, $" at '{token.Lexeme}'", message);
            }
        }

        static bool hadError;
        static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error{where}:{message}");
            hadError = true;
        }
    }
}
