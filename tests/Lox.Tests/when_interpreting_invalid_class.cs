using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_invalid_class
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"
class Cake
  taste()
";

        Because of = () =>
            lox.Run(source);

        It should_not_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(string.Empty);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual("[line 3] Error at 'taste':Expect '{' before class body.");
    }
}
