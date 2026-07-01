using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_constructor_early_return
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Foo {
  init() {
    print ""init"";
    return;
    print ""nope"";
  }
}

var foo = Foo(); // expect: init
print foo; // expect: Foo instance
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"init\r\nFoo instance\r\n"
);
    }
}
