using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_operator_add
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"print 123 + 456; // expect: 579
print ""str"" + ""ing""; // expect: string
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"579\r\nstring\r\n"
);
    }
}
