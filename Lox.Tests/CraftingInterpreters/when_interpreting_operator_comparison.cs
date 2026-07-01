using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_operator_comparison
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"print 1 < 2;    // expect: true
print 2 < 2;    // expect: false
print 2 < 1;    // expect: false

print 1 <= 2;    // expect: true
print 2 <= 2;    // expect: true
print 2 <= 1;    // expect: false

print 1 > 2;    // expect: false
print 2 > 2;    // expect: false
print 2 > 1;    // expect: true

print 1 >= 2;    // expect: false
print 2 >= 2;    // expect: true
print 2 >= 1;    // expect: true

// Zero and negative zero compare the same.
print 0 < -0; // expect: false
print -0 < 0; // expect: false
print 0 > -0; // expect: false
print -0 > 0; // expect: false
print 0 <= -0; // expect: true
print -0 <= 0; // expect: true
print 0 >= -0; // expect: true
print -0 >= 0; // expect: true
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"true\r\nfalse\r\nfalse\r\ntrue\r\ntrue\r\nfalse\r\nfalse\r\nfalse\r\ntrue\r\nfalse\r\ntrue\r\ntrue\r\nfalse\r\nfalse\r\nfalse\r\nfalse\r\ntrue\r\ntrue\r\ntrue\r\ntrue\r\n"
);
    }
}
