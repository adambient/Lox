using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_constructor_arguments
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Foo {
  init(a, b) {
    print ""init""; // expect: init
    this.a = a;
    this.b = b;
  }
}

var foo = Foo(1, 2);
print foo.a; // expect: 1
print foo.b; // expect: 2
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"init\r\n1\r\n2\r\n"
);
    }
}
