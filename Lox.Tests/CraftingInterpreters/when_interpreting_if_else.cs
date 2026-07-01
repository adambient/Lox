using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_if_else
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"// Evaluate the 'else' expression if the condition is false.
if (true) print ""good""; else print ""bad""; // expect: good
if (false) print ""bad""; else print ""good""; // expect: good

// Allow block body.
if (false) nil; else { print ""block""; } // expect: block
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"good\r\ngood\r\nblock\r\n"
);
    }
}
