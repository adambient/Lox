using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_super_extra_arguments
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
    print ""Derived.foo()""; // expect: Derived.foo()
    super.foo(""a"", ""b"", ""c"", ""d""); // expect runtime error: Expected 2 arguments but got 4.
  }
}

Derived().foo();
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
        {
            console.GetStdOut().ShouldEqual(
"Derived.foo()\r\n"
);
            console.GetStdErr().ShouldEqual(
"Expected 2 arguments but got 4.\n[line 10]"
);
        };
    }
}
