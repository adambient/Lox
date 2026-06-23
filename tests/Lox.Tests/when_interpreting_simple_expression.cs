using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_simple_expression
    {
        static Interpreter interpreter;
        static Expr expression;
        static string? stringValue;

        Establish context = () =>
        {
            var scanner = new Scanner(@"6 / (3 - 1)");
            var tokens = scanner.ScanTokens();
            var parser = new Parser(tokens);
            expression = parser.Parse();
            interpreter = new Interpreter();
        };

        Because of = () => stringValue = interpreter.Interpret(expression);

        It should_return_correct_result = () =>
            stringValue.ShouldEqual("3");
    }
}
