using System.Text;

namespace Lox
{
    internal class Program
    {
        async static Task<int> Main(string[] args)
        {
            var console = new ConsoleWriter();
            var lox = new Lox(console);
            if (args.Length > 1)
            {
                console.StdOutLn("Usage: jlox [script]"); // TODO - dotnet equiv
                return 64;
            }
            else if (args.Length == 1)
            {
                var source = await File.ReadAllTextAsync(args[0], Encoding.Default, default);
                return lox.Run(source);
            }
            else
            {
                while (true)
                {
                    console.StdOut("> ");
                    var line = Console.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        break;
                    }

                    lox.Run(line);
                }
            }


            return 0;
        }
    }
}
