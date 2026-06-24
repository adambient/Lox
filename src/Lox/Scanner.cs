using static Lox.TokenTypeEnum;

namespace Lox
{
    public class Scanner(string source, IErrorHandler error)
    {
        int start = 0;
        int current = 0;
        int line = 1;
        readonly List<Token> tokens = [];
        readonly static Dictionary<string, TokenTypeEnum> keywords = new()
        {
            { "and", AND },
            { "class", CLASS },
            { "else", ELSE },
            { "false", FALSE },
            { "for", FOR },
            { "fun", FUN },
            { "if", IF },
            { "nil", NIL },
            { "or", OR },
            { "print", PRINT },
            { "return", RETURN },
            { "super", SUPER },
            { "this", THIS },
            { "true", TRUE },
            { "var", VAR },
            { "while", WHILE }
        };

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                // we are at the beginning of the next lexeme
                start = current;
                ScanToken();
            }

            tokens.Add(new Token(EOF, string.Empty, null, line));
            return tokens;
        }

        bool IsAtEnd() => current >= source.Length;

        void ScanToken()
        {
            var c = Advance();
            switch (c)
            {
                case '(':
                    AddToken(LEFT_PAREN);
                    break;
                case ')':
                    AddToken(RIGHT_PAREN);
                    break;
                case '{':
                    AddToken(LEFT_BRACE);
                    break;
                case '}':
                    AddToken(RIGHT_BRACE);
                    break;
                case ',':
                    AddToken(COMMA);
                    break;
                case '.':
                    AddToken(DOT);
                    break;
                case '-':
                    AddToken(MINUS);
                    break;
                case '+':
                    AddToken(PLUS);
                    break;
                case ';':
                    AddToken(SEMICOLON);
                    break;
                case '*':
                    AddToken(STAR);
                    break;
                case '!':
                    AddToken(Match('=') ? BANG_EQUAL : BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? EQUAL_EQUAL : EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? LESS_EQUAL : LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? GREATER_EQUAL : GREATER);
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
                        AddToken(SLASH);
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
                        error.Error(line, "Unexpected character.");
                    }
                    break;
            }
        }

        char Advance() => source[current++];

        void AddToken(TokenTypeEnum type, object? literal = null)
        {
            var text = source.Substring(start, current - start);
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
                error.Error(line, "Unterminated string.");
                return;
            }

            // the closing "
            Advance();

            // trim the surrounding quotes
            var value = source.Substring(start + 1, current - start - 2);
            AddToken(STRING, value);
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

            AddToken(NUMBER, double.Parse(source.Substring(start, current - start)));
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

            var text = source.Substring(start, current - start);
            var type = IDENTIFIER;
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
