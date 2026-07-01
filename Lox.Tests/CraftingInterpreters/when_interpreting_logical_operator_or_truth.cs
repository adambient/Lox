using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_logical_operator_or_truth
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"// False and nil are false.
print false or ""ok""; // expect: ok
print nil or ""ok""; // expect: ok

// Everything else is true.
print true or ""ok""; // expect: true
print 0 or ""ok""; // expect: 0
print ""s"" or ""ok""; // expect: s
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"ok\r\nok\r\ntrue\r\n0\r\ns\r\n"
);
    }
}
