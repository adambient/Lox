using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_assignment_associativity
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"var a = ""a"";
var b = ""b"";
var c = ""c"";

// Assignment is right-associative.
a = b = c;
print a; // expect: c
print b; // expect: c
print c; // expect: c
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"c\r\nc\r\nc\r\n"
);
    }
}
