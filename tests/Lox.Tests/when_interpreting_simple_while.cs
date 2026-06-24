using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_simple_while
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;
        static string? stringValue;

        Establish context = () =>
        {
            source = @"
var a = 1;
while (a < 3)
{
  a = a + 1;
}
print a;
";
        };

        Because of = () =>
        {
            lox.Run(source);
            stringValue = console.GetStdOut();
        };

        It should_return_correct_result = () =>
            stringValue.ShouldEqual("3\r\n"); // console adds newline
    }
}
