using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_variable_scope_reuse_in_different_blocks
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"{
  var a = ""first"";
  print a; // expect: first
}

{
  var a = ""second"";
  print a; // expect: second
}
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"first\r\nsecond\r\n"
);
    }
}
