using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_while_closure_in_body
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"var f1;
var f2;
var f3;

var i = 1;
while (i < 4) {
  var j = i;
  fun f() { print j; }

  if (j == 1) f1 = f;
  else if (j == 2) f2 = f;
  else f3 = f;

  i = i + 1;
}

f1(); // expect: 1
f2(); // expect: 2
f3(); // expect: 3
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"1\r\n2\r\n3\r\n"
);
    }
}
