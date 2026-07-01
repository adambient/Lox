using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_comments_only_line_comment_and_line
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"// comment
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
        {
            console.GetStdOut().ShouldEqual(string.Empty);
            console.GetStdErr().ShouldEqual(string.Empty);
        };
    }
}
