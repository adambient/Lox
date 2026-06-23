using static Lox.TokenTypeEnum;

namespace Lox
{
    public class Interpreter : Expr.IVisitor<object?>
    {
        public string? Interpret(Expr expression)
        {
            try
            {
                var value = Evaluate(expression);
                var stringValue = Stringify(value);
                Console.Out.WriteLine(stringValue);
                return stringValue;
            }
            catch (RuntimeException exception)
            {
                Lox.RuntimeException(exception);
                return null;
            }
        }

        string? Stringify(object? value)
        {
            if (value == null)
            {
                return "nil";
            }

            if (value is double doubleValue)
            {
                var text = doubleValue.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            return value.ToString();
        }

        object? Expr.IVisitor<object?>.VisitBinaryExpr(Expr.Binary expr)
        {
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case GREATER:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double?)left > (double?)right;
                case GREATER_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double?)left >= (double?)right;
                case LESS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double?)left < (double?)right;
                case LESS_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double?)left <= (double?)right;
                case BANG_EQUAL:
                    return !IsEqual(left, right);
                case EQUAL_EQUAL:
                    return IsEqual(left, right);
                case MINUS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double?)left - (double?)right;
                case PLUS:
                    if (left is double doubleLeft && right is double doubleRight)
                    {
                        return doubleLeft + doubleRight;
                    }
                    if (left is string stringLeft && right is string stringRight)
                    {
                        return stringLeft + stringRight;
                    }

                    throw new RuntimeException(expr.Operator, "Operands must be two numbers or two strings.");
                case SLASH:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double?)left / (double?)right;
                case STAR:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double?)left * (double?)right;
            }

            // unreachable
            return null;
        }

        object? Expr.IVisitor<object?>.VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        object? Expr.IVisitor<object?>.VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        object? Expr.IVisitor<object?>.VisitUnaryExpr(Expr.Unary expr)
        {
            var right = Evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case BANG:
                    return !IsTruthy(right);
                case MINUS:
                    CheckNumberOperand(expr.Operator, right);
                    return -(double?)right;
            }

            // unreachable
            return null;
        }

        object? Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        bool IsTruthy(object? value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is bool boolValue)
            {
                return boolValue;
            }

            return true;
        }

        bool IsEqual(object? lhs, object? rhs)
        {
            if (lhs == null && rhs == null)
            {
                return true;
            }

            if (lhs == null)
            {
                return false;
            }

            return lhs.Equals(rhs);
        }

        void CheckNumberOperand(Token op, object? operand)
        {
            if (operand is double)
            {
                return;
            }

            throw new RuntimeException(op, "Operand must be a number.");
        }

        void CheckNumberOperands(Token op, object? lhs, object? rhs)
        {
            if (lhs is double && rhs is double)
            {
                return;
            }

            throw new RuntimeException(op, "Operands must be numbers.");
        }
    }
}
