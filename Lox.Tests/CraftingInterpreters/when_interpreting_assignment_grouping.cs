using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_assignment_grouping
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"var a = ""a"";
(a) = ""value""; // Error at '=': Invalid assignment target.
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual(
"[line 2] Error at '=':Invalid assignment target."
);
    }
}
