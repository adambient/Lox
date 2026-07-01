using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_empty_file
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
        {
            console.GetStdOut().ShouldEqual(string.Empty);
            console.GetStdErr().ShouldEqual(string.Empty);
        };
    }
}
