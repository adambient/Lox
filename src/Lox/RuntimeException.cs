namespace Lox
{
    public class RuntimeException : ApplicationException
    {
        public Token Token { get; }

        public RuntimeException(Token token, string message) : base(message)
        {
            Token = token;
        }
    }
}
