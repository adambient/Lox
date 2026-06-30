using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_invalid_expression
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"
print 6 / (3 - 1;
";

        Because of = () =>
            lox.Run(source);

        It should_not_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(string.Empty);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual("[line 2] Error at ';':Expect ')' after expression.");
    }
}
