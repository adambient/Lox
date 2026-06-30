namespace Lox
{
    public class Resolver(Interpreter interpreter, IErrorHandler error) : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
    {
        enum FunctionTypeEnum
        {
            NONE,
            FUNCTION,
            INITIALIZER,
            METHOD
        }

        enum ClassTypeEnum
        {
            NONE,
            CLASS,
            SUBCLASS
        }

        readonly Stack<Dictionary<string, bool>> scopes = new();
        FunctionTypeEnum currentFunction = FunctionTypeEnum.NONE;
        ClassTypeEnum currentClass = ClassTypeEnum.NONE;

        object? Expr.IVisitor<object?>.VisitAssignExpr(Expr.Assign expr)
        {
            Resolve(expr.Value);
            ResolveLocal(expr, expr.Name);
            return null;
        }

        object? Expr.IVisitor<object?>.VisitBinaryExpr(Expr.Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        object? Stmt.IVisitor<object?>.VisitBlockStmt(Stmt.Block stmt)
        {
            BeginScope();
            Resolve(stmt.Statements);
            EndScope();
            return null;
        }

        object? Stmt.IVisitor<object?>.VisitClassStmt(Stmt.Class stmt)
        {
            var enclosingClass = currentClass;
            currentClass = ClassTypeEnum.CLASS;

            Declare(stmt.Name);
            Define(stmt.Name);

            if (stmt.Superclass != null && stmt.Name.Lexeme == stmt.Superclass.Name.Lexeme)
            {
                error.Error(stmt.Superclass.Name, "A class can't inherit from itself.");
            }

            if (stmt.Superclass != null)
            {
                currentClass = ClassTypeEnum.SUBCLASS;
                Resolve(stmt.Superclass);
            }

            if (stmt.Superclass != null)
            {
                BeginScope();
                scopes.Peek()["super"] = true;
            }

            BeginScope();
            scopes.Peek()["this"] = true;

            foreach (var method in stmt.Methods)
            {
                var declaration = FunctionTypeEnum.METHOD;
                if (method.Name.Lexeme == "init")
                {
                    declaration = FunctionTypeEnum.INITIALIZER;
                }
                ResolveFunction(method, declaration);
            }

            EndScope();

            if (stmt.Superclass != null)
            {
                EndScope();
            }

            currentClass = enclosingClass;
            return null;
        }

        public void Resolve(List<Stmt> statements)
        {
            foreach (var statement in statements)
            {
                Resolve(statement);
            }
        }

        void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        void Resolve(Expr? expr)
        {
            expr?.Accept(this);
        }

        void ResolveFunction(Stmt.Function function, FunctionTypeEnum type)
        {
            var enclosingFunction = currentFunction;
            currentFunction = type;
            BeginScope();
            foreach (var param in function.Params)
            {
                Declare(param);
                Define(param);
            }

            Resolve(function.Body);
            EndScope();
            currentFunction = enclosingFunction;
        }

        void BeginScope() =>
            scopes.Push(new Dictionary<string, bool>());

        void EndScope() =>
            scopes.Pop();

        object? Expr.IVisitor<object?>.VisitCallExpr(Expr.Call expr)
        {
            Resolve(expr.Callee);
            foreach (var argument in expr.Arguments)
            {
                Resolve(argument);
            }

            return null;
        }

        object? Expr.IVisitor<object?>.VisitGetExpr(Expr.Get expr)
        {
            Resolve(expr.Obj);
            return null;
        }

        object? Stmt.IVisitor<object?>.VisitExpressionStmt(Stmt.Expression stmt)
        {
            Resolve(stmt.Expr);
            return null;
        }

        object? Stmt.IVisitor<object?>.VisitFunctionStmt(Stmt.Function stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);
            ResolveFunction(stmt, FunctionTypeEnum.FUNCTION);
            return null;
        }

        object? Expr.IVisitor<object?>.VisitGroupingExpr(Expr.Grouping expr)
        {
            Resolve(expr.Expr);
            return null;
        }

        object? Stmt.IVisitor<object?>.VisitIfStmt(Stmt.If stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.ThenBranch);
            if (stmt.ElseBranch != null)
            {
                Resolve(stmt.ElseBranch);
            }

            return null;
        }

        object? Expr.IVisitor<object?>.VisitLiteralExpr(Expr.Literal expr)
        {
            return null;
        }

        object? Expr.IVisitor<object?>.VisitLogicalExpr(Expr.Logical expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        object? Expr.IVisitor<object?>.VisitSetExpr(Expr.Set expr)
        {
            Resolve(expr.Value);
            Resolve(expr.Obj);
            return null;
        }

        object? Expr.IVisitor<object?>.VisitSuperExpr(Expr.Super expr)
        {
            if (currentClass == ClassTypeEnum.NONE)
            {
                error.Error(expr.Keyword, "Can't use 'super' outside of a class.");
            }
            else if (currentClass != ClassTypeEnum.SUBCLASS)
            {
                error.Error(expr.Keyword, "Can't use 'super' in a class with no superclass.");
            }

            ResolveLocal(expr, expr.Keyword);
            return null;
        }

        object? Expr.IVisitor<object?>.VisitThisExpr(Expr.This expr)
        {
            if (currentClass == ClassTypeEnum.NONE)
            {
                error.Error(expr.Keyword, "Can't use 'this' outside of a class.");
            }

            ResolveLocal(expr, expr.Keyword);
            return null;
        }

        object? Stmt.IVisitor<object?>.VisitPrintStmt(Stmt.Print stmt)
        {
            Resolve(stmt.Expr);
            return null;
        }

        object? Stmt.IVisitor<object?>.VisitReturnStmt(Stmt.Return stmt)
        {
            if (currentFunction == FunctionTypeEnum.NONE)
            {
                error.Error(stmt.Keyword, "Can't return from top-level code.");
            }

            if (stmt.Value != null)
            {
                if (currentFunction == FunctionTypeEnum.INITIALIZER)
                {
                    error.Error(stmt.Keyword, "Can't return a value from an initializer.");
                }

                Resolve(stmt.Value);
            }

            return null;
        }

        object? Expr.IVisitor<object?>.VisitUnaryExpr(Expr.Unary expr)
        {
            Resolve(expr.Right);
            return null;
        }

        object? Expr.IVisitor<object?>.VisitVariableExpr(Expr.Variable expr)
        {
            if (scopes.Count > 0 && scopes.Peek().TryGetValue(expr.Name.Lexeme, out bool value) && value == false)
            {
                error.Error(expr.Name, "Can't read local variable in its own initializer.");
            }

            ResolveLocal(expr, expr.Name);
            return null;
        }

        void ResolveLocal(Expr expr, Token name)
        {
            var scopesArray = scopes.ToArray();
            for (int i = scopesArray.Length - 1; i >= 0; i--)
            {
                if (scopesArray[i].ContainsKey(name.Lexeme))
                {
                    interpreter.Resolve(expr, i);
                    return;
                }
            }
        }

        object? Stmt.IVisitor<object?>.VisitVarStmt(Stmt.Var stmt)
        {
            Declare(stmt.Name);
            if (stmt.Init != null)
            {
                Resolve(stmt.Init);
            }

            Define(stmt.Name);
            return null;
        }

        void Declare(Token name)
        {
            if (scopes.Count == 0)
            {
                return;
            }

            var scope = scopes.Peek();
            if (scope.ContainsKey(name.Lexeme))
            {
                error.Error(name, "Already a variable with this name in this scope.");
            }

            scope[name.Lexeme] = false;
        }

        void Define(Token name)
        {
            if (scopes.Count == 0)
            {
                return;
            }

            scopes.Peek()[name.Lexeme] = true;
        }

        object? Stmt.IVisitor<object?>.VisitWhileStmt(Stmt.While stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.Body);
            return null;
        }
    }
}
