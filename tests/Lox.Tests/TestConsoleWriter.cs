using System.Text;

namespace Lox.Tests
{
    public class TestConsoleWriter : ConsoleWriter
    {
        StringBuilder sbOut = new();
        StringBuilder sbError = new();
        public override void WriteStdOut(string? value) => sbOut.Append(value);
        public override void WriteLineStdOut(string? value) => sbOut.AppendLine(value);
        public override void WriteLineStdError(string? value) => sbError.Append(value);
        public string GetStdOut() => sbOut.ToString();
        public string GetStdErr() => sbError.ToString();
    }
}
