using System.Text;

namespace Lox
{
    public class AstPrinter : Expr.IVisitor<string>
    {
        public string Print(Expr expr) =>
            expr.Accept(this);

        string Expr.IVisitor<string>.VisitAssignExpr(Expr.Assign expr) =>
            Parenthisize(expr.Name.Lexeme, expr.Value);

        string Expr.IVisitor<string>.VisitBinaryExpr(Expr.Binary expr) =>
            Parenthisize(expr.Operator.Lexeme, expr.Left, expr.Right);

        string Expr.IVisitor<string>.VisitGroupingExpr(Expr.Grouping expr) =>
            Parenthisize("group", expr.Expr);

        string Expr.IVisitor<string>.VisitLiteralExpr(Expr.Literal expr) =>
            expr.Value?.ToString() ?? "nil";

        string Expr.IVisitor<string>.VisitUnaryExpr(Expr.Unary expr) =>
            Parenthisize(expr.Operator.Lexeme, expr.Right);

        string Expr.IVisitor<string>.VisitVariableExpr(Expr.Variable expr) =>
            expr.Name?.ToString() ?? "nil";

        string Parenthisize(string name, params Expr[] exprs)
        {
            var sb = new StringBuilder();
            sb.Append('(').Append(name);
            foreach (var expr in exprs)
            {
                sb.Append(' ');
                sb.Append(expr.Accept(this));
            }
            sb.Append(')');
            return sb.ToString();
        }

        
    }
}
