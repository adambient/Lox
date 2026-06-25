using System.Text;

namespace Lox
{
    public class Lox : IErrorHandler
    {
        readonly ConsoleWriter console;
        readonly Interpreter interpreter;
        bool hadError;
        bool hadRuntimeException;

        public Lox(ConsoleWriter console)
        {
            this.console = console;
            interpreter = new Interpreter(console, this);
        }

        public int Run(string source)
        {
            var scanner = new Scanner(source, this);
            var tokens = scanner.ScanTokens();
            var parser = new Parser(tokens, this);
            var statements = parser.Parse();            
            if (hadError)
            {
                // stop if there was a syntax error
                return 65;
            }
            
            var resolver = new Resolver(interpreter, this);
            resolver.Resolve(statements);
            if (hadError)
            {
                // stop if there was a resolution error
                return 65;
            }

            interpreter.Interpret(statements);
            if (hadRuntimeException)
            {
                return 70;
            }

            return 0;
        }

        public void Error(int line, string message) =>
            Report(line, string.Empty, message);

        public void Error(Token token, string message)
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

        public void RuntimeException(RuntimeException exception)
        {
            console.StdErrorLn($"{exception.Message}\n[line {exception.Token.Line}]");
            hadRuntimeException = true;
        }
        
        void Report(int line, string where, string message)
        {
            console.StdErrorLn($"[line {line}] Error{where}:{message}");
            hadError = true;
        }
    }
}
