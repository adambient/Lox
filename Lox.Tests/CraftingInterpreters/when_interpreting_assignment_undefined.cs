using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_assignment_undefined
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"unknown = ""what""; // expect runtime error: Undefined variable 'unknown'.
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual(
"Undefined variable 'unknown'.\n[line 1]"
);
    }
}
