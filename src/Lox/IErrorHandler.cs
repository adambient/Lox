namespace Lox
{
    public interface IErrorHandler
    {
        void Error(int line, string message);
        void Error(Token token, string message);
        void RuntimeException(RuntimeException exception);
    }
}
