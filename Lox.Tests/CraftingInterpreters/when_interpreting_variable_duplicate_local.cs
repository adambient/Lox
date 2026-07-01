using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_variable_duplicate_local
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"{
  var a = ""value"";
  var a = ""other""; // Error at 'a': Already a variable with this name in this scope.
}
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual(
"[line 3] Error at 'a':Already a variable with this name in this scope."
);
    }
}
