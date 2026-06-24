using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_simple_if
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;
        static string? stringValue;

        Establish context = () =>
        {
            source = @"
var a = 1;
if (a == 3)
{
  print ""fizz"";
}
else
{
  print ""buzz"";
}
";
        };

        Because of = () =>
        {
            lox.Run(source);
            stringValue = console.GetStdOut();
        };

        It should_return_correct_result = () =>
            stringValue.ShouldEqual("buzz\r\n"); // console adds newline
    }
}
