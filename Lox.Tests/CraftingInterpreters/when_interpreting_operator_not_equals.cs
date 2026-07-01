using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_operator_not_equals
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"print nil != nil; // expect: false

print true != true; // expect: false
print true != false; // expect: true

print 1 != 1; // expect: false
print 1 != 2; // expect: true

print ""str"" != ""str""; // expect: false
print ""str"" != ""ing""; // expect: true

print nil != false; // expect: true
print false != 0; // expect: true
print 0 != ""0""; // expect: true
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"false\r\nfalse\r\ntrue\r\nfalse\r\ntrue\r\nfalse\r\ntrue\r\ntrue\r\ntrue\r\ntrue\r\n"
);
    }
}
