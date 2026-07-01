using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_super_constructor
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Base {
  init(a, b) {
    print ""Base.init("" + a + "", "" + b + "")"";
  }
}

class Derived < Base {
  init() {
    print ""Derived.init()"";
    super.init(""a"", ""b"");
  }
}

Derived();
// expect: Derived.init()
// expect: Base.init(a, b)
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"Derived.init()\r\nBase.init(a, b)\r\n"
);
    }
}
