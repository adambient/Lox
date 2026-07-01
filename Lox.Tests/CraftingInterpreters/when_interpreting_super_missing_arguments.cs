using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_super_missing_arguments
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Base {
  foo(a, b) {
    print ""Base.foo("" + a + "", "" + b + "")"";
  }
}

class Derived < Base {
  foo() {
    super.foo(1); // expect runtime error: Expected 2 arguments but got 1.
  }
}

Derived().foo();
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual(
"Expected 2 arguments but got 1.\n[line 9]"
);
    }
}
