using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_super_super_without_dot
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class A {}

class B < A {
  method() {
    // [line 6] Error at ';': Expect '.' after 'super'.
    super;
  }
}
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual(
"[line 6] Error at ';':Expect '.' after 'super'."
);
    }
}
