using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_unexpected_character
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"// [line 3] Error: Unexpected character.
// [java line 3] Error at 'b': Expect ')' after arguments.
foo(a | b);
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual(
"[line 3] Error:Unexpected character.[line 3] Error at 'b':Expect ')' after arguments."
);
    }
}
