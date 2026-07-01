using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_super_closure
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Base {
  toString() { return ""Base""; }
}

class Derived < Base {
  getClosure() {
    fun closure() {
      return super.toString();
    }
    return closure;
  }

  toString() { return ""Derived""; }
}

var closure = Derived().getClosure();
print closure(); // expect: Base
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"Base\r\n"
);
    }
}
