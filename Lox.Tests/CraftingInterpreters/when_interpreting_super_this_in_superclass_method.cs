using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_super_this_in_superclass_method
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Base {
  init(a) {
    this.a = a;
  }
}

class Derived < Base {
  init(a, b) {
    super.init(a);
    this.b = b;
  }
}

var derived = Derived(""a"", ""b"");
print derived.a; // expect: a
print derived.b; // expect: b
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"a\r\nb\r\n"
);
    }
}
