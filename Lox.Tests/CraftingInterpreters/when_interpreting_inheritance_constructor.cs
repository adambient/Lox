using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_inheritance_constructor
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class A {
  init(param) {
    this.field = param;
  }

  test() {
    print this.field;
  }
}

class B < A {}

var b = B(""value"");
b.test(); // expect: value
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"value\r\n"
);
    }
}
