using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_while_return_closure
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"fun f() {
  while (true) {
    var i = ""i"";
    fun g() { print i; }
    return g;
  }
}

var h = f();
h(); // expect: i
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"i\r\n"
);
    }
}
