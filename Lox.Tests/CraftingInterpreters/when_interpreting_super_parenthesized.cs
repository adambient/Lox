using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_super_parenthesized
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class A {
  method() {}
}

class B < A {
  method() {
    // [line 8] Error at ')': Expect '.' after 'super'.
    (super).method();
  }
}
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual(
"[line 8] Error at ')':Expect '.' after 'super'."
);
    }
}
