using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_constructor_return_in_nested_function
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Foo {
  init() {
    fun init() {
      return ""bar"";
    }
    print init(); // expect: bar
  }
}

print Foo(); // expect: Foo instance
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"bar\r\nFoo instance\r\n"
);
    }
}
