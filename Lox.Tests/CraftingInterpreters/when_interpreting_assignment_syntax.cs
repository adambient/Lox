using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_assignment_syntax
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"// Assignment on RHS of variable.
var a = ""before"";
var c = a = ""var"";
print a; // expect: var
print c; // expect: var
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"var\r\nvar\r\n"
);
    }
}
