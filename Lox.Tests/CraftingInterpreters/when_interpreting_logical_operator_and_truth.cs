using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_logical_operator_and_truth
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"// False and nil are false.
print false and ""bad""; // expect: false
print nil and ""bad""; // expect: nil

// Everything else is true.
print true and ""ok""; // expect: ok
print 0 and ""ok""; // expect: ok
print """" and ""ok""; // expect: ok
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"false\r\nnil\r\nok\r\nok\r\nok\r\n"
);
    }
}
