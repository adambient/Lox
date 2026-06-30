namespace Lox
{
    public class ReturnException : ApplicationException
    {
        public object? Value { get; }
        public ReturnException(object? value)
        {
            Value = value;
        }
    }
}
