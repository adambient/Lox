namespace Lox
{
    public interface ILoxCallable
    {
        int Arity();
        object? Call(Interpreter interpreter, List<object?> arguments);
        public class Clock : ILoxCallable
        {
            int ILoxCallable.Arity() => 0;
            object? ILoxCallable.Call(Interpreter interpreter, List<object?> arguments) =>
                DateTime.UtcNow.Millisecond / 1000.0;
            public override string ToString() => "<native fn>";
        }
    }
}
