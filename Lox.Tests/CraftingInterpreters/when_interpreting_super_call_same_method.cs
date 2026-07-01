using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_super_call_same_method
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Base {
  foo() {
    print ""Base.foo()"";
  }
}

class Derived < Base {
  foo() {
    print ""Derived.foo()"";
    super.foo();
  }
}

Derived().foo();
// expect: Derived.foo()
// expect: Base.foo()
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"Derived.foo()\r\nBase.foo()\r\n"
);
    }
}
