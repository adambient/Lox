using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_simple_block
    {
        static TestConsoleWriter console = new();
        static Interpreter interpreter = null!;
        static List<Stmt> statements = null!;
        static string? stringValue;

        Establish context = () =>
        {
            var scanner = new Scanner(@"
var a = 1;
{
  var a = a + 2;
  print a;
}
");
            var tokens = scanner.ScanTokens();
            var parser = new Parser(tokens);
            statements = parser.Parse();
            interpreter = new Interpreter(console);
        };

        Because of = () =>
        {
            interpreter.Interpret(statements);
            stringValue = console.GetStdOut();
        };

        It should_return_correct_result = () =>
            stringValue.ShouldEqual("3\r\n"); // console adds newline
    }
}
