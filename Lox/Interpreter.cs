using static Lox.TokenTypeEnum;

namespace Lox
{
    public class Interpreter : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
    {
        readonly ConsoleWriter console;
        readonly IErrorHandler error;                
        readonly Dictionary<Expr, int> locals = new();
        Environment environment;

        public Environment Globals { get; }

        public Interpreter(ConsoleWriter console, IErrorHandler error)
        {
            this.console = console;
            this.error = error;
            Globals = new();
            environment = Globals;

            Globals.Define("clock", new ILoxCallable.Clock());
        }

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
            if (locals.ContainsKey(expr))
            {
                environment.AssignAt(locals[expr], expr.Name, value);
            }
            else
            {
                Globals.Assign(expr.Name, value);
            }

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

        object? Expr.IVisitor<object?>.VisitCallExpr(Expr.Call expr)
        {            
            var arguments = new List<object?>();
            foreach (var argument in expr.Arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            var callee = Evaluate(expr.Callee);
            if (callee is not ILoxCallable function)
            {
                throw new RuntimeException(expr.Paren, "Can only call functions and classes.");
            }

            if (arguments.Count != function.Arity())
            {
                throw new RuntimeException(expr.Paren, $"Expected {function.Arity()} arguments but got {arguments.Count}.");
            }

            return function.Call(this, arguments);
        }

        object? Expr.IVisitor<object?>.VisitGetExpr(Expr.Get expr)
        {
            var obj = Evaluate(expr.Obj);
            if (obj is LoxInstance loxInstance)
            {
                return loxInstance.Get(expr.Name);
            }

            throw new RuntimeException(expr.Name, "only instances have properties.");
        }

        object? Expr.IVisitor<object?>.VisitGroupingExpr(Expr.Grouping expr) =>
            Evaluate(expr.Expr);

        object? Expr.IVisitor<object?>.VisitLiteralExpr(Expr.Literal expr) =>
            expr.Value;

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

        object? Expr.IVisitor<object?>.VisitSetExpr(Expr.Set expr)
        {
            var obj = Evaluate(expr.Obj);
            if (obj is not LoxInstance loxInstance)
            {
                throw new RuntimeException(expr.Name, "Only instances have fields.");
            }

            var value = Evaluate(expr.Value);
            loxInstance.Set(expr.Name, value);
            return value;
        }

        object? Expr.IVisitor<object?>.VisitSuperExpr(Expr.Super expr)
        {
            int distance = locals[expr];
            var superclass = (LoxClass?)environment.GetAt(distance, "super");
            var obj = (LoxInstance?)environment.GetAt(distance - 1, "this");
            var method = superclass?.FindMethod(expr.Method.Lexeme);
            if (method == null)
            {
                throw new RuntimeException(expr.Method, $"Undefined property '{expr.Method.Lexeme}'.");
            }

            return method.Bind(obj);
        }

        object? Expr.IVisitor<object?>.VisitThisExpr(Expr.This expr) =>
            LookUpVariable(expr.Keyword, expr);

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

        object? Expr.IVisitor<object?>.VisitVariableExpr(Expr.Variable expr) =>
            LookUpVariable(expr.Name, expr);

        object? LookUpVariable(Token name, Expr expr)
        {
            if (locals.ContainsKey(expr))
            {
                return environment.GetAt(locals[expr], name.Lexeme);
            }
            else
            {
                return Globals.Get(name);
            }
        }

        object? Evaluate(Expr? expr) =>
            expr?.Accept(this);

        void Execute(Stmt stmt) =>
            stmt.Accept(this);

        public void Resolve(Expr expr, int depth) =>
            locals[expr] = depth;

        public void ExecuteBlock(List<Stmt> statements, Environment environment)
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

        object? Stmt.IVisitor<object?>.VisitClassStmt(Stmt.Class stmt)
        {
            object? superclass = null;
            if (stmt.Superclass != null)
            {
                superclass = Evaluate(stmt.Superclass);
                if (superclass is not LoxClass)
                {
                    throw new RuntimeException(stmt.Superclass.Name, "Superclass must be a class.");
                }
            }

            environment.Define(stmt.Name.Lexeme, null);
            if (stmt.Superclass != null)
            {
                environment = new Environment(environment);
                environment.Define("super", superclass);
            }

            var methods = new Dictionary<string, LoxFunction>();
            foreach (var method in stmt.Methods)
            {
                var function = new LoxFunction(method, environment, method.Name.Lexeme == "init");
                methods[method.Name.Lexeme] = function;
            }

            var klass = new LoxClass(stmt.Name.Lexeme, (LoxClass?)superclass, methods);
            if (superclass != null)
            {
                environment = environment.Enclosing!;
            }

            environment.Assign(stmt.Name, klass);
            return null;
        }

        object? Stmt.IVisitor<object?>.VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.Expr);
            return null;
        }

        object? Stmt.IVisitor<object?>.VisitFunctionStmt(Stmt.Function stmt)
        {
            var function = new LoxFunction(stmt, environment, false);
            environment.Define(stmt.Name.Lexeme, function);
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

        object? Stmt.IVisitor<object?>.VisitReturnStmt(Stmt.Return stmt)
        {
            object? value = null;
            if (stmt.Value != null)
            {
                value = Evaluate(stmt.Value);
            }

            throw new ReturnException(value);
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
