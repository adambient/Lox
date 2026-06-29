using static Lox.TokenTypeEnum;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lox
{
    public class Parser(List<Token> tokens, IErrorHandler error)
    {
        class ParseException : ApplicationException { }

        int current = 0;

        public List<Stmt> Parse()
        {
            var statements = new List<Stmt>();
            while (!IsAtEnd())
            {
                var declaration = Declaration();
                if (declaration != null)
                {
                    statements.Add(declaration);
                }
            }

            return statements;
        }

        Expr Expression()
        {
            return Assignment();
        }

        Stmt? Declaration()
        {
            try
            {
                if (Match(CLASS))
                {
                    return ClassDeclaration();
                }

                if (Match(FUN))
                {
                    return Function("function");
                }

                if (Match(VAR))
                {
                    return VarDeclaration();
                }

                return Statement();
            }
            catch (ParseException)
            {
                Synchronize();
                return null;
            }
        }

        Stmt ClassDeclaration()
        {
            var name = Consume(IDENTIFIER, "Expect class name.");
            Consume(LEFT_BRACE, "Expect '{' before class body.");

            var methods = new List<Stmt.Function>();
            while (!Check(RIGHT_BRACE) && !IsAtEnd())
            {
                methods.Add(Function("method"));
            }

            Consume(RIGHT_BRACE, "Expect '}' after class body.");

            return new Stmt.Class(name, methods);
        }

        Stmt Statement()
        {
            if (Match(FOR))
            {
                return ForStatement();
            }

            if (Match(IF))
            {
                return IfStatement();
            }

            if (Match(PRINT))
            {
                return PrintStatement();
            }

            if (Match(RETURN))
            {
                return ReturnStatement();
            }

            if (Match(WHILE))
            {
                return WhileStatement();
            }

            if (Match(LEFT_BRACE))
            {
                return new Stmt.Block(Block());
            }

            return ExpressionStatement();
        }

        Stmt ForStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' after 'for'.");
            Stmt? initializer;
            if (Match(SEMICOLON))
            {
                initializer = null;
            }
            else if (Match(VAR))
            {
                initializer = VarDeclaration();
            }
            else
            {
                initializer = ExpressionStatement();
            }

            Expr? condition = null;
            if (!Check(SEMICOLON))
            {
                condition = Expression();
            }

            Consume(SEMICOLON, "Expect ';' after loop condition.");
            Expr? increment = null;
            if (!Check(RIGHT_PAREN))
            {
                increment = Expression();
            }

            Consume(RIGHT_PAREN, "Expect ')' after for clauses.");
            var body = Statement();
            if (increment != null)
            {
                body = new Stmt.Block([body, new Stmt.Expression(increment)]);
            }

            if (condition == null)
            {
                condition = new Expr.Literal(true);
            }

            body = new Stmt.While(condition, body);
            if (initializer != null)
            {
                body = new Stmt.Block([initializer, body]);
            }

            return body;
        }

        Stmt IfStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' after 'if'.");
            var condition = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after if condition.");
            var thenBranch = Statement();
            Stmt? elseBranch = null;
            if (Match(ELSE))
            {
                elseBranch = Statement();
            }

            return new Stmt.If(condition, thenBranch, elseBranch);
        }

        Stmt PrintStatement()
        {
            var value = Expression();
            Consume(SEMICOLON, "Expect ';' after value.");
            return new Stmt.Print(value);
        }

        Stmt ReturnStatement()
        {
            var keyword = Previous();
            Expr? value = null;
            if (!Check(SEMICOLON))
            {
                value = Expression();
            }

            Consume(SEMICOLON, "Expect ';' after return value.");
            return new Stmt.Return(keyword, value);
        }

        Stmt VarDeclaration()
        {
            var name = Consume(IDENTIFIER, "Expect variable name.");
            Expr? initializer = null;
            if (Match(EQUAL))
            {
                initializer = Expression();
            }

            Consume(SEMICOLON, "Expect ';' after variable declaration.");
            return new Stmt.Var(name, initializer);
        }

        Stmt WhileStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' after 'while'.");
            var condition = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after condition.");
            var body = Statement();

            return new Stmt.While(condition, body);
        }

        Stmt ExpressionStatement()
        {
            var expr = Expression();
            Consume(SEMICOLON, "Expect ';' after expression.");
            return new Stmt.Expression(expr);
        }

        Stmt.Function Function(string kind)
        {
            var name = Consume(IDENTIFIER, $"Expect {kind} name.");
            Consume(LEFT_PAREN, $"Expect '(' after {kind} name.");
            var parameters = new List<Token>();
            if (!Check(RIGHT_PAREN))
            {
                do
                {
                    if (parameters.Count >= 255)
                    {
                        Error(Peek(), "Can't have more than 255 parameters.");
                    }

                    parameters.Add(Consume(IDENTIFIER, "Expect parameter name."));
                } while (Match(COMMA));
            }

            Consume(RIGHT_PAREN, "Expect ')' after parameters.");
            Consume(LEFT_BRACE, $"Expect '{{' before {kind} body.");
            var body = Block();
            return new Stmt.Function(name, parameters, body);
        }

        List<Stmt> Block()
        {
            var statements = new List<Stmt>();
            while (!Check(RIGHT_BRACE) && !IsAtEnd())
            {
                var declaration = Declaration();
                if (declaration != null)
                {
                    statements.Add(declaration);
                }
            }

            Consume(RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }

        Expr Assignment()
        {
            var expr = Or();
            if (Match(EQUAL))
            {
                var equals = Previous();
                var value = Assignment();
                if (expr is Expr.Variable variableExpr)
                {
                    var name = variableExpr.Name;
                    return new Expr.Assign(name, value);
                }
                else if (expr is Expr.Get getExpr)
                {
                    return new Expr.Set(getExpr.Obj, getExpr.Name, value);
                }
            }

            return expr;
        }

        Expr Or()
        {
            var expr = And();
            while (Match(OR))
            {
                var op = Previous();
                var right = And();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

        Expr And()
        {
            var expr = Equality();
            while (Match(AND))
            {
                var op = Previous();
                var right = Equality();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

        Expr Equality()
        {
            var expr = Comparison();
            while (Match(BANG_EQUAL, EQUAL_EQUAL))
            {
                var op = Previous();
                var right = Comparison();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        bool Match(params TokenTypeEnum[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        bool Check(TokenTypeEnum type)
        {
            if (IsAtEnd())
            {
                return false;
            }

            return Peek().Type == type;
        }

        Expr Comparison()
        {
            var expr = Term();
            while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
            {
                var op = Previous();
                var right = Term();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        Expr Term()
        {
            var expr = Factor();
            while (Match(MINUS, PLUS))
            {
                var op = Previous();
                var right = Factor();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        Expr Factor()
        {
            var expr = Unary();
            while (Match(SLASH, STAR))
            {
                var op = Previous();
                var right = Unary();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        Expr Unary()
        {
            if (Match(BANG, MINUS))
            {
                var op = Previous();
                var right = Unary();
                return new Expr.Unary(op, right);
            }

            return Call();
        }

        Expr FinishCall(Expr callee)
        {
            var arguments = new List<Expr>();
            if (!Check(RIGHT_PAREN))
            {
                do
                {
                    if (arguments.Count >= 255)
                    {
                        Error(Peek(), "Can't have more than 255 arguments.");
                    }

                    arguments.Add(Expression());
                } while (Match(COMMA));
            }

            var paren = Consume(RIGHT_PAREN, "Expect ')' after arguments.");
            return new Expr.Call(callee, paren, arguments);
        }

        Expr Call()
        {
            var expr = Primary();
            while (true)
            {
                if (Match(LEFT_PAREN))
                {
                    expr = FinishCall(expr);
                }
                else if (Match(DOT))
                {
                    var name = Consume(IDENTIFIER, "Expect property name after '.'.");
                    expr = new Expr.Get(expr, name);
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        Expr Primary()
        {
            if (Match(FALSE))
            {
                return new Expr.Literal(false);
            }

            if (Match(TRUE))
            {
                return new Expr.Literal(true);
            }

            if (Match(NIL))
            {
                return new Expr.Literal(null);
            }

            if (Match(NUMBER, STRING))
            {
                return new Expr.Literal(Previous().Literal);
            }

            if (Match(THIS))
            {
                return new Expr.This(Previous());
            }

            if (Match(IDENTIFIER))
            {
                return new Expr.Variable(Previous());
            }

            if (Match(LEFT_PAREN))
            {
                var expr = Expression();
                Consume(RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }

            throw Error(Peek(), "Expect expression.");
        }

        Token Consume(TokenTypeEnum type, string message)
        {
            if (Check(type))
            {
                return Advance();
            }

            throw Error(Peek(), message);
        }

        ParseException Error(Token token, string message)
        {
            error.Error(token, message);
            return new ParseException();
        }

        void Synchronize()
        {
            Advance();
            while (!IsAtEnd())
            {
                if (Previous().Type == SEMICOLON)
                {
                    return;
                }

                switch (Peek().Type)
                {
                    case CLASS:
                    case FUN:
                    case VAR:
                    case FOR:
                    case IF:
                    case WHILE:
                    case PRINT:
                    case RETURN:
                        return;
                }

                Advance();
            }            
        }

        Token Advance()
        {
            if (!IsAtEnd())
            {
                current++;
            }

            return Previous();
        }

        bool IsAtEnd() => Peek().Type == EOF;

        Token Peek() => tokens[current];

        Token Previous() => tokens[current - 1];
    }
}

