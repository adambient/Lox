using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_simple_for
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;
        static string? stringValue;

        Establish context = () =>
        {
            source = @"
var a = 0;
var temp;

for (var b = 1; a < 10000; b = temp + b) {
  print a;
  temp = a;
  a = b;
}
";
        };

        Because of = () =>
        {
            lox.Run(source);
            stringValue = console.GetStdOut();
        };

        It should_return_correct_result = () =>
            stringValue.ShouldEqual(@"0
1
1
2
3
5
8
13
21
34
55
89
144
233
377
610
987
1597
2584
4181
6765
"); // console adds newline
    }
}
