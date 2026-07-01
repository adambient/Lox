using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_super_indirectly_inherited
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class A {
  foo() {
    print ""A.foo()"";
  }
}

class B < A {}

class C < B {
  foo() {
    print ""C.foo()"";
    super.foo();
  }
}

C().foo();
// expect: C.foo()
// expect: A.foo()
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"C.foo()\r\nA.foo()\r\n"
);
    }
}
