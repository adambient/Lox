using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_operator_equals_class
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"// Bound methods have identity equality.
class Foo {}
class Bar {}

print Foo == Foo; // expect: true
print Foo == Bar; // expect: false
print Bar == Foo; // expect: false
print Bar == Bar; // expect: true

print Foo == ""Foo""; // expect: false
print Foo == nil;   // expect: false
print Foo == 123;   // expect: false
print Foo == true;  // expect: false
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"true\r\nfalse\r\nfalse\r\ntrue\r\nfalse\r\nfalse\r\nfalse\r\nfalse\r\n"
);
    }
}
