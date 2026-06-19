namespace Lox
{
    internal class Token
    {
        readonly TokenTypeEnum type;
        readonly string lexeme;
        readonly object? literal;
        readonly int line;

        public Token(TokenTypeEnum type, string lexeme, object? literal, int line)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public override string ToString() =>
            $"{type} {lexeme} {literal}";
    }
}
