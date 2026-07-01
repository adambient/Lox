using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_constructor_default_arguments
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Foo {}

var foo = Foo(1, 2, 3); // expect runtime error: Expected 0 arguments but got 3.
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual(
"Expected 0 arguments but got 3.\n[line 3]"
);
    }
}
