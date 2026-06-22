using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Parser))]
    public class when_parsing_simple_expression
    {
        static Parser parser;
        static Expr? expression;        

        Establish context = () =>
        {
            var scanner = new Scanner(@"6 / (3 - 1)");
            var tokens = scanner.ScanTokens();
            parser = new Parser(tokens);
        };

        Because of = () => expression = parser.Parse();

        It should_return_correct_expression = () =>
        {
            // use ast printer to test equivalence (I thought records could be tested like this but not working for some reason)
            var printer = new AstPrinter();
            var lhs = printer.Print(expression);
            var rhs = printer.Print(new Expr.Binary(
                    new Expr.Literal(6),
                    new Token(TokenTypeEnum.SLASH, "/"),
                    new Expr.Grouping(
                        new Expr.Binary(
                            new Expr.Literal(3),
                            new Token(TokenTypeEnum.MINUS, "-"),
                            new Expr.Literal(1)))));

            lhs.ShouldEqual(rhs);
        };
    }
}
