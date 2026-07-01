using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_if_dangling_else
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"// A dangling else binds to the right-most if.
if (true) if (false) print ""bad""; else print ""good""; // expect: good
if (false) if (true) print ""bad""; else print ""bad"";
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"good\r\n"
);
    }
}
