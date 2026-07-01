using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_variable_shadow_and_local
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"{
  var a = ""outer"";
  {
    print a; // expect: outer
    var a = ""inner"";
    print a; // expect: inner
  }
}";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"outer\r\ninner\r\n"
);
    }
}
