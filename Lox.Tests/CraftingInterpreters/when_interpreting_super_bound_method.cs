using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_super_bound_method
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class A {
  method(arg) {
    print ""A.method("" + arg + "")"";
  }
}

class B < A {
  getClosure() {
    return super.method;
  }

  method(arg) {
    print ""B.method("" + arg + "")"";
  }
}


var closure = B().getClosure();
closure(""arg""); // expect: A.method(arg)
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"A.method(arg)\r\n"
);
    }
}
