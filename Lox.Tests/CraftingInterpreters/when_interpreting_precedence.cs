using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_precedence
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"// * has higher precedence than +.
print 2 + 3 * 4; // expect: 14

// * has higher precedence than -.
print 20 - 3 * 4; // expect: 8

// / has higher precedence than +.
print 2 + 6 / 3; // expect: 4

// / has higher precedence than -.
print 2 - 6 / 3; // expect: 0

// < has higher precedence than ==.
print false == 2 < 1; // expect: true

// > has higher precedence than ==.
print false == 1 > 2; // expect: true

// <= has higher precedence than ==.
print false == 2 <= 1; // expect: true

// >= has higher precedence than ==.
print false == 1 >= 2; // expect: true

// 1 - 1 is not space-sensitive.
print 1 - 1; // expect: 0
print 1 -1;  // expect: 0
print 1- 1;  // expect: 0
print 1-1;   // expect: 0

// Using () for grouping.
print (2 * (6 - (2 + 2))); // expect: 4
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"14\r\n8\r\n4\r\n0\r\ntrue\r\ntrue\r\ntrue\r\ntrue\r\n0\r\n0\r\n0\r\n0\r\n4\r\n"
);
    }
}
