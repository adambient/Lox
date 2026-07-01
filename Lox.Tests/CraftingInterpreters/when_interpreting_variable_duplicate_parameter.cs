using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_variable_duplicate_parameter
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"fun foo(arg,
        arg) { // Error at 'arg': Already a variable with this name in this scope.
  ""body"";
}
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual(
"[line 2] Error at 'arg':Already a variable with this name in this scope."
);
    }
}
