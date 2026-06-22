namespace Lox
{
    public record Token(TokenTypeEnum Type, string Lexeme, object? Literal = null, int Line = 0)
    {
        public override string ToString() =>
            $"{Type} {Lexeme} {Literal}";
    }
}
