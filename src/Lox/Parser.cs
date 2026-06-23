using static Lox.TokenTypeEnum;

namespace Lox
{
    public class Parser(List<Token> tokens)
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

        Stmt Statement()
        {
            if (Match(PRINT))
            {
                return PrintStatement();
            }

            if (Match(LEFT_BRACE))
            {
                return new Stmt.Block(Block());
            }

            return ExpressionStatement();
        }

        Stmt PrintStatement()
        {
            var value = Expression();
            Consume(SEMICOLON, "Expect ';' after value.");
            return new Stmt.Print(value);
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

        Stmt ExpressionStatement()
        {
            var expr = Expression();
            Consume(SEMICOLON, "Expect ';' after expression.");
            return new Stmt.Expression(expr);
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
            var expr = Equality();

            if (Match(EQUAL))
            {
                var equals = Previous();
                var value = Assignment();

                if (expr is Expr.Variable variableExpr)
                {
                    var name = variableExpr.Name;
                    return new Expr.Assign(name, value);
                }
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

            return Primary();
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
            Lox.Error(token, message);
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
            }

            Advance();
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

