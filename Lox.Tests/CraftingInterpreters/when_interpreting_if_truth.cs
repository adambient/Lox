using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_if_truth
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"// False and nil are false.
if (false) print ""bad""; else print ""false""; // expect: false
if (nil) print ""bad""; else print ""nil""; // expect: nil

// Everything else is true.
if (true) print true; // expect: true
if (0) print 0; // expect: 0
if ("""") print ""empty""; // expect: empty
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"false\r\nnil\r\ntrue\r\n0\r\nempty\r\n"
);
    }
}
