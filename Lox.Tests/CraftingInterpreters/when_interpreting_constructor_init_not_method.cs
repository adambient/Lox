using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_constructor_init_not_method
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Foo {
  init(arg) {
    print ""Foo.init("" + arg + "")"";
    this.field = ""init"";
  }
}

fun init() {
  print ""not initializer"";
}

init(); // expect: not initializer
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"not initializer\r\n"
);
    }
}
