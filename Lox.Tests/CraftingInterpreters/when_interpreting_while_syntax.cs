using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_while_syntax
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"// Single-expression body.
var c = 0;
while (c < 3) print c = c + 1;
// expect: 1
// expect: 2
// expect: 3

// Block body.
var a = 0;
while (a < 3) {
  print a;
  a = a + 1;
}
// expect: 0
// expect: 1
// expect: 2

// Statement bodies.
while (false) if (true) 1; else 2;
while (false) while (true) 1;
while (false) for (;;) 1;
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"1\r\n2\r\n3\r\n0\r\n1\r\n2\r\n"
);
    }
}
