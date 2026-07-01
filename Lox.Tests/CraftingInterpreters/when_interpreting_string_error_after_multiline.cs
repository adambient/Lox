using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_string_error_after_multiline
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"// Tests that we correctly track the line info across multiline strings.
var a = ""1
2
3
"";

err; // // expect runtime error: Undefined variable 'err'.";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual(
"Undefined variable 'err'.\n[line 7]"
);
    }
}
