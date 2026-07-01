using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_inheritance_set_fields_from_base_class
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Foo {
  foo(a, b) {
    this.field1 = a;
    this.field2 = b;
  }

  fooPrint() {
    print this.field1;
    print this.field2;
  }
}

class Bar < Foo {
  bar(a, b) {
    this.field1 = a;
    this.field2 = b;
  }

  barPrint() {
    print this.field1;
    print this.field2;
  }
}

var bar = Bar();
bar.foo(""foo 1"", ""foo 2"");
bar.fooPrint();
// expect: foo 1
// expect: foo 2

bar.bar(""bar 1"", ""bar 2"");
bar.barPrint();
// expect: bar 1
// expect: bar 2

bar.fooPrint();
// expect: bar 1
// expect: bar 2
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"foo 1\r\nfoo 2\r\nbar 1\r\nbar 2\r\nbar 1\r\nbar 2\r\n"
);
    }
}
