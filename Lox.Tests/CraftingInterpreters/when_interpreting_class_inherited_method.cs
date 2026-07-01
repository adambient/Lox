using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_class_inherited_method
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Foo {
  inFoo() {
    print ""in foo"";
  }
}

class Bar < Foo {
  inBar() {
    print ""in bar"";
  }
}

class Baz < Bar {
  inBaz() {
    print ""in baz"";
  }
}

var baz = Baz();
baz.inFoo(); // expect: in foo
baz.inBar(); // expect: in bar
baz.inBaz(); // expect: in baz
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"in foo\r\nin bar\r\nin baz\r\n"
);
    }
}
