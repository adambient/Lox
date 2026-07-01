using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_operator_not
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"print !true;     // expect: false
print !false;    // expect: true
print !!true;    // expect: true

print !123;      // expect: false
print !0;        // expect: false

print !nil;     // expect: true

print !"""";       // expect: false

fun foo() {}
print !foo;      // expect: false
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"false\r\ntrue\r\ntrue\r\nfalse\r\nfalse\r\ntrue\r\nfalse\r\nfalse\r\n"
);
    }
}
