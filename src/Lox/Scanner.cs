namespace Lox
{
    internal class Scanner(string source)
    {
        int start = 0;
        int current = 0;
        int line = 1;
        readonly List<Token> tokens = [];
        readonly static Dictionary<string, TokenTypeEnum> keywords = new()
        {
            { "and", TokenTypeEnum.AND },
            { "class", TokenTypeEnum.CLASS },
            { "else", TokenTypeEnum.ELSE },
            { "false", TokenTypeEnum.FALSE },
            { "for", TokenTypeEnum.FOR },
            { "fun", TokenTypeEnum.FUN },
            { "if", TokenTypeEnum.IF },
            { "nil", TokenTypeEnum.NIL },
            { "or", TokenTypeEnum.OR },
            { "print", TokenTypeEnum.PRINT },
            { "return", TokenTypeEnum.RETURN },
            { "super", TokenTypeEnum.SUPER },
            { "this", TokenTypeEnum.THIS },
            { "true", TokenTypeEnum.TRUE },
            { "var", TokenTypeEnum.VAR },
            { "while", TokenTypeEnum.WHILE }
        };

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                // we are at the beginning of the next lexeme
                start = current;
                ScanToken();
            }

            tokens.Add(new Token(TokenTypeEnum.EOF, string.Empty, null, line));
            return tokens;
        }

        bool IsAtEnd() => current >= source.Length;

        void ScanToken()
        {
            var c = Advance();
            switch (c)
            {
                case '(':
                    AddToken(TokenTypeEnum.LEFT_PAREN);
                    break;
                case ')':
                    AddToken(TokenTypeEnum.RIGHT_PAREN);
                    break;
                case '{':
                    AddToken(TokenTypeEnum.LEFT_BRACE);
                    break;
                case '}':
                    AddToken(TokenTypeEnum.RIGHT_BRACE);
                    break;
                case ',':
                    AddToken(TokenTypeEnum.COMMA);
                    break;
                case '.':
                    AddToken(TokenTypeEnum.DOT);
                    break;
                case '-':
                    AddToken(TokenTypeEnum.MINUS);
                    break;
                case '+':
                    AddToken(TokenTypeEnum.PLUS);
                    break;
                case ';':
                    AddToken(TokenTypeEnum.SEMICOLON);
                    break;
                case '*':
                    AddToken(TokenTypeEnum.STAR);
                    break;
                case '!':
                    AddToken(Match('=') ? TokenTypeEnum.BANG_EQUAL : TokenTypeEnum.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenTypeEnum.EQUAL_EQUAL : TokenTypeEnum.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenTypeEnum.LESS_EQUAL : TokenTypeEnum.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenTypeEnum.GREATER_EQUAL : TokenTypeEnum.GREATER);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        // a comment goes until the end of the line
                        while (Peek() != '\n' && !IsAtEnd())
                        {
                            Advance();
                        }
                    }
                    else
                    {
                        AddToken(TokenTypeEnum.SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    // ignore whitespace
                    break;
                case '\n':
                    line++;
                    break;
                case '"':
                    String();
                    break;
                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Lox.Error(line, "Unexpected character.");
                    }
                    break;
            }
        }

        char Advance() => source[current++];

        void AddToken(TokenTypeEnum type, object? literal = null)
        {
            var text = source.Substring(start, current);
            tokens.Add(new Token(type, text, literal, line));
        }

        bool Match(char expected)
        {
            if (IsAtEnd() || source[current] != expected)
            {
                return false;
            }

            current++;
            return true;
        }

        char Peek()
        {
            if (IsAtEnd())
            {
                return '\0';
            }

            return source[current];
        }

        void String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                {
                    line++;
                }

                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(line, "Unterminated string.");
                return;
            }

            // the closing "
            Advance();

            // trim the surrounding quotes
            var value = source.Substring(start + 1, current - 1);
            AddToken(TokenTypeEnum.STRING, value);
        }

        void Number()
        {
            while (IsDigit(Peek()))
            {
                Advance();
            }

            // look for fractional part
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // consume the "."
                Advance();

                while (IsDigit(Peek()))
                {
                    Advance();
                }
            }

            AddToken(TokenTypeEnum.NUMBER, double.Parse(source.Substring(start, current)));
        }

        char PeekNext()
        {
            if (current + 1 >= source.Length)
            {
                return '\0';
            }

            return source[current + 1];
        }

        void Identifier()
        {
            while (IsAlphaNumberic(Peek()))
            {
                Advance();
            }

            var text = source.Substring(start, current);
            var type = TokenTypeEnum.IDENTIFIER;
            if (keywords.TryGetValue(text, out TokenTypeEnum value))
            {
                type = value;
            }

            AddToken(type);
        }

        static bool IsDigit(char c) =>
            c >= '0' && c <= '9';

        static bool IsAlpha(char c) =>
            (c >= 'a' && c <= 'z') ||
            (c >= 'A' && c <= 'Z') ||
            c == '_';

        static bool IsAlphaNumberic(char c) =>
            IsAlpha(c) || IsDigit(c);
    }
}
