using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_logical_operator_or
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"// Note: These tests implicitly depend on ints being truthy.

// Return the first true argument.
print 1 or true; // expect: 1
print false or 1; // expect: 1
print false or false or true; // expect: true

// Return the last argument if all are false.
print false or false; // expect: false
print false or false or false; // expect: false

// Short-circuit at the first true argument.
var a = ""before"";
var b = ""before"";
(a = false) or
    (b = true) or
    (a = ""bad"");
print a; // expect: false
print b; // expect: true
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"1\r\n1\r\ntrue\r\nfalse\r\nfalse\r\nfalse\r\ntrue\r\n"
);
    }
}
