using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_assignment_to_this
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Foo {
  Foo() {
    this = ""value""; // Error at '=': Invalid assignment target.
  }
}

Foo();
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual(
"[line 3] Error at '=':Invalid assignment target."
);
    }
}
