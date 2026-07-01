using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_method_missing_arguments
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Foo {
  method(a, b) {}
}

Foo().method(1); // expect runtime error: Expected 2 arguments but got 1.
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual(
"Expected 2 arguments but got 1.\n[line 5]"
);
    }
}
