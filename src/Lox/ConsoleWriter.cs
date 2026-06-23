namespace Lox
{
    public class ConsoleWriter
    {
        public virtual void WriteStdOut(string? value) => Console.Out.Write(value);

        public virtual void WriteLineStdOut(string? value) => Console.Out.WriteLine(value);

        public virtual void WriteLineStdError(string? value) => Console.Error.WriteLine(value);
    }
}
