using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_this_nested_class
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Outer {
  method() {
    print this; // expect: Outer instance

    fun f() {
      print this; // expect: Outer instance

      class Inner {
        method() {
          print this; // expect: Inner instance
        }
      }

      Inner().method();
    }
    f();
  }
}

Outer().method();
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"Outer instance\r\nOuter instance\r\nInner instance\r\n"
);
    }
}
