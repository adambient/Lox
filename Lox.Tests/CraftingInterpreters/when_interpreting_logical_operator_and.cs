using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_logical_operator_and
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"// Note: These tests implicitly depend on ints being truthy.

// Return the first non-true argument.
print false and 1; // expect: false
print true and 1; // expect: 1
print 1 and 2 and false; // expect: false

// Return the last argument if all are true.
print 1 and true; // expect: true
print 1 and 2 and 3; // expect: 3

// Short-circuit at the first false argument.
var a = ""before"";
var b = ""before"";
(a = true) and
    (b = false) and
    (a = ""bad"");
print a; // expect: true
print b; // expect: false
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"false\r\n1\r\nfalse\r\ntrue\r\n3\r\ntrue\r\nfalse\r\n"
);
    }
}
