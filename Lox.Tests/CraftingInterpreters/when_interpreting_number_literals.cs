using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_number_literals
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"print 123;     // expect: 123
print 987654;  // expect: 987654
print 0;       // expect: 0
print -0;      // expect: -0

print 123.456; // expect: 123.456
print -0.001;  // expect: -0.001
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"123\r\n987654\r\n0\r\n-0\r\n123.456\r\n-0.001\r\n"
);
    }
}
