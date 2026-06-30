using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_invalid_for
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"
var a = 0;
var temp;

for (var b = 1; a < 10000 b = temp + b) {
";

        Because of = () =>
            lox.Run(source);

        It should_not_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(string.Empty);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual("[line 5] Error at 'b':Expect ';' after loop condition.");
    }
}
