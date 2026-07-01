using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_super_no_superclass_call
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Base {
  foo() {
    super.doesNotExist(1); // Error at 'super': Can't use 'super' in a class with no superclass.
  }
}

Base().foo();
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual(
"[line 3] Error at 'super':Can't use 'super' in a class with no superclass."
);
    }
}
