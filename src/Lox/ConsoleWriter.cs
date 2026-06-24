namespace Lox
{
    public class ConsoleWriter
    {
        public virtual void StdOut(string? value) => Console.Out.Write(value);

        public virtual void StdOutLn(string? value) => Console.Out.WriteLine(value);

        public virtual void StdErrorLn(string? value) => Console.Error.WriteLine(value);
    }
}
