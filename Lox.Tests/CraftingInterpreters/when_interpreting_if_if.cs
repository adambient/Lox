using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_if_if
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"// Evaluate the 'then' expression if the condition is true.
if (true) print ""good""; // expect: good
if (false) print ""bad"";

// Allow block body.
if (true) { print ""block""; } // expect: block

// Assignment in if condition.
var a = false;
if (a = true) print a; // expect: true
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"good\r\nblock\r\ntrue\r\n"
);
    }
}
