using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_invalid_subclass
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"
class Doughnut {
  cook() {
    print ""Fry until golden brown."";
  }
}

class BostonCream < {
";

        Because of = () =>
            lox.Run(source);

        It should_not_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(string.Empty);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual("[line 8] Error at '{':Expect superclass name.");
    }
}
