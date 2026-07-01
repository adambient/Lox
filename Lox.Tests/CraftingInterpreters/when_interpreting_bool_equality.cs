using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_bool_equality
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"print true == true;    // expect: true
print true == false;   // expect: false
print false == true;   // expect: false
print false == false;  // expect: true

// Not equal to other types.
print true == 1;        // expect: false
print false == 0;       // expect: false
print true == ""true"";   // expect: false
print false == ""false""; // expect: false
print false == """";      // expect: false

print true != true;    // expect: false
print true != false;   // expect: true
print false != true;   // expect: true
print false != false;  // expect: false

// Not equal to other types.
print true != 1;        // expect: true
print false != 0;       // expect: true
print true != ""true"";   // expect: true
print false != ""false""; // expect: true
print false != """";      // expect: true
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"true\r\nfalse\r\nfalse\r\ntrue\r\nfalse\r\nfalse\r\nfalse\r\nfalse\r\nfalse\r\nfalse\r\ntrue\r\ntrue\r\nfalse\r\ntrue\r\ntrue\r\ntrue\r\ntrue\r\ntrue\r\n"
);
    }
}
