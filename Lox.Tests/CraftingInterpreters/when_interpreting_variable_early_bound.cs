using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_variable_early_bound
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"var a = ""outer"";
{
  fun foo() {
    print a;
  }

  foo(); // expect: outer
  var a = ""inner"";
  foo(); // expect: outer
}
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"outer\r\nouter\r\n"
);
    }
}
