using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(AstPrinter))]
    public class when_printing_ast
    {
        static Expr expression = null!;
        static AstPrinter printer = null!;
        static string? output;

        Establish context = () =>
        {
            expression = new Expr.Binary(
                new Expr.Unary(
                    new Token(TokenTypeEnum.MINUS, "-", null, 1),
                    new Expr.Literal(123)),
                new Token(TokenTypeEnum.STAR, "*", null, 1),
                new Expr.Grouping(
                    new Expr.Literal(45.67)));
            printer = new AstPrinter();
        };

        Because of = () => output = printer.Print(expression);

        It should_print_as_expected = () =>
            output.ShouldEqual("(* (- 123) (group 45.67))");
    }
}
