using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_string_unterminated
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"// [line 2] Error: Unterminated string.
""this string has no close quote";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual(
"[line 2] Error:Unterminated string."
);
    }
}
