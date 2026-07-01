using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_super_reassign_superclass
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class Base {
  method() {
    print ""Base.method()"";
  }
}

class Derived < Base {
  method() {
    super.method();
  }
}

class OtherBase {
  method() {
    print ""OtherBase.method()"";
  }
}

var derived = Derived();
derived.method(); // expect: Base.method()
Base = OtherBase;
derived.method(); // expect: Base.method()
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"Base.method()\r\nBase.method()\r\n"
);
    }
}
