using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_this_closure
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Foo {
  getClosure() {
    fun closure() {
      return this.toString();
    }
    return closure;
  }

  toString() { return ""Foo""; }
}

var closure = Foo().getClosure();
print closure(); // expect: Foo
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"Foo\r\n"
);
    }
}
