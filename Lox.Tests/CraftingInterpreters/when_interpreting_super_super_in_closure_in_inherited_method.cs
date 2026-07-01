using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_super_super_in_closure_in_inherited_method
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"class A {
  say() {
    print ""A"";
  }
}

class B < A {
  getClosure() {
    fun closure() {
      super.say();
    }
    return closure;
  }

  say() {
    print ""B"";
  }
}

class C < B {
  say() {
    print ""C"";
  }
}

C().getClosure()(); // expect: A
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"A\r\n"
);
    }
}
