using static Lox.TokenTypeEnum;

namespace Lox
{
    public class Interpreter(ConsoleWriter console, IErrorHandler error) : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
    {
        Environment environment = new();

        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (var stmt in statements)
                {
                    Execute(stmt);
                }
            }
            catch (RuntimeException exception)
            {
                error.RuntimeException(exception);
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

        object? Expr.IVisitor<object?>.VisitAssignExpr(Expr.Assign expr)
        {
            var value = Evaluate(expr.Value);
            environment.Assign(expr.Name, value);
            return value;
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
            return Evaluate(expr.Expr);
        }

        object? Expr.IVisitor<object?>.VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        object? Expr.IVisitor<object?>.VisitLogicalExpr(Expr.Logical expr)
        {
            var left = Evaluate(expr.Left);

            if (expr.Operator.Type == OR)
            {
                if (IsTruthy(left))
                {
                    return left;
                }
            }
            else
            {
                if (!IsTruthy(left))
                {
                    return left;
                }
            }

            return Evaluate(expr.Right);
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

        object? Expr.IVisitor<object?>.VisitVariableExpr(Expr.Variable expr)
        {
            return environment.Get(expr.Name);
        }

        object? Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        void ExecuteBlock(List<Stmt> statements, Environment environment)
        {
            var previous = this.environment;
            try
            {
                this.environment = environment;
                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                this.environment = previous;
            }
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

        object? Stmt.IVisitor<object?>.VisitBlockStmt(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.Statements, new Environment(environment));
            return null;
        }

        object? Stmt.IVisitor<object?>.VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.Expr);
            return null;
        }

        object? Stmt.IVisitor<object?>.VisitIfStmt(Stmt.If stmt)
        {
            if (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.ThenBranch);
            }
            else if (stmt.ElseBranch != null)
            {
                Execute(stmt.ElseBranch);
            }
            return null;
        }

        object? Stmt.IVisitor<object?>.VisitPrintStmt(Stmt.Print stmt)
        {
            var value = Evaluate(stmt.Expr);
            console.StdOutLn(Stringify(value));
            return null;
        }

        object? Stmt.IVisitor<object?>.VisitVarStmt(Stmt.Var stmt)
        {
            object? value = null;
            if (stmt.Init != null)
            {
                value = Evaluate(stmt.Init);
            }

            environment.Define(stmt.Name.Lexeme, value);
            return null;
        }

        object? Stmt.IVisitor<object?>.VisitWhileStmt(Stmt.While stmt)
        {
            while (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.Body);
            }
            return null;
        }
    }
}
