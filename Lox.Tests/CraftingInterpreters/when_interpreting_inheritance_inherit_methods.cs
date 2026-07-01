using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_inheritance_inherit_methods
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Foo {
  methodOnFoo() { print ""foo""; }
  override() { print ""foo""; }
}

class Bar < Foo {
  methodOnBar() { print ""bar""; }
  override() { print ""bar""; }
}

var bar = Bar();
bar.methodOnFoo(); // expect: foo
bar.methodOnBar(); // expect: bar
bar.override(); // expect: bar
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"foo\r\nbar\r\nbar\r\n"
);
    }
}
