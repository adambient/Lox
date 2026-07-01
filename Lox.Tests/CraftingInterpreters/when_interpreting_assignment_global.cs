using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_assignment_global
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"var a = ""before"";
print a; // expect: before

a = ""after"";
print a; // expect: after

print a = ""arg""; // expect: arg
print a; // expect: arg
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"before\r\nafter\r\narg\r\narg\r\n"
);
    }
}
