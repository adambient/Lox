using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_constructor_call_init_explicitly
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

var foo = Foo(""one""); // expect: Foo.init(one)
foo.field = ""field"";

var foo2 = foo.init(""two""); // expect: Foo.init(two)
print foo2; // expect: Foo instance

// Make sure init() doesn't create a fresh instance.
print foo.field; // expect: init
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"Foo.init(one)\r\nFoo.init(two)\r\nFoo instance\r\ninit\r\n"
);
    }
}
