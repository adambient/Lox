using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_method_extra_arguments
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Foo {
  method(a, b) {
    print a;
    print b;
  }
}

Foo().method(1, 2, 3, 4); // expect runtime error: Expected 2 arguments but got 4.
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual(
"Expected 2 arguments but got 4.\n[line 8]"
);
    }
}
