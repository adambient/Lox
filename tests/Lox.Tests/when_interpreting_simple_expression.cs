using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_simple_expression
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;
        static string? stringValue;

        Establish context = () =>
        {
            source = @"print 6 / (3 - 1);";
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
