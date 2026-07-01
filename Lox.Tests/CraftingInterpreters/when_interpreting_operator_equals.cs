using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_operator_equals
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"print nil == nil; // expect: true

print true == true; // expect: true
print true == false; // expect: false

print 1 == 1; // expect: true
print 1 == 2; // expect: false

print ""str"" == ""str""; // expect: true
print ""str"" == ""ing""; // expect: false

print nil == false; // expect: false
print false == 0; // expect: false
print 0 == ""0""; // expect: false
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"true\r\ntrue\r\nfalse\r\ntrue\r\nfalse\r\ntrue\r\nfalse\r\nfalse\r\nfalse\r\nfalse\r\n"
);
    }
}
