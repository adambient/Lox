using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_super_no_superclass_method
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Base {}

class Derived < Base {
  foo() {
    super.doesNotExist(1); // expect runtime error: Undefined property 'doesNotExist'.
  }
}

Derived().foo();
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual(
"Undefined property 'doesNotExist'.\n[line 5]"
);
    }
}
