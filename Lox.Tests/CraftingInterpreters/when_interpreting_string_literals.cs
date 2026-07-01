using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_string_literals
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"print ""("" + """" + "")"";   // expect: ()
print ""a string""; // expect: a string

// Non-ASCII.
print ""A~¶Þॐஃ""; // expect: A~¶Þॐஃ
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"()\r\na string\r\nA~¶Þॐஃ\r\n"
);
    }
}
