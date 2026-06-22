namespace Lox
{
    public abstract record Expr
    {
        public interface IVisitor<TOut>
        {
            TOut VisitBinaryExpr(Binary expr);
            TOut VisitGroupingExpr(Grouping expr);
            TOut VisitLiteralExpr(Literal expr);
            TOut VisitUnaryExpr(Unary expr);
        }
        public record Binary(Expr Left, Token Operator, Expr Right) : Expr
        {
            public override TOut Accept<TOut>(IVisitor<TOut> visitor) =>
                visitor.VisitBinaryExpr(this);
        }
        public record Grouping(Expr Expression) : Expr
        {
            public override TOut Accept<TOut>(IVisitor<TOut> visitor) =>
                visitor.VisitGroupingExpr(this);
        }
        public record Literal(object? Value) : Expr
        {
            public override TOut Accept<TOut>(IVisitor<TOut> visitor) =>
                visitor.VisitLiteralExpr(this);
        }
        public record Unary(Token Operator, Expr Right) : Expr
        {
            public override TOut Accept<TOut>(IVisitor<TOut> visitor) =>
                visitor.VisitUnaryExpr(this);
        }

        public abstract TOut Accept<TOut>(IVisitor<TOut> visitor);
    }
}
