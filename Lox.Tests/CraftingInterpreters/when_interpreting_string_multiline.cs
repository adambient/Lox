using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_string_multiline
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = "var a = \"1\r\n2\r\n3\";\r\nprint a;\r\n// expect: 1\r\n// expect: 2\r\n// expect: 3";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"1\r\n2\r\n3\r\n"
);
    }
}
